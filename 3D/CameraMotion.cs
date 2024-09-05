using System.Numerics;
namespace Uniray
{
    public class CameraMotion
    {
        /// <summary>
        /// Y Offset of the camera
        /// </summary>
        private float yOffset;
        /// <summary>
        /// Distance of the camera to its target
        /// </summary>
        private float distance;
        /// <summary>
        /// The current position of the mouse on the screen
        /// </summary>
        private Vector2 mousePos;
        /// <summary>
        /// The origin movement of the mouse
        /// </summary>
        private Vector2 mouseOrigin;
        /// <summary>
        /// Fake position of the mouse
        /// </summary>
        private Vector2 fakePos;
        /// <summary>
        /// Y Offset of the camera
        /// </summary>
        public float YOffset { get { return yOffset; } set { yOffset = value; } }
        /// <summary>
        /// Distance of the camera to its target
        /// </summary>
        public float Distance { get { return distance; } set { distance = value; } }
        /// <summary>
        /// The current position of the mouse on the screen
        /// </summary>
        public Vector2 Mouse { get { return mousePos; } set { mousePos = value; } }
        /// <summary>
        /// The origin movement of the mouse
        /// </summary>
        public Vector2 MouseOrigin { get { return mouseOrigin; } set { mouseOrigin = value; } }
        /// <summary>
        /// Fake position of the mouse
        /// </summary>
        public Vector2 FakePosition { get { return fakePos; } set { fakePos = value; } }
        /// <summary>
        /// CameraMotion constructor
        /// </summary>
        /// <param name="distance">Distance of the camera to its target</param>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public CameraMotion(float distance, short width, short height)
        {
            this.distance = distance;
            yOffset = 0;
            // Center the intial position of the mouse to the center of the screen
            mousePos = new Vector2(width / 2, height / 2);
            // Set other vector variables to zero
            mouseOrigin = Vector2.Zero;
            fakePos = Vector2.Zero;
        }
    }
}