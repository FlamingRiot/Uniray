using Raylib_cs;

namespace Uniray
{
    public class Ressource
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
        public Ressource(List<string> _textures, List<string> _sounds, List<string> _models)
        {
            this._textures = new Dictionary<string, Texture2D>();
            this._sounds = new Dictionary<string, Sound>();
            this._models = new Dictionary<string, Model>();

            // Load textures
            for (int i = 0; i < _textures.Count; i++)
            {
                this._textures.Add(_textures[i].Split('/').Last().Split('.')[0] ,Raylib.LoadTexture(_textures[i]));
            }
            // Load sounds
            for (int i = 0; i < _sounds.Count; i++)
            {
                this._sounds.Add(_sounds[i].Split('/').Last().Split('.')[0], Raylib.LoadSound(_sounds[i]));
            }
            // Load models
            for (int i = 0; i < _models.Count; i++)
            {
                this._models.Add(_models[i].Split('/').Last().Split('.')[0], Raylib.LoadModel(_models[i]));
            }
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
        public override string ToString()
        {
            return "Les ressources contiennent :\n" + GetTextureCount() + " textures\n" + GetSoundCount() + " sons\n" + GetModelCount() + " modèles";
        }
    }
}