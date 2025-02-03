using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Uniray
{
    internal static class HardRessource
    {
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<ShapeType, Mesh> ShapeMeshes = new Dictionary<ShapeType, Mesh>();

        public static void Init()
        {
            // Load static textures
            Textures = new Dictionary<string, Texture2D>() 
            {
                { "file", LoadTexture("data/img/file.png")},
                { "model_file", LoadTexture("data/img/model_file.png")},
                { "folder", LoadTexture("data/img/folder.png")},
                { "camera", LoadTexture("data/img/camera.png")},
                { "cs_script", LoadTexture("data/img/cs_script.png")},
            };

            // Load UShape Meshes
            ShapeMeshes = new Dictionary<ShapeType, Mesh>()
            {
                { ShapeType.Cube, GenMeshCube(1, 1, 1) },
                { ShapeType.Sphere, GenMeshSphere(0.5f, 20, 20) },
                { ShapeType.Cylinder, GenMeshCylinder(0.5f, 1, 20) },
                { ShapeType.Cone, GenMeshCone(0.5f, 1, 20) },
                { ShapeType.Torus, GenMeshTorus(0.5f, 1, 20, 20) },
                { ShapeType.HemiSphere, GenMeshHemiSphere(0.5f, 15, 15) },
                { ShapeType.Knot, GenMeshKnot(0.5f, 1, 20, 20) },
                { ShapeType.HollowCylinder, GenMeshTorus(0.5f, 1, 4, 20) },
            };
        }

        /// <summary>Unloads Hard Resources from RAM/VRAM</summary>
        public static void Unload()
        {
            ShapeMeshes.ToList().ForEach(x => UnloadMesh(x.Value)); // Unload Shape Meshes
            Textures.ToList().ForEach(x => UnloadTexture(x.Value)); // Unload textures
        }
    }
}