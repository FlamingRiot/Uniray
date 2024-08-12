using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Uniray
{
    public class UShaders
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
        /// Outline shader used for rendering an outline around the currently selected GameObjects
        /// </summary>
        public Shader OutlineShader { get { return outlineShader; } set { outlineShader = value; } }
        /// <summary>
        /// The used material to render the outline shader
        /// </summary>
        public Material OutlineMaterial { get { return outlineMaterial; } set { outlineMaterial = value; } }
        /// <summary>
        /// UShaders constructor
        /// </summary>
        public UShaders()
        {
            // Outline shader
            outlineShader = LoadShader("data/shaders/outline.vs", "data/shaders/outline.fs");
            outlineMaterial = LoadMaterialDefault();
            outlineMaterial.Shader = outlineShader;
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
    }
}