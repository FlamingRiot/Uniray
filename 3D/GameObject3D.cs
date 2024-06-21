using System.Numerics;

namespace Uniray
{
    public abstract class GameObject3D
    {
        /// <summary>
        /// Name of the object
        /// </summary>
        private string name;
        /// <summary>
        /// 3-Dimensional position of the object
        /// </summary>
        protected Vector3 position;
        /// <summary>
        /// Behaviour of the object
        /// </summary>
        private Behaviour? behaviour;
        /// <summary>
        /// Name of the object
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// 3-Dimensional position of the object
        /// </summary>
        abstract public Vector3 Position { get; set; }
        /// <summary>
        /// Behaviour of the object
        /// </summary>
        public Behaviour? Behaviour { get { return behaviour; } set { behaviour = value; } }
        /// <summary>
        /// X Position of the vector
        /// </summary>
        public float X { get { return position.X; } set { position.X = value; } }
        /// <summary>
        /// Y Position of the vector
        /// </summary>
        public float Y { get { return position.Y; } set { position.Y = value; } }
        /// <summary>
        /// Z Position of the vector
        /// </summary>
        public float Z { get { return position.Z; } set { position.Z = value; } }
        /// <summary>
        /// GameObject3D default constructor
        /// </summary>
        public GameObject3D() 
        {
            this.name = "";
            this.position = new Vector3();
        }
        /// <summary>
        /// GameObject3D Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        public GameObject3D(string name, Vector3 position)
        {
            this.name = name;
            this.position = position;
        }
        /// <summary>
        /// GameObject3D Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="behaviour">Object behaviour script</param>
        public GameObject3D(string name, Vector3 position, Behaviour? behaviour)
        {
            this.name = name;
            this.position = position;
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Send object informations
        /// </summary>
        /// <returns>Stringified informations</returns>
        public override string ToString()
        {
            return "Name : " + this.name + " Position : ";
        }
    }
}