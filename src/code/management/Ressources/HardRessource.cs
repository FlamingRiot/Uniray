using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Uniray
{
    internal static class HardRessource
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Model> Models = new Dictionary<string, Model>();
        public static Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        public static void Init()
        {
            // Load static textures
            Textures = new Dictionary<string, Texture2D>() 
            {
                { "file", LoadTexture("data/img/file.png")},
                { "model_file", LoadTexture("data/img/model_file.png")},
                { "folder", LoadTexture("data/img/folder.png")}
            };
            // Load static models
            Models = new Dictionary<string, Model>()
            {
                { "camera", LoadModel("data/camera.m3d")}
            };
            // Load static materials
            Materials = new Dictionary<string, Material>();
            // Camera material
            Material camMat = LoadMaterialDefault();
            SetMaterialTexture(ref camMat, MaterialMapIndex.Albedo, LoadTexture("data/cameraTex.png"));
            Materials.Add("camera", camMat);
        }
    }
}