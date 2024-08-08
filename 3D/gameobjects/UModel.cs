using System.Net.WebSockets;
using System.Numerics;
using Raylib_cs;

namespace Uniray
{
    public unsafe class UModel : GameObject3D
    {
        /// <summary>
        /// Yaw rotation
        /// </summary>
        private float yaw;
        /// <summary>
        /// Pitch rotation
        /// </summary>
        private float pitch;
        /// <summary>
        /// Roll rotation
        /// </summary>
        private float roll;
        /// <summary>
        /// Model mesh
        /// </summary>
        private Mesh mesh;
        /// <summary>
        /// Model material
        /// </summary>
        private Material material;
        /// <summary>
        /// Model transform matrix
        /// </summary>
        private Matrix4x4 transform;
        /// <summary>
        /// Texture ID of the dictionnary
        /// </summary>
        private string textureID;
        /// <summary>
        /// Model ID of the dictionnary
        /// </summary>
        private string modelID;
        /// <summary>
        /// 3-Dimensional position of the object
        /// </summary>
        public override Vector3 Position 
        {
            get 
            {
                return new Vector3(transform.M14, transform.M24, transform.M34);
            } 
            set 
            {
                transform.M14 = value.X; 
                transform.M24 = value.Y; 
                transform.M34 = value.Z;
            } 
        }
        /// <summary>
        /// Yaw rotation
        /// </summary>
        public float Yaw { get { return yaw; } set { yaw = value; } }
        /// <summary>
        /// Pitch rotation
        /// </summary>
        public float Pitch { get { return pitch; } set { pitch = value; } }
        /// <summary>
        /// Roll rotation
        /// </summary>
        public float Roll { get { return roll; } set { roll = value; } }
        /// <summary>
        /// Object model
        /// </summary>
        public Mesh Mesh { get { return mesh; } set { mesh = value; } } 
        /// <summary>
        /// Texture ID in the dictionnary
        /// </summary>
        public string TextureID { get { return textureID; } set { textureID = value; } }
        /// <summary>
        /// Model ID in the dictionnary
        /// </summary>
        public string ModelID { get { return modelID; } set { modelID = value; } }
        /// <summary>
        /// Model transform matrix
        /// </summary>
        public Matrix4x4 Transform { get { return transform; } set { transform = value; } }
        /// <summary>
        /// Model material
        /// </summary>
        public Material Material { get { return material; } set { material = value; } }
        /// <summary>
        /// UModel default Constructor
        /// </summary>
        public UModel() : base()
        {
            this.textureID = "";
            this.modelID = "";
            this.Transform = Matrix4x4.Identity;
            Material = Raylib.LoadMaterialDefault();
        }
        /// <summary>
        /// UModel Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="model">Object model</param>
        /// <param name="modelPath">Object model path</param>
        public UModel(string name, Vector3 position, Mesh mesh, string modelID) : base(name, position)
        {
            Mesh = mesh;
            Position = position;
            this.modelID = modelID;
            this.textureID = "";
            this.Transform = Matrix4x4.Identity;
            this.transform.M14 = position.X;
            this.transform.M24 = position.Y;
            this.transform.M34 = position.Z;
            Material = Raylib.LoadMaterialDefault();
        }
        /// <summary>
        /// UModel Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="model">Object model</param>
        /// <param name="modelPath">Object model path</param>
        /// <param name="textureID">Object texture ID</param>
        public UModel(string name, Vector3 position, Mesh mesh, string modelID, string textureID) : base(name, position)
        {
            Mesh = mesh;
            Position = position;
            this.modelID = modelID;
            this.textureID = textureID;
            this.Transform = Matrix4x4.Identity;
            this.transform.M14 = position.X;
            this.transform.M24 = position.Y;
            this.transform.M34 = position.Z;
            Material = Raylib.LoadMaterialDefault();
        }
        /// <summary>
        /// Set the model texture by giving on of the ressource's texture
        /// </summary>
        /// <param name="textureID"></param>
        /// <param name="texture"></param>
        public void SetTexture(string textureID, Texture2D texture)
        {
            this.textureID = textureID;
            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, texture);
        }
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Rotation: < " + Yaw + "; " + Pitch + "; " + Roll + " >";  
        }
    }
}