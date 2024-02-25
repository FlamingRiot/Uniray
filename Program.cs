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

            int wWindow = GetScreenWidth();
            int hWindow = GetScreenHeight();

            // Set 3D camera for the scene
            Camera3D camera = new Camera3D();
            camera.Projection = CameraProjection.Perspective;
            camera.Position = Vector3.UnitY * 5;
            camera.Target = Vector3.UnitX;
            camera.Up = Vector3.UnitY;
            camera.FovY = 90;

            // ===============================================================================================================================================
            // =============================================================== SET APPLICATION UI ============================================================
            // ===============================================================================================================================================

            Font baseFont = LoadFont("data/font/Ubuntu-Regular.ttf");

            float cont1X = wWindow - wWindow / 1.25f;
            float cont1Y = hWindow - hWindow / 3;
            Container fileManager = new Container((int)cont1X, (int)cont1Y, wWindow - (int)cont1X, hWindow - (int)cont1Y, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            fileManager.Type = ContainerType.FileDropper;
            fileManager.OutputFilePath = "assets/models/";
            fileManager.ExtensionFile = "m3d";

            Container gameManager = new Container(10, 30, (int)cont1X - 20, hWindow - 10, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);

            Textbox textbox = new Textbox("Hello World !", 140, 200, 50, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            Button button = new Button("Evan", 1200, 400, 60, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            button.Type = ButtonType.PathFinder;

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
                DrawRectangle(0, 0, (int)cont1X, hWindow, Uniray.APPLICATION_COLOR);
                DrawRectangle(0, (int)cont1Y - 10, wWindow, hWindow - (int)cont1Y + 10, Uniray.APPLICATION_COLOR);
                DrawContainer(ref fileManager);
                DrawContainer(ref gameManager);
                DrawTextbox(ref textbox, baseFont);
                DrawButton(button, baseFont);

                EndDrawing();
            }
            CloseWindow();
        }
    }
}