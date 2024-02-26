using static Raylib_cs.Raylib;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using RayGUI_cs;
using System.Numerics;

namespace Uniray
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Initialize window and set mode
            InitWindow(1800, 900, "Uniray");
            InitGUI(Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);

            int wWindow = GetScreenWidth();
            int hWindow = GetScreenHeight();

            Uniray uniray = new Uniray(wWindow, hWindow);

            // Set 3D camera for the scene
            Camera3D camera = new Camera3D();
            camera.Projection = CameraProjection.Perspective;
            camera.Position = Vector3.UnitY * 5;
            camera.Target = Vector3.UnitX;
            camera.Up = Vector3.UnitY;
            camera.FovY = 90;

            // Set FPS
            SetTargetFPS(60);

            /*List<int> keys = new List<int>();
            int index = 0;
            int key;*/

            while (!WindowShouldClose())
            {
                /*key = GetKeyPressed();
                if (key != 0)keys.Add(key);
                if (keys.Count != 0)Console.WriteLine(keys.Last());*/


                BeginDrawing();

                ClearBackground(Color.White);

                // ===============================================================================================================================================
                // =============================================================== MANAGE 3D SPACE ===============================================================
                // ===============================================================================================================================================
                BeginMode3D(camera);

                DrawGrid(10, 10);

                EndMode3D();
                // ===============================================================================================================================================
                // ============================================================= MANAGE APPLICATION ==============================================================
                // ===============================================================================================================================================
                uniray.DrawUI();

                EndDrawing();
            }
            CloseWindow();
        }
    }
}