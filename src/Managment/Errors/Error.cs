using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class Error
    {
        /// <summary>
        /// Error code
        /// </summary>
        private int code;
        /// <summary>
        /// Error message
        /// </summary>
        private string message;
        /// <summary>
        /// Time elapsed since display
        /// </summary>
        private double time;
        /// <summary>
        /// Rectangle position
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// Rectangle size
        /// </summary>
        private Vector2 size;
        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get { return code; } }
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get { return message; } }
        /// <summary>
        /// Time elapsed since display
        /// </summary>
        public double Time { get { return time; } }
        /// <summary>
        /// Error rectangle Y position
        /// </summary>
        public float Y { get { return position.Y; } set { position.Y = value; } }
        /// <summary>
        /// Rectangle size
        /// </summary>
        public Vector2 Size { get { return size; } set { size = value; } }
        /// <summary>
        /// Error position
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }
        /// <summary>
        /// Error height
        /// </summary>
        public float Height { get { return size.Y; } set { size.Y = value; } }
        /// <summary>
        /// Error Constructor
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message/param>
        public Error(int code, string message)
        {
            this.code = code;
            this.message = message;
            this.time = Raylib.GetTime();
        }
        /// <summary>
        /// Send Error informations
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Code + ": " + message;
        }
    }
}