using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Uniray
{
    public unsafe class UShaders
    {
        /// <summary>
        /// Outline shader used for rendering an outline around the currently selected GameObjects
        /// </summary>
        private Shader outlineShader;
        /// <summary>
        /// The used material to render the outline shader
        /// </summary>
        private Material outlineMaterial;
        /// <summary>
        /// Skybox rendering shader
        /// </summary>
        private Shader skyboxShader;
        /// <summary>
        /// Skybox material
        /// </summary>
        private Material skyboxMaterial;
        /// <summary>
        /// Cubemap loading shader
        /// </summary>
        private Shader cubemapShader;
        /// <summary>
        /// Outline shader used for rendering an outline around the currently selected GameObjects
        /// </summary>
        public Shader OutlineShader { get { return outlineShader; } set { outlineShader = value; } }
        /// <summary>
        /// Skybox rendering shader
        /// </summary>
        public Shader SkyboxShader { get { return skyboxShader; } set { skyboxShader = value; } }
        /// <summary>
        /// Skybox loading shader
        /// </summary>
        public Shader CubemapShader { get { return cubemapShader; } set { cubemapShader = value; } }
        /// <summary>
        /// The used material to render the outline shader
        /// </summary>
        public Material OutlineMaterial { get { return outlineMaterial; } set { outlineMaterial = value; } }
        public Material SkyboxMaterial { get { return skyboxMaterial; } set { skyboxMaterial = value; } }
        /// <summary>
        /// UShaders constructor
        /// </summary>
        public UShaders()
        {
            // Outline shader
            outlineShader = LoadShader("data/shaders/outline.vs", "data/shaders/outline.fs");
            outlineMaterial = LoadMaterialDefault();
            outlineMaterial.Shader = outlineShader;
            // Skybox shader
            skyboxShader = LoadShader("data/shaders/skybox.vs", "data/shaders/skybox.fs");
            SetShaderValue(skyboxShader, GetShaderLocation(skyboxShader, "environmentMap"), (int)MaterialMapIndex.Cubemap, ShaderUniformDataType.Int);
            SetShaderValue(skyboxShader, GetShaderLocation(skyboxShader, "doGamma"), 1, ShaderUniformDataType.Int);
            SetShaderValue(skyboxShader, GetShaderLocation(skyboxShader, "vflipped"), 1, ShaderUniformDataType.Int);
            skyboxMaterial = LoadMaterialDefault();
            skyboxMaterial.Shader = skyboxShader;
            // Cubemap shader
            cubemapShader = LoadShader("data/shaders/cubemap.vs", "data/shaders/cubemap.fs");
            SetShaderValue(cubemapShader, GetShaderLocation(cubemapShader, "equirectangularMap"), 0, ShaderUniformDataType.Int);
        }
        /// <summary>
        /// Unload shaders and materials
        /// </summary>
        public void UnloadShaders()
        {
            // Unload shaders
            UnloadShader(outlineShader);

            // Unload materials
            UnloadMaterial(outlineMaterial);
        }
        /// <summary>
        /// Generate Cubemap according to the passed HDR texture
        /// </summary>
        /// <param name="panorama">Texture to use/param>
        /// <param name="size">FrameBuffer size</param>
        /// <param name="format">Format of the cubemap</param>
        /// <returns></returns>
        public Texture2D GenTexureCubemap(Texture2D panorama, int size, PixelFormat format)
        {
            Texture2D cubemap;

            // Disable Backface culling to render inside the cube
            Rlgl.DisableBackfaceCulling();

            // Setup frame buffer
            uint rbo = Rlgl.LoadTextureDepth(size, size, true);
            cubemap.Id = Rlgl.LoadTextureCubemap(null, size, format);

            uint fbo = Rlgl.LoadFramebuffer(size, size);
            Rlgl.FramebufferAttach(fbo, rbo, FramebufferAttachType.Depth, FramebufferAttachTextureType.Renderbuffer, 0);
            Rlgl.FramebufferAttach(fbo, cubemap.Id, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.CubemapPositiveX, 0);

            // Check if framebuffer is valid

            if (Rlgl.FramebufferComplete(fbo))
            {
                Console.WriteLine($"FBO: [ID {fbo}] Framebuffer object created successfully");
            }

            // Draw to framebuffer
            Rlgl.EnableShader(cubemapShader.Id);

            // Define projection matrix and send it to the shader
            Matrix4x4 matFboProjection = Raymath.MatrixPerspective(90.0f * DEG2RAD, 1.0f, Rlgl.CULL_DISTANCE_NEAR, Rlgl.CULL_DISTANCE_FAR);
            Rlgl.SetUniformMatrix(cubemapShader.Locs[(int)ShaderLocationIndex.MatrixProjection], matFboProjection);

            // Define view matrix for every side of the cube
            Matrix4x4[] fboViews = new Matrix4x4[]
            {
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3(-1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 1.0f,  0.0f,  0.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  1.0f,  0.0f), new Vector3( 0.0f,  0.0f, -1.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f, -1.0f,  0.0f), new Vector3( 0.0f,  0.0f, 1.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f, -1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
                Raymath.MatrixLookAt(Vector3.Zero, new Vector3( 0.0f,  0.0f,  1.0f), new Vector3( 0.0f, -1.0f,  0.0f)),
            };

            // Set viewport to current fbo dimensions
            Rlgl.Viewport(0, 0, size, size);

            // Activate and enable texture for drawing to cubemap faces
            Rlgl.ActiveTextureSlot(0);
            Rlgl.EnableTexture(panorama.Id);

            for (int i = 0; i < 6; i++)
            {
                // Set the view matrix for current face
                Rlgl.SetUniformMatrix(cubemapShader.Locs[(int)ShaderLocationIndex.MatrixView], fboViews[i]);

                // Select the current cubemap face attachment for the fbo
                Rlgl.FramebufferAttach(fbo, cubemap.Id, FramebufferAttachType.ColorChannel0, FramebufferAttachTextureType.CubemapPositiveX + i, 0);
                Rlgl.EnableFramebuffer(fbo);

                Rlgl.ClearScreenBuffers();
                Rlgl.LoadDrawCube();
            }

            // Unload framebuffer and reset state
            Rlgl.DisableShader();
            Rlgl.DisableTexture();
            Rlgl.DisableFramebuffer();

            Rlgl.UnloadFramebuffer(fbo);

            Rlgl.Viewport(0, 0, Rlgl.GetFramebufferWidth(), Rlgl.GetFramebufferHeight());
            Rlgl.EnableBackfaceCulling();

            cubemap.Width = size;
            cubemap.Height = size;
            cubemap.Mipmaps = 1;
            cubemap.Format = format;

            return cubemap;
        }
        /// <summary>
        /// Set the skybox material texture
        /// </summary>
        /// <param name="tex"></param>
        public void SetCubemap(Texture2D tex)
        {
            SetMaterialTexture(ref skyboxMaterial, MaterialMapIndex.Cubemap, tex);
        }
    }
}