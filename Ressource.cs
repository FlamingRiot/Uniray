﻿using Raylib_cs;

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

        public Ressource(List<string> _textures, List<string> _sounds)
        {
            this._textures = new Dictionary<string, Texture2D>();
            this._sounds = new Dictionary<string, Sound>();

            // Load textures
            for (int i = 0; i < _textures.Count; i++)
            {
                this._textures.Add(_textures[i].Split('/').Last().Split('.')[0] ,Raylib.LoadTexture(_textures[i]));
            }
            for (int i = 0; i < _sounds.Count; i++)
            {
                this._sounds.Add(_sounds[i].Split('/').Last().Split('.')[0], Raylib.LoadSound(_sounds[i]));
            }
        }
        /// <summary>
        /// Empty lists constructor
        /// </summary>
        public Ressource()
        {
            this._textures = new Dictionary<string, Texture2D>();
            this._sounds = new Dictionary<string, Sound>();
        }

        public Texture2D GetTexture(string index)
        {
            return this._textures[index];
        }
        public Sound GetSound(string index)
        {
            return this._sounds[index];
        }
        public int GetTextureCount()
        {
            return this._textures.Count;    
        }
        public int GetSoundCount()
        {
            return this._sounds.Count;
        }
        public void AddTexture(Texture2D texture, string key)
        {
            _textures.Add(key, texture);
        }
        public override string ToString()
        {
            return "Les ressources contiennent :\n" + GetTextureCount() + " textures\n" + GetSoundCount() + " sons";
        }
    }
}