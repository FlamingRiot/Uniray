using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using System.Numerics;
namespace Uniray
{
    /// <summary>Represents an instance of <see cref="Conceptor3D"/>.</summary>
    internal static class Conceptor3D
    {
        /// <summary>3D Camera used to render the conceptor.</summary>
        public static Camera3D EnvCamera;

        // -----------------------------------------------------------
        // Private and internal instances
        // -----------------------------------------------------------
        private static Vector2 mouse;
        private static Ray _mouseRay;
        private static RayCollision _mouseRayCollision;

        /// <summary>Inits the internal variables of the 3D conceptor.</summary>
        public static void Init()
        {
            _mouseRay = new Ray();
            _mouseRayCollision = new RayCollision();
        }

        /// <summary>Sets a value to the mouse ray.</summary>
        /// <param name="ray">Ray to set.</param>
        public static void UpdateMouseRay()
        {
            mouse = GetMousePosition();
            _mouseRay = GetMouseRay(mouse, EnvCamera);
        }

        /// <summary>Translates a position according to the user's inputs.</summary>
        /// <param name="position">Position of the Game object to move.</param>
        /// <returns>New position as a <see cref="Vector3"/>.</returns>
        public static Vector3 TranslateObject(Vector3 position)
        {
            if (IsKeyDown(KeyboardKey.X))
            {
                position.X += (GetCameraRight(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Y))
            {
                position.Z += (GetCameraForward(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Z))
            {
                position.Y -= (GetCameraUp(ref EnvCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, EnvCamera.Position)).Y;
            }
            else
            {
                position += GetCameraRight(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position);
                position -= GetCameraUp(ref EnvCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, EnvCamera.Position);
                
            }
            HideCursor();
            return position;
        }

        /// <summary>Rotates a model object.</summary>
        /// <param name="model">Model object.</param>
        public static void RotateObject(ref UModel model)
        {
            // Rotate the model according to the 3-Dimensional axes
            Vector2 mouse = GetMouseDelta();
            if (IsKeyDown(KeyboardKey.X))
            {
                model.Pitch += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Y))
            {
                model.Roll += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Z))
            {
                model.Yaw += mouse.X;
            }
            // Set the model tranform
            Matrix4x4 nm = MatrixRotateXYZ(new Vector3(model.Pitch / RAD2DEG, model.Yaw / RAD2DEG, model.Roll / RAD2DEG));
            nm.M14 = model.Position.X;
            nm.M24 = model.Position.Y;
            nm.M34 = model.Position.Z;

            model.Transform = nm;
        }

        /// <summary>Rotates a camera object and modifies its target.</summary>
        /// <param name="camera">Camera object.</param>
        public static void RotateObject(ref UCamera camera)
        {
            // Rotate the camera according to the 3-Dimensional axes
            Vector2 mouse = GetMouseDelta();
            if (IsKeyDown(KeyboardKey.X))
            {
                camera.Pitch += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Y))
            {
                camera.Roll += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Z))
            {
                camera.Yaw += mouse.X;
            }
            // Set the camera model Transform
            Matrix4x4 nm = MatrixRotateXYZ(new Vector3(camera.Pitch / RAD2DEG, camera.Yaw / RAD2DEG, camera.Roll / RAD2DEG));

            nm.M14 = camera.Position.X;
            nm.M24 = camera.Position.Y;
            nm.M34 = camera.Position.Z;

            camera.Transform = nm;
        }

        /// <summary>Checks whether a collision between the mouse and a <see cref="GameObject3D"/> occurs or not.</summary>
        /// <param name="go"><see cref="GameObject3D"/> to use.</param>
        /// <returns><see langword="true"/> if a collision occurs. <see langword="false"/> otherwise.</returns>
        public static int CheckCollisionScreenToWorld(GameObject3D go)
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                if (mouse.X > Uniray.UI.Components["gameManager"].X + Uniray.UI.Components["gameManager"].Width && mouse.Y < Uniray.UI.Components["fileManager"].Y - 10)
                {
                    // Type cast
                    switch (go)
                    {
                        case UModel model:
                            for (int i = 0; i < model.MeshCount; i++)
                            {
                                if (!_mouseRayCollision.Hit) _mouseRayCollision = GetRayCollisionMesh(_mouseRay, model.Meshes[i], model.Transform);
                            }
                            break;
                        case UCamera camera:
                            unsafe
                            {
                                _mouseRayCollision = GetRayCollisionMesh(_mouseRay, HardRessource.Models["camera"].Meshes[0], camera.Transform);
                            }
                            break;
                    }
                    // Return hit information
                    if (_mouseRayCollision.Hit)
                    {
                        return UData.CurrentScene.GameObjects.IndexOf(go);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>Checks the distance between two <see cref="GameObject3D"/> and returns the closest index.</summary>
        /// <param name="index1">Index of the first object</param>
        /// <param name="index2">Index of the second object</param>
        /// <returns>The index to change or keep</returns>
        public static int CheckDistance(int index1, int index2)
        {
            if (index2 != -1)
            {
                if (index1 != -1)
                {
                    // Retrive the position data of the conflicted game objects
                    Vector3 iPos1 = UData.CurrentScene.GameObjects[index1].Position;
                    Vector3 iPos2 = UData.CurrentScene.GameObjects[index2].Position;

                    if (Vector3Distance(iPos1, EnvCamera.Position) > Vector3Distance(iPos2, EnvCamera.Position))
                    {
                        // Set the new selected game object as closer
                        return index2;
                    }
                    else
                    {
                        return index1;
                    }
                }
                // Set the first potential selected element
                return index2;
            }
            // Keep the old one 
            return index1;
        }
    }
}