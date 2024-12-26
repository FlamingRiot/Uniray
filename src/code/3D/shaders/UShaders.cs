using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

namespace Uniray
{
    public unsafe class UShaders
    {
        /// <summary>Skybox rendering shader.</summary>
        public Shader SkyboxShader;
        /// <summary>Skybox loading shader.</summary>
        public Shader CubemapShader;
        /// <summary>Basic lighting shader.</summary>
        public Shader LightingShader;
        /// <summary>Game Object outline shader.</summary>
        public Shader OutlineShader;

        // -----------------------------------------------------------
        // Shader associated materials
        // -----------------------------------------------------------
        public Material SkyboxMaterial;
        public Material OutlineMaterial;

        // -----------------------------------------------------------
        // Shader uniforms locations
        // -----------------------------------------------------------
        private readonly int _viewPosLoc;

        /// <summary>Creates an instance of Uniray Shaders</summary>
        public UShaders()
        {
            // Skybox shader
            SkyboxShader = LoadShader("data/shaders/skybox.vs", "data/shaders/skybox.fs");
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "environmentMap"), (int)MaterialMapIndex.Cubemap, ShaderUniformDataType.Int);
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "doGamma"), 1, ShaderUniformDataType.Int);
            SetShaderValue(SkyboxShader, GetShaderLocation(SkyboxShader, "vflipped"), 1, ShaderUniformDataType.Int);
            SkyboxMaterial = LoadMaterialDefault();
            SkyboxMaterial.Shader = SkyboxShader;
            // Cubemap shader
            CubemapShader = LoadShader("data/shaders/cubemap.vs", "data/shaders/cubemap.fs");
            SetShaderValue(CubemapShader, GetShaderLocation(CubemapShader, "equirectangularMap"), 0, ShaderUniformDataType.Int);
            // Load lighting shader
            LightingShader = LoadShader("data/shaders/lighting.vs", "data/shaders/lighting.fs");
            _viewPosLoc = GetShaderLocation(LightingShader, "viewPos");
            // Load outline shader
            OutlineShader = LoadShader("data/shaders/outline.vs", "data/shaders/outline.fs");
            OutlineMaterial = LoadMaterialDefault();
            OutlineMaterial.Shader = OutlineShader;
        }

        /// <summary>Updates the shader values.</summary>
        /// <param name="viewPos">Camera position (view camera).</param>
        public void Update(Vector3 viewPos)
        {
            SetShaderValue(LightingShader, _viewPosLoc, viewPos, ShaderUniformDataType.Vec3);
        }

        /// <summary>Unloads shaders and materials</summary>
        public void UnloadShaders()
        {
            // Unload shaders
            UnloadShader(SkyboxShader);
            UnloadShader(CubemapShader);

            // Unload materials
            UnloadMaterial(SkyboxMaterial);
        }

        /// <summary>Generate skybox cubemap using the loading shader.</summary>
        /// <param name="panorama">HDR Texture to use./param>
        /// <param name="size">FrameBuffer size.</param>
        /// <param name="format">Format of the cubemap.</param>
        /// <returns>The loaded cubemap.</returns>
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
            Rlgl.EnableShader(CubemapShader.Id);

            // Define projection matrix and send it to the shader
            Matrix4x4 matFboProjection = Raymath.MatrixPerspective(90.0f * DEG2RAD, 1.0f, Rlgl.CULL_DISTANCE_NEAR, Rlgl.CULL_DISTANCE_FAR);
            Rlgl.SetUniformMatrix(CubemapShader.Locs[(int)ShaderLocationIndex.MatrixProjection], matFboProjection);

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
                Rlgl.SetUniformMatrix(CubemapShader.Locs[(int)ShaderLocationIndex.MatrixView], fboViews[i]);

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

        /// <summary>Sets the cubemap texture.</summary>
        /// <param name="tex">¨Texture to set.</param>
        public void SetCubemap(Texture2D tex)
        {
            SetMaterialTexture(ref SkyboxMaterial, MaterialMapIndex.Cubemap, tex);
        }
    }
}