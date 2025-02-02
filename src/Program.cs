﻿using static Raylib_cs.Raylib;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using System.Numerics;
using System.Globalization;
using Uniray.Managment;
using Raylib_cs.Complements;

namespace Uniray
{
    /// <summary>Represents the current state of the program.</summary>
    internal enum ProgramState
    {
        Loading,
        Running,
        Reduced
    }

    /// <summary>Represents an instance of the running program.</summary>
    public class Program
    {
        internal static int Width;
        internal static int Height;
        internal static Mesh Skybox;

        internal static int SPLASH_SIZE = 162; // Splash screen size (square)

        /// <summary>Enters the entrypoint of the program.</summary>
        /// <param name="args">Passed arguments from the outside.</param>
        unsafe static void Main(string[] args)
        {
            if (args.Length > 0) // Change working directory if opened externally
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                Environment.CurrentDirectory = appDirectory;
            }

            // Initialize window and set mode
            InitWindow(SPLASH_SIZE, SPLASH_SIZE, "Uniray - New Project");
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.UndecoratedWindow);

            // Set window icon
            Image icon = LoadImage("data/img/logo.png");
            SetWindowIcon(icon);
            UnloadImage(icon);

            // Load splash screen
            Texture2D splash = LoadTexture("data/img/splash.png");
            BeginDrawing();
            DrawTexture(splash, 0, 0, Color.White);
            EndDrawing();
            UnloadTexture(splash);

            // Set program state
            Uniray.State = ProgramState.Loading;

            Width = 1800;
            Height = 900;
            Skybox = GenMeshCube(1.0f, 1.0f, 1.0f);

            // Change CultureInfo
            ChangeCultureInfo();

            // Init the GUI library with the two main colors of the application
            InitGUI(Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, LoadFont("data/font/Ubuntu-Regular.ttf"));
            SetDefaultFontSize(15);

            // Set 3D camera for the default scene
            Camera3D camera = RLoading.LoadCamera();

            // Init the engine
            Uniray.Init();
            
            // Set camera motion object
            CameraMotion motion = new(2f, (short)Width, (short)Height);

            // Load project if opened externally
            if (args.Length > 0) Uniray.LoadProject(args[0]);

            // Maximize window
            ClearWindowState(ConfigFlags.UndecoratedWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);

            // Get definitive size
            Width = GetScreenWidth();
            Height = GetScreenHeight();

            // Disable exit key
            SetExitKey(KeyboardKey.Null);
#if !DEBUG
            SetTargetFPS(60);
#endif
            while (!WindowShouldClose())
            {
                if (IsKeyPressed(KeyboardKey.F5))
                {
                    Uniray.StartGameSimulation();
                }

                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.S))
                {
                    Uniray.SaveProject();
                }

                // -----------------------------------------------------------
                // Manage environment updating
                // -----------------------------------------------------------

                if (Hover(Uniray.UI.Components["gameManager"].X + Uniray.UI.Components["gameManager"].Width + 
                    10, 0, Width - Uniray.UI.Components["gameManager"].Width - 20, Height - Uniray.UI.Components["fileManager"].Height - 20))
                {
                    if (IsMouseButtonReleased(MouseButton.Middle))
                    {
                        motion.Mouse = motion.FakePosition;
                        motion.MouseOrigin = Vector2.Zero;
                    }
                    if (IsMouseButtonDown(MouseButton.Middle))
                    {

                        if (IsKeyDown(KeyboardKey.LeftShift))
                        {
                            Vector3 movX = GetCameraRight(ref camera) * GetMouseDelta().X * (motion.Distance / 200);
                            Vector3 movY = GetCameraUp(ref camera) * GetMouseDelta().Y * (motion.Distance / 200);

                            camera.Position -= movX;
                            camera.Target -= movX;

                            camera.Position += movY;
                            camera.Target += movY;
                        }
                        else
                        {
                            if (motion.MouseOrigin == Vector2.Zero) { motion.MouseOrigin = GetMousePosition(); }
                            motion.FakePosition = MoveCamera(motion.Distance, ref camera, camera.Target, motion.YOffset, false, motion.Mouse, motion.MouseOrigin);
                        }
                    }
                    else
                    {
                        motion.Distance -= GetMouseWheelMove() * 2f * Raymath.Vector3Distance(camera.Position, camera.Target) / 10;
                        MoveCamera(motion.Distance, ref camera, camera.Target, motion.YOffset, true, motion.Mouse, motion.MouseOrigin);
                    }
                    Conceptor3D.EnvCamera = camera;
                }
                // -----------------------------------------------------------
                // Manage application overall rendering
                // -----------------------------------------------------------

                BeginDrawing();

                ClearBackground(Uniray.APPLICATION_COLOR);

                if (UData.GameSimulation is not null)
                {
                    BeginTextureMode(GameSimulation.GameSimView);
                    ClearBackground(Uniray.APPLICATION_COLOR);
                    BeginMode3D(UData.CurrentScene.Camera.Camera);
                    DrawSkybox();
                    Uniray.RunGameSimulation();
                    EndMode3D();
                    EndTextureMode();
                }
                else
                {
                    BeginMode3D(camera);
                    DrawSkybox();
                    DrawGrid(10, 10);
                    Uniray.DrawScene();
                    EndMode3D();
                }

                Uniray.DrawUI();

                DrawFPS(40, 200);

                EndDrawing();

            }
            // Close window
            CloseWindow();

            if (UData.CurrentProject is not null)
            {
                // Save project beforehand
                Uniray.SaveProject();

                // Unload all used ressources
                Uniray.Ressource.UnloadRessources();
                // Unload materials
                foreach (GameObject3D go in UData.CurrentScene.GameObjects)
                {
                    if (go is UModel m) RaylibComplements.UnloadUModel(m);
                }
            }
            // Unload shaders
            Uniray.Shaders.UnloadShaders();
        }

        /// <summary>Moves the editor's camera.</summary>
        /// <param name="distance">Distance from the target.</param>
        /// <param name="camera">3D camera of the editor.</param>
        /// <param name="targetPosition">Target of the camera.</param>
        /// <param name="yOffset">Well I don't even remember what this is.</param>
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
        /// <summary>
        /// Switch from . to , for decimal separator when parsing into a file
        /// </summary>
        static void ChangeCultureInfo()
        {
            // Create a clone of the current thread's culture info
            CultureInfo? clone = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
            // Test if the returned clone is null
            if (clone is not null)
            {
                // Set the decimal separator to . instead of ,
                clone.NumberFormat.NumberDecimalSeparator = ".";
                // Set new CultureInfo
                Thread.CurrentThread.CurrentCulture = clone;
                Thread.CurrentThread.CurrentUICulture = clone;
            }
        }
        
        /// <summary>Draws a skybox into a 3D context.</summary>
        static void DrawSkybox()
        {
            // Draw the external skybox 
            Rlgl.DisableBackfaceCulling();
            Rlgl.DisableDepthMask();
            DrawMesh(Skybox, Uniray.Shaders.SkyboxMaterial, Raymath.MatrixIdentity());
            Rlgl.EnableBackfaceCulling();
            Rlgl.EnableDepthMask();
        }
    }
}