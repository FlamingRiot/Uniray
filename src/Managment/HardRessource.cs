using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Uniray
{
    internal static class HardRessource
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public static void Init()
        {
            Textures = new Dictionary<string, Texture2D>() 
            {
                { "file", LoadTexture("data/img/file.png")},
                { "model_file", LoadTexture("data/img/model_file.png")},
                { "folder", LoadTexture("data/img/folder.png")}
            };
        }
    }
}