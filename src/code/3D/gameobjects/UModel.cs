using System.Numerics;
using Microsoft.VisualBasic;
using Raylib_cs;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UModel"/>, inheriting <see cref="GameObject3D"/>.</summary>
    public unsafe class UModel : GameObject3D
    {
        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        /// <summary>Meshes of the object.</summary>
        public Mesh[] Meshes = new Mesh[0];
        /// <summary>Materials of the object.</summary>
        public Material[] Materials = new Material[0];

        /// <summary>X Axis rotation.</summary>
        public float Pitch;
        /// <summary>Y Axis rotation</summary>
        public float Yaw;
        /// <summary>Z Axis rotation.</summary>
        public float Roll;

        /// <summary>Texture ID in the dictionary.</summary>
        public string TextureID;
        /// <summary>Model ID in the dictionary.</summary>
        public string ModelID;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------
        /// <summary>3-Dimensional position of the object.</summary>
        public override Vector3 Position 
        {
            get 
            {
                return new Vector3(Transform.M14, Transform.M24, Transform.M34);
            } 
            set 
            {
                Transform.M14 = value.X; 
                Transform.M24 = value.Y; 
                Transform.M34 = value.Z;
            } 
        }

        /// <summary>Gets the amount of meshes the object has.</summary>
        public int MeshCount {  get { return Meshes.Length; } }
        /// <summary>Gets the amount of materials the object has.</summary>
        public int MaterialCount {  get { return Materials.Length; } }

        /// <summary>Creates an empty instance of <see cref="UModel"/>.</summary>
        public UModel() : base()
        {
            TextureID = "";
            ModelID = "";
        }

        /// <summary>Creates an instance of <see cref="UModel"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        public UModel(string name, Vector3 position, string modelID) : base(name, position)
        {
            // Apply Model
            ModelID = modelID;
            LoadMeshes();

            // Load default textures
            TextureID = "";
            Materials = new Material[1];
            Materials[0] = Raylib.LoadMaterialDefault();

            Transform = Matrix4x4.Identity;
            Position = position;
        }

        /// <summary>Creates an instance of <see cref="UModel"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="textureID">Object texture ID</param>
        public UModel(string name, Vector3 position, string modelID, string textureID) : base(name, position)
        {
            // Apply Model
            ModelID = modelID;
            LoadMeshes();

            // Load textures
            TextureID = textureID;
            LoadTextures();

            Transform = Matrix4x4.Identity;
            Position = position;
            // Set the texture to the mesh
            SetTexture(textureID, 0);
        }

        /// <summary>Sets the model texture.</summary>
        /// <param name="textureID">Texture ID to apply.</param>
        public void SetTexture(string textureID, int materialIndex)
        {
            TextureID = textureID;
            try
            {
                Texture2D texture = Uniray.Ressource.GetTexture(TextureID);
                Raylib.SetMaterialTexture(ref Materials[materialIndex], MaterialMapIndex.Diffuse, texture);
            }
            catch
            {
#if !DEBUG
                ErrorHandler.Send(new Error(3, $"No texture found at given location -> {TextureID}"));
#elif DEBUG
                Raylib.TraceLog(TraceLogLevel.Warning, $"No texture found at given location -> {TextureID}");
#endif
            }
        }

        /// <summary>Applies the corresponding texture to the model (from ressources).</summary>
        internal void LoadTextures()
        {
            if (TextureID != "")
            {
                try
                {
                    // Load default materials
                    int materialCount = Uniray.Ressource.GetModel(ModelID).MeshCount;
                    Materials = new Material[materialCount];
                    for (int i = 0; i < materialCount; i++) Materials[i] = Raylib.LoadMaterialDefault();
                    // Apply textures to loaded materials
                    Texture2D texture = Uniray.Ressource.GetTexture(TextureID);
                    Raylib.SetMaterialTexture(ref Materials[0], MaterialMapIndex.Diffuse, texture);
                }
                catch
                {
#if !DEBUG
                ErrorHandler.Send(new Error(3, $"No texture found at registered location -> {TextureID}"));
#elif DEBUG
                    Raylib.TraceLog(TraceLogLevel.Warning, $"No texture found at registered location -> {TextureID}");
#endif
                }
            }
        }

        /// <summary>Loads meshes and materials from registered keys.</summary>
        internal void LoadMeshes()
        {
            if (ModelID != "")
            {
                try
                {
                    int meshCount = Uniray.Ressource.GetModel(ModelID).MeshCount;
                    Meshes = new Mesh[meshCount];
                    Mesh* meshes = Uniray.Ressource.GetModel(ModelID).Meshes;
                    for (int i = 0; i < meshCount; i++) Meshes[i] = meshes[i];
                }
                catch
                {
#if !DEBUG
                ErrorHandler.Send(new Error(3, $"No model found at registered location -> {ModelID}"));
#elif DEBUG
                    Raylib.TraceLog(TraceLogLevel.Warning, $"No model found at registered location -> {ModelID}");
#endif
                }
            }
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Rotation: < " + Yaw + "; " + Pitch + "; " + Roll + " >";  
        }

        /// <summary>Rotates the object with new rotation angles.</summary>
        /// <param name="pitch">X Axis rotation.</param>
        /// <param name="yaw">Y Axis rotation.</param>
        /// <param name="roll">Z Axis rotation</param>
        public void SetRotation(float pitch, float yaw, float roll)
        {
            // Set the camera model Transform
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
            // Calculate matrix rotation
            Matrix4x4 nm = Raymath.MatrixRotateXYZ(new Vector3(pitch / Raylib.RAD2DEG, yaw / Raylib.RAD2DEG, roll / Raylib.RAD2DEG));
            // Multiply matrices
            Transform = Raymath.MatrixMultiply(Transform, nm);
        }
    }
}