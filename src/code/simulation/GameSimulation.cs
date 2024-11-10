using Raylib_cs;
using System.Diagnostics;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="GameSimulation"/>.</summary>
    internal class GameSimulation
    {
        // -----------------------------------------------------------
        // Constants and static instances
        // -----------------------------------------------------------

        public static RenderTexture2D GameSimView;
        public static Rectangle DestinationRectangle;
        public static Rectangle SourceRectangle;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------

        public Stopwatch Timer;

        /// <summary>Creates a <see cref="GameSimulation"/> object.</summary>
        public GameSimulation()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }
    }
}