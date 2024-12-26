using System.Numerics;

namespace Uniray
{
    /// <summary>Defines the necessity to be drawable.</summary>
    public interface IDrawable 
    { 
        void Draw(); // Draw function
        void DrawOutline(); // Outline Draw function
    }

    /// <summary>Defines the ability of an object to rotate.</summary>
    public interface IRotation
    {
        float Pitch { get; set; }
        float Yaw { get; set; }
        float Roll { get; set; }
    }

    /// <summary>Defines the ability of an object to be scaled.</summary>
    public interface IScalable
    {
        float Width { get; set; }
        float Height { get; set; }
        float Depth { get; set; }
    }

    /// <summary>Defines the ability of an object to cloned (copy-paste).</summary>
    /// <typeparam name="T">Type of the game object. (must inherit from <see cref="GameObject3D"/>.</typeparam>
    public interface IClonable<out T> where T : GameObject3D
    {
        T Clone();
    }

    /// <summary>Reprsents an instance of <see cref="GameObject3D"/>.</summary>
    public abstract class GameObject3D : IDrawable
    { 
        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------
        /// <summary>Transformative matrix of the object.</summary>
        public Matrix4x4 Transform;
        /// <summary>Name of the object.</summary>
        public string Name;
        /// <summary>Behvior of the object.</summary>
        public Behavior? Behavior;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------
        /// <summary>3-Dimensional object position.</summary>
        abstract public Vector3 Position { get; set; }
        /// <summary>X Position of the object.</summary>
        [ComponentField(Group = "Position")]
        public float X { get { return Transform.M14; } set { ComponentField.SetProperty(value, ref Transform.M14, nameof(X)); } }
        /// <summary>X Position of the object.</summary>
        [ComponentField(Group = "Position")]
        public float Y { get { return Transform.M24; } set { ComponentField.SetProperty(value, ref Transform.M24, nameof(Y)); } }
        /// <summary>X Position of the object.</summary>
        [ComponentField(Group = "Position")]
        public float Z { get { return Transform.M34; } set { ComponentField.SetProperty(value, ref Transform.M34, nameof(Z)); } }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        public GameObject3D() 
        {
            Name = "";
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
        }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        public GameObject3D(string name, Vector3 position)
        {
            Name = name;
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
            Position = position;
        }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="behaviour">Object behaviour script</param>
        public GameObject3D(string name, Vector3 position, Behavior? behavior)
        {
            Name = name;
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
            Position = position;
            Behavior = behavior;
        }

        /// <summary>Draws the Game Object to the screen.</summary>
        public abstract void Draw();
        /// <summary>Draws the Game Object's outline to the screen.</summary>
        public abstract void DrawOutline();

        /// <summary>Clamps a rotation angle.</summary>
        /// <param name="angle">Angle to clamp.</param>
        /// <returns>The clamped angle.</returns>
        public static float ClampAngle(float angle)
        {
            if (angle > 360 || angle < 0)
            {
                return angle - 360 * MathF.Floor(angle/360);
            }
            return angle;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name : " + Name + " Position : ";
        }
    }
}