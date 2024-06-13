﻿using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Uniray
{
    public unsafe class GameObject
    {
        /// <summary>
        /// Position of the object
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Rotation of the object
        /// </summary>
        private Vector3 rotation;

        /// <summary>
        /// Scale of the object
        /// </summary>
        private Vector3 scale;

        /// <summary>
        /// Model of the object
        /// </summary>
        private Model model;

        /// <summary>
        /// File path for the go's model
        /// </summary>
        private string modelPath;

        /// <summary>
        /// Dictionary key of the go's texture
        /// </summary>
        private string textureID;

        /// <summary>
        /// Behaviour
        /// </summary>
        private Behaviour behaviour;

        /// <summary>
        /// Name of the object
        /// </summary>
        private string name;

        // Properties
        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float Z { get { return position.Z; } set { position.Z = value; } }
        public float Rx { get { return rotation.X; } set { rotation.X = value; } }
        public float Ry { get { return rotation.Y; } set { rotation.Y = value; } }
        public float Rz { get { return rotation.Z; } set { rotation.Z = value; } }
        public float Sx { get { return scale.X; } set { scale.X = value; } }
        public float Sy { get { return scale.Y; } set { scale.Y = value; } }
        public float Sz { get { return scale.Z; } set { scale.Z = value; } }

        public Model Model { get { return model; } set { model = value; } }
        public string ModelPath { get { return modelPath; } set { modelPath = value; } }
        public string TextureID { get { return textureID; } set { textureID = value; } }
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Rotation { get { return rotation; } set { rotation = value; } }
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        public string Name  { get { return name; } set { name = value; } }

        /// <summary>
        /// Default constructor for the json parsing 
        /// </summary>
        public GameObject() { }

        /// <summary>
        /// Constructor 1
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        public GameObject(Vector3 position, Vector3 rotation, Vector3 scale, string name)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.name = name;
            this.model = new Model();
            this.modelPath = "";
            this.textureID = "null";

            behaviour = new Behaviour();
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        /// <param name="model">Model of the object</param>
        public GameObject(Vector3 position, Vector3 rotation, Vector3 scale, string name, Model model, string modelPath)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.name = name;
            this.model = model;
            this.modelPath = modelPath;
            this.textureID = "";

            behaviour = new Behaviour();
        }

        /// <summary>
        /// Set game object textures
        /// </summary>
        /// <param name="tex">Texture to apply</param>
        public void SetTexture(string id)
        {
            this.textureID = id;
            Raylib.SetMaterialTexture(&model.Materials[0], MaterialMapIndex.Diffuse, Uniray.Ressource.GetTexture(id));
        }
        /// <summary>
        /// Set the 4x4 matrix transform
        /// </summary>
        /// <param name="transform">4x4 Matrix</param>
        /// <param name="index">Game Object Index</param>
        public void SetTransform(Matrix4x4 transform, int index)
        {
            this.model.Transform = transform;
        }
        /// <summary>
        /// Returns the 4x4 matrix transform
        /// </summary>
        /// <param name="index">Game Object Index</param>
        /// <returns></returns>
        public Matrix4x4 GetTransform(int index)
        {
            return this.model.Transform;
        }

        /// <summary>
        /// Game object to string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "X : " + X.ToString() + "\n\nY : " + Y.ToString() + "\n\nZ : " + Z.ToString();
        }
    }
}
