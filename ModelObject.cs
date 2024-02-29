using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class ModelObject
    {
        private Model model;

        private string filePath;

        public Model Model { get { return model; } set { model = value; } }
    }
}
