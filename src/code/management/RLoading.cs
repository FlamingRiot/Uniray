using Raylib_cs;
using System.Numerics;

namespace Uniray.Managment
{
    /// <summary>Represents an instance of the <see cref="RLoading"/> class, used for static loading functions.</summary>
    public static class RLoading
    {
        /// <summary>Loads the base environment camera for scene rendering.</summary>
        /// <returns>The generated 3D camera.</returns>
        public static Camera3D LoadCamera()
        {
            Camera3D camera = new()
            {
                Projection = CameraProjection.Perspective,
                Position = new Vector3(5, 5, 0),
                Target = Vector3.Zero,
                Up = Vector3.UnitY,
                FovY = Uniray.CAMERA_FOV
            };
            return camera;
        }
    }
}
