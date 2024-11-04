using System.Numerics;
using Raylib_cs;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="ErrorHandler"/>.</summary>
    public static class ErrorHandler
    {
        private static List<Error> _errors = new List<Error>();
        private static Font _font;
        private static Vector2 _position;

        /// <summary>Initializes the inner variables of the error handler.</summary>
        /// <param name="font">Font of the error handler.</param>
        /// <param name="position">General position of the errors.</param>
        public static void Init(Font font, Vector2 position)
        {
            _font = font;
            _position = position;
        }

        /// <summary>Sends an error to the handler.</summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public static void Send(Error error)
        {
            error.Position = _position;
            error.Size = new Vector2(350, 40);
            _errors.Add(error);
        }

        /// <summary>Updates the error handler and its error timers.</summary>
        public static void Tick()
        {
            List<int> outDated = new List<int>();
            foreach (Error error in _errors)
            {
                error.Y = _position.Y - (error.Height + 10) * _errors.IndexOf(error);
                // Draw error label
                Raylib.DrawRectangleGradientH((int)error.Position.X, (int)error.Position.Y, (int)error.Size.X, (int)error.Size.Y, new Color(130, 12, 3, 255), new Color(201, 27, 14, 255));
                Raylib.DrawTextPro(_font, "U" + error.Code + ": " + error.Message, new Vector2(error.Position.X + error.Size.X / 2 - error.Message.Length * 4, error.Position.Y + error.Size.Y / 3), Vector2.Zero, 0, 1, 1, Color.White);
                // Check displayed time
                if ((Raylib.GetTime() - error.Time) > 5)
                {
                    outDated.Add(_errors.IndexOf(error));
                }
            }
            foreach (int i in outDated) { _errors.Remove(_errors.ElementAt(i)); }
        }
    }
}