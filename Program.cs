using static Raylib_cs.Raylib;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Initialize window and set mode
            InitWindow(1800, 900, "Uniray");
            SetWindowState(ConfigFlags.ResizableWindow);
            SetWindowState(ConfigFlags.MaximizedWindow);
            InitGUI(Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            

            // Load font
            Font font = LoadFont("data/font/Ubuntu-Regular.ttf");

            // Get Window size
            int wWindow = GetScreenWidth();
            int hWindow = GetScreenHeight();

            // Set UI
            Uniray uniray = new Uniray(wWindow, hWindow, font);

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
                // Manage resize options
                if (IsWindowResized())
                {
                    hWindow = GetScreenHeight();
                    wWindow = GetScreenWidth();
                    uniray = new Uniray(wWindow, hWindow, font);
                }

                BeginDrawing();

                ClearBackground(Color.White);

                BeginMode3D(camera);

                DrawGrid(10, 10);

                EndMode3D();

                uniray.DrawUI();

                EndDrawing();
            }
            CloseWindow();
        }
    }
}