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
        /// Model path
        /// </summary>
        private Model model;
        /// <summary>
        /// Texture ID of the dictionnary
        /// </summary>
        private string textureID;
        /// <summary>
        /// Model absolute path
        /// </summary>
        private string modelPath;
        /// <summary>
        /// 3-Dimensional position of the object
        /// </summary>
        public override Vector3 Position { get { return position; } set { position = value; } }
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
        public Model Model { get { return model; } set { model = value; } } 
        /// <summary>
        /// Texture ID in the dictionnary
        /// </summary>
        public string TextureID { get { return textureID; } set { textureID = value; } }
        /// <summary>
        /// Model absolute path
        /// </summary>
        public string ModelPath { get { return modelPath; } set { modelPath = value; } }
        /// <summary>
        /// UModel default Constructor
        /// </summary>
        public UModel() : base()
        {
            this.textureID = "";
            this.modelPath = "";
        }
        /// <summary>
        /// UModel Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="model">Object model</param>
        /// <param name="modelPath">Object model path</param>
        public UModel(string name, Vector3 position, Model model, string modelPath) : base(name, position)
        {
            this.model = model;
            this.modelPath = modelPath;
            this.textureID = "";
        }
        /// <summary>
        /// UModel Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="model">Object model</param>
        /// <param name="modelPath">Object model path</param>
        /// <param name="textureID">Object texture ID</param>
        public UModel(string name, Vector3 position, Model model, string modelPath, string textureID) : base(name, position)
        {
            this.model = model;
            this.modelPath = modelPath;
            this.textureID = textureID;
        }
        /// <summary>
        /// Set the model texture by giving on of the ressource's texture
        /// </summary>
        /// <param name="textureID"></param>
        /// <param name="texture"></param>
        public void SetTexture(string textureID, Texture2D texture)
        {
            this.textureID = textureID;
            Raylib.SetMaterialTexture(this.model.Materials, MaterialMapIndex.Diffuse, texture);
        }
        /// <summary>
        /// Set the model transofmr for rotations and scaling
        /// </summary>
        /// <param name="transform">4x4 matrix</param>
        public void SetTransform(Matrix4x4 transform)
        {
            this.model.Transform = transform;
        }
        /// <summary>
        /// Get the model transform for rotations and scaling
        /// </summary>
        /// <returns>4x4 Matrix</returns>
        public Matrix4x4 GetTransform() 
        { 
            return this.model.Transform;
        }
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Rotation: < " + Yaw + "; " + Pitch + "; " + Roll + " >";  
        }
    }
}