using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Uniray
{
    internal static class GameDebugger
    {
        // -----------------------------------------------------------
        // Constants and static instances
        // -----------------------------------------------------------

        public const int FontSize = 12;

        private static Dictionary<string, Color> _logs = new Dictionary<string, Color>();
        private static Font _font;
        private static float _textHeight;
        private static Vector2 _position;

        /// <summary>Inits the game debugger.</summary>
        /// <param name="font">Default font of the debugger.</param>
        public static void Init(Font font, Vector2 position)
        {
            _font = font;
            _position = position; // Define debugger position
            _textHeight = MeasureTextEx(_font, "R", FontSize, 1).Y; // Measure text height
            ULog($"Game {UData.CurrentProject?.Name} launched");
            ULog($"Started: {UData.GameSimulation?.Timer.Elapsed.ToString()}");
        }

        /// <summary>Closes and clear the game debugger.</summary>
        public static void Close()
        {
            _logs.Clear();
        }

        /// <summary>Updates game debugger.</summary>
        public static void Update()
        {
            DrawTextPro(_font, UData.GameSimulation?.Timer.Elapsed.ToString(), _position, new Vector2(-10, -10), 0, FontSize, 1, Color.Green);
            for (int i = 0; i < _logs.Count; i++)
            {
                DrawTextPro(_font, "INFO: " + _logs.ToList()[i].Key, _position, new Vector2(-10, -25 - _textHeight * i), 0, FontSize, 1, _logs.ToList()[i].Value);
            }
        }

        /// <summary>Adds a custom log to the debugger.</summary>
        /// <param name="msg">Log message.</param>
        public static void ULog(string msg)
        {
            _logs.Add(msg, Color.White);
        }

        /// <summary>Adds a custom log to the debugger.</summary>
        /// <param name="msg">Log message.</param>
        /// <param name="color">Log color.</param>
        public static void ULog(string msg, Color color)
        {
            _logs.Add(msg, color);
        }
    }
}