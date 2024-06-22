﻿using static Raylib_cs.Raylib;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using System.Numerics;
using System.Text;
using System.Globalization;

namespace Uniray
{
    public class Program
    {
        public float CameraDistance;

        unsafe static void Main(string[] args)
        {
            // Initialize window and set mode
            InitWindow(1800, 900, "Uniray - New Project");
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);
            SetWindowIcon(LoadImageFromTexture(LoadTexture("data/logo.png")));
            InitGUI(Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);

            // Change culture info to enable parsing float values in json
            var clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            clone.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = clone;
            Thread.CurrentThread.CurrentUICulture = clone;
            
            // Load font
            Font font = LoadFont("data/font/Ubuntu-Regular.ttf");

            // Get Window size
            int wWindow = GetScreenWidth();
            int hWindow = GetScreenHeight();

            // Set 3D camera for the default scene
            Camera3D camera = new Camera3D();
            camera.Projection = CameraProjection.Perspective;
            camera.Position = new Vector3(5, 5, 0);
            camera.Target = Vector3.Zero;
            camera.Up = Vector3.UnitY;
            camera.FovY = 90f;
            float camYOffset = 0f;
            float camDistance = 2f;
            Vector2 mousePos = new Vector2(wWindow / 2, hWindow / 2);
            Vector2 mouseMovementOrigin = Vector2.Zero;
            Vector2 fakePos = Vector2.Zero;

            // Set UI and application default
            Scene scene = new Scene(new List<GameObject3D>());
            Uniray uniray = new Uniray(wWindow, hWindow, font, scene);

            while (!WindowShouldClose())
            {

                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.S))
                {
                    uniray.SaveProject();
                }

                // =========================================================================================================================================================
                // ============================================================= MANAGE 3D ENVIRONMENT =====================================================================
                // =========================================================================================================================================================
                
                if (Hover((int)uniray.GameManager.X + uniray.GameManager.Width + 10, 0, wWindow - uniray.GameManager.Width - 20, hWindow - uniray.FileManager.Height - 20))
                {
                    if (IsMouseButtonReleased(MouseButton.Middle))
                    {
                        mousePos = fakePos;
                        mouseMovementOrigin = Vector2.Zero;
                    }
                    if (IsMouseButtonDown(MouseButton.Middle))
                    {

                        if (IsKeyDown(KeyboardKey.LeftShift))
                        {
                            Vector3 movX = GetCameraRight(ref camera) * GetMouseDelta().X * (camDistance / 200);
                            Vector3 movY = GetCameraUp(ref camera) * GetMouseDelta().Y * (camDistance / 200);

                            camera.Position -= movX;
                            camera.Target -= movX;

                            camera.Position += movY;
                            camera.Target += movY;
                        }
                        else
                        {
                            if (mouseMovementOrigin == Vector2.Zero) { mouseMovementOrigin = GetMousePosition(); }
                            fakePos = MoveCamera(camDistance, ref camera, camera.Target, camYOffset, false, mousePos, mouseMovementOrigin);
                        }
                    }
                    else
                    {
                        camDistance -= GetMouseWheelMove() * 2f;
                        MoveCamera(camDistance, ref camera, camera.Target, camYOffset, true, mousePos, mouseMovementOrigin);
                    }
                    uniray.EnvCamera = camera;
                }
                // =========================================================================================================================================================

                // Manage resize options
                if (IsWindowResized())
                {
                    hWindow = GetScreenHeight();
                    wWindow = GetScreenWidth();
                    uniray = new Uniray(wWindow, hWindow, font, uniray.CurrentScene);
                }
                // =========================================================================================================================================================
                // ============================================================= MANAGE OVERALL DRAWING ====================================================================
                // =========================================================================================================================================================

                BeginDrawing();

                ClearBackground(new Color(70, 70, 70, 255));

                BeginMode3D(camera);

                DrawGrid(10, 10);

                if (uniray.CurrentProject is not null) uniray.DrawScene();

                EndMode3D();

                uniray.DrawUI();

                DrawFPS(40, 200);

                EndDrawing();
            }
            CloseWindow();
        }

        /// <summary>
        /// Move the conceptor's camera
        /// </summary>
        /// <param name="distance">Distance from the target</param>
        /// <param name="camera">3D camera (conceptor's)</param>
        /// <param name="targetPosition">Target of the camera</param>
        /// <param name="yOffset">Well I don't even remember what this is</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="mousePos">Last position of the mouse</param>
        /// <param name="mouseOrigin">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 MoveCamera(float distance, ref Camera3D camera, Vector3 targetPosition, float yOffset, bool zoom, Vector2 mousePos, Vector2 mouseOrigin)
        {
            float alpha = 0;
            float beta = 0;
            Vector2 verticalPosition = CalculateVerticalPosition(distance, targetPosition, ref alpha, zoom, mousePos, mouseOrigin);
            Vector2 HorizontalPosition = CalculateHorizontalPosition(distance, targetPosition, ref beta, zoom, mousePos, mouseOrigin);
            camera.Position.Y = verticalPosition.Y + yOffset;
            camera.Position.X = HorizontalPosition.X;
            camera.Position.Z = HorizontalPosition.Y;

            return mousePos - (mouseOrigin - GetMousePosition());
        }

        /// <summary>
        /// Calculate the vertical position of the conceptor's camera
        /// </summary>
        /// <param name="distance">Distance from the target</param>
        /// <param name="targetPosition">Target of the camera</param>
        /// <param name="alpha">Alpha angle</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="m">Last position of the mouse</param>
        /// <param name="mO">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 CalculateVerticalPosition(float distance, Vector3 targetPosition, ref float alpha, bool zoom, Vector2 m, Vector2 mO)
        {
            if (!zoom) alpha = (m.Y - (mO.Y - GetMousePosition().Y)) * 0.005f;
            else alpha = m.Y * 0.005f;
            float offsetZ = (float)(distance * Math.Cos(alpha));
            float offsetY = (float)(distance * Math.Sin(alpha));
            float posY = targetPosition.Y + offsetY;
            float posZ = targetPosition.Z + offsetZ;
            return new Vector2(posZ, posY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance">Distance from the target</param>
        /// <param name="targetPosition">Target of the camera</param>
        /// <param name="beta">Beta angle</param>
        /// <param name="zoom">Is zoom possible ?</param>
        /// <param name="m">Last position of the mouse</param>
        /// <param name="mO">First position of the mouse when interacting with movement</param>
        /// <returns></returns>
        static Vector2 CalculateHorizontalPosition(float distance, Vector3 targetPosition, ref float beta, bool zoom, Vector2 m, Vector2 mO)
        {
            if (!zoom) beta = (m.X - (mO.X - GetMousePosition().X)) * 0.005f;
            else beta = m.X * 0.005f;
            float offsetX = (float)(distance * Math.Cos(beta));
            float offsetZ = (float)(distance * Math.Sin(beta));
            float posX = targetPosition.X + offsetX;
            float posZ = targetPosition.Z + offsetZ;
            return new Vector2(posX, posZ); 
        }
    }
}