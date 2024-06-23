using System.Numerics;
using Raylib_cs;

namespace Uniray
{
    public class ErrorHandler
    {
        /// <summary>
        /// List of errors
        /// </summary>
        private List<Error> errors;
        /// <summary>
        /// Font
        /// </summary>
        private Font font;
        /// <summary>
        /// Positon of the error handler
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// ErrorHandler Constructor
        /// </summary>
        public ErrorHandler(Vector2 position, Font font)
        {
            errors = new List<Error>();
            this.position = position;
            this.font = font;
        }
        /// <summary>
        /// Send an error to the error stack
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public void Send(Error error)
        {
            error.Position = position;
            error.Size = new Vector2(350, 40);
            errors.Add(error);
        }
        /// <summary>
        /// Run time verification for the displayed errors
        /// </summary>
        public void Tick()
        {
            List<int> outDated = new List<int>();
            foreach (Error error in errors)
            {
                font.BaseSize = 2;
                error.Y = position.Y - (error.Height + 10) * errors.IndexOf(error);
                // Draw error label
                Raylib.DrawRectangleGradientH((int)error.Position.X, (int)error.Position.Y, (int)error.Size.X, (int)error.Size.Y, new Color(130, 12, 3, 255), new Color(201, 27, 14, 255));
                Raylib.DrawTextPro(font, "U" + error.Code + ": " + error.Message, new Vector2(error.Position.X + error.Size.X / 2 - error.Message.Length * 4, error.Position.Y + error.Size.Y / 3), Vector2.Zero, 0, 1, 1, Color.White);
                // Check displayed time
                if ((Raylib.GetTime() - error.Time) > 5)
                {
                    outDated.Add(errors.IndexOf(error));
                }
            }
            foreach (int i in outDated) { errors.Remove(errors.ElementAt(i)); }
        }
        /// <summary>
        /// Send ErrorHandler informations
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "The error stack contains " + errors.Count + " errors";
        }
    }
}