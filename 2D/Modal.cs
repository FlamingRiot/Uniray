using RayGUI_cs;

namespace Uniray
{
    public class Modal
    {
        /// <summary>
        /// Every component inside of the modal window
        /// </summary>
        public Dictionary<string, Component> Components;
        /// <summary>
        /// Modal constructor
        /// </summary>
        /// <param name="components">Every component inside of the modal window</param>
        public Modal(Dictionary<string, Component> components)
        {
            Components = components;
        }
    }
}