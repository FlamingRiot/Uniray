using Raylib_cs;

namespace Uniray
{
    public unsafe class Ressource
    {
        /// <summary>
        /// Dictionary containing the currently loaded textures
        /// </summary>
        private Dictionary<string, Texture2D> _textures;
        /// <summary>
        /// Dictionary containing the currently loaded sounds
        /// </summary>
        private Dictionary<string, Sound> _sounds;
        /// <summary>
        /// Dictionary containing the currently loaded models
        /// </summary>
        private Dictionary<string, Model> _models;
        public Ressource(List<UStorage> _textures, List<UStorage> _sounds, List<UStorage> _models)
        {
            this._textures = new Dictionary<string, Texture2D>();
            this._sounds = new Dictionary<string, Sound>();
            this._models = new Dictionary<string, Model>();

            // Load textures
            LoadTextures(_textures);

            // Load sounds
            LoadSounds(_sounds);
            // Load models
            LoadModels(_models);
        }


        /// <summary>
        /// Empty lists constructor
        /// </summary>
        public Ressource()
        {
            this._textures = new Dictionary<string, Texture2D>();
            this._sounds = new Dictionary<string, Sound>();
            this._models = new Dictionary<string, Model>();
        }
        /// <summary>
        /// Load textures from the whole architecture
        /// </summary>
        /// <param name="textures">List of the top directory's storage units</param>
        private void LoadTextures(List<UStorage> textures)
        {
            // Load textures from the whole architecture
            for (int i = 0; i < textures.Count; i++)
            {
                if (textures[i] is UFile) _textures.Add(textures[i].Path.Split('/').Last().Split('.')[0], Raylib.LoadTexture(textures[i].Path));
                else LoadTextures(((UFolder)textures[i]).Files);
            }
        }
        /// <summary>
        /// Load models from the whole architecture
        /// </summary>
        /// <param name="textures">List of the top directory's storage units</param>
        private void LoadModels(List<UStorage> models)
        {
            // Load sounds from the whole architecture
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i] is UFile)
                {
                    // Load the model and fix the material issue
                    Model m = Raylib.LoadModel(models[i].Path);
                    for (int j = 0; j < m.Meshes[0].VertexCount * 4; j++)
                        m.Meshes[0].Colors[j] = 255;
                    Raylib.UpdateMeshBuffer(m.Meshes[0], 3, m.Meshes[0].Colors, m.Meshes[0].VertexCount * 4, 0);

                    // Add the model
                    _models.Add(models[i].Path.Split('/').Last().Split('.')[0], m);
                }
                else LoadModels(((UFolder)models[i]).Files);
            }
        }
        /// <summary>
        /// Load sounds from the whole architecture
        /// </summary>
        /// <param name="sounds">List of the top directory's storage units</param>
        private void LoadSounds(List<UStorage> sounds) 
        {
            // Load sounds from the whole architecture
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i] is UFile) _sounds.Add(sounds[i].Path.Split('/').Last().Split('.')[0], Raylib.LoadSound(sounds[i].Path));
                else LoadSounds(((UFolder)sounds[i]).Files);
            }
        }
        /// <summary>
        /// Unload all the loaded ressources in RAM
        /// </summary>
        public void UnloadRessources()
        {
            foreach (Model m in this._models.Values)
            {
                Raylib.UnloadModel(m);
            }
            foreach (Texture2D t in this._textures.Values)
            {
                Raylib.UnloadTexture(t);
            }
            foreach (Sound s in this._sounds.Values)
            {
                Raylib.UnloadSound(s);
            }
        }
        public Texture2D GetTexture(string index)
        {
            return this._textures[index];
        }
        public Sound GetSound(string index)
        {
            return this._sounds[index];
        }
        public Model GetModel(string index)
        {
            return this._models[index];
        }
        public int GetTextureCount()
        {
            return this._textures.Count;    
        }
        public int GetSoundCount()
        {
            return this._sounds.Count;
        }
        public int GetModelCount()
        {
            return this._models.Count;
        }
        public void AddTexture(Texture2D texture, string key)
        {
            if (key == "Image_0") { }
            _textures.Add(key, texture);
        }
        public void AddModel(Model model, string key)
        {
            _models.Add(key, model);
        }
        public bool TextureExists(string key)
        {
            return _textures.ContainsKey(key);
        }
        public bool SoundExists(string key)
        {
            return _sounds.ContainsKey(key);
        }
        public bool ModelExists(string key)
        {
            return _models.ContainsKey(key);
        }
        /// <summary>
        /// Remove texture from the list and unload from the RAM
        /// </summary>
        /// <param name="key">Key</param>
        public void DeleteTexture(string key)
        {
            Raylib.UnloadTexture(_textures[key]);
            _textures.Remove(key);
        }
        /// <summary>
        /// Remove model from the list and unload from the RAM
        /// </summary>
        /// <param name="key">Key</param>
        public void DeleteModel(string key)
        {
            Raylib.UnloadModel(_models[key]);
            _models.Remove(key);
        }
        /// <summary>
        /// Remove sound from the list and unload from the RAM
        /// </summary>
        /// <param name="key">Key</param>
        public void DeleteSound(string key)
        {
            Raylib.UnloadSound(_sounds[key]);
            _sounds.Remove(key);
        }
        public override string ToString()
        {
            return "Les ressources contiennent :\n" + GetTextureCount() + " textures\n" + GetSoundCount() + " sons\n" + GetModelCount() + " modèles";
        }
    }
}