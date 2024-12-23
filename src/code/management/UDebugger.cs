namespace Uniray
{
    /// <summary>Represents the static class of the game debug system.</summary>
    internal static class UDebugger
    {
        /// <summary>Sends a message to the game debugger.</summary>
        /// <param name="msg">Message to send.</param>
        public static void Send(string msg)
        {
            Console.WriteLine($"UNIRAY: {msg}");
        }

        /// <summary>Sends a message to the game debugger.</summary>
        /// <param name="msg">Message to send.</param>
        /// <param name="color">Color to use.</param>
        public static void Send(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"UNIRAY: {msg}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}