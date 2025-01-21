using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Uniray
{
    /// <summary>Defines a direction in 3D world.</summary>
    public enum EnvDirection
    {
        East,
        West,
        North,
        South
    }

    /// <summary>Represents a <see cref="Skybox"/> world object.</summary>
    public struct Skybox
    {
        public const string DEFAULT_CUBEMAP_TEXTURE = "data/shaders/skyboxes/default.hdr";

        public string KeyID { get
            {
                string rep = DEFAULT_CUBEMAP_TEXTURE.Replace('\\', '/');
                return rep.Split('/').Last().Split('.')[0];
            } 
        }

        /// <summary>Defines the default mesh used for skybox rendering.</summary>
        public static readonly Mesh Mesh = GenMeshCube(1, 1, 1);

        /// <summary>Cubemap texture of the skybox object.</summary>
        public Material CubemapMaterial;

        /// <summary>Defines the direction the skybox is facing.</summary>
        public EnvDirection Direction;

        /// <summary>Creates an empty <see cref="Skybox"/> instance.</summary>
        public Skybox() { }
    }
}