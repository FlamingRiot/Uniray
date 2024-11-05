using Raylib_cs;

namespace Uniray
{
    /// <summary>Represents a <see cref="UFile"/> instance.</summary>
    public class UFile : UStorage, IRenamable
    {
        /// <summary>Extension of the file.</summary>
        public string Extension;

        /// <summary>Returns the full name (including extension of the file.</summary>
        public string FullName { get { return Name + '.' + Extension; } }

        /// <summary>Creates an instance of a <see cref="UFile"/> object.</summary>
        /// <param name="path">The absolute path to the file.</param>
        public UFile(string path) : base(path)
        {
            string[] slashed = path.Split('/');
            Name = slashed.Last().Split('.').First();
            Extension = path.Split('.').Last();
        }

        /// <summary>Renames the file both virtually and effectively.</summary>
        /// <param name="name">The new name of the file</param>
        public void Rename(string name)
        {
            // If text null, keep old name and path
            if (name != "")
            {
                string oldKey = Name;
                string oldPath = Path;
                Path = Path.Replace($"{Name}", $"{name}");
                Name = name;
                File.Move(oldPath, Path);

                // Update model and texture keys
                switch (Extension)
                {
                    case "m3d":
                        // Loop over every scene
                        UData.CurrentProject?.Scenes.ForEach(scene =>
                        {
                            // Get objects with corresponding keys
                            scene.GameObjects.Where(obj => obj is UModel).Where(obj => ((UModel)obj).ModelID == oldKey).ToList().ForEach(obj =>
                            {
                                // Replace with new key/name
                                ((UModel)obj).ModelID = Name;
                            });
                        });
                        break;
                    case "png":
                        // Loop over every scene
                        UData.CurrentProject?.Scenes.ForEach(scene =>
                        {
                            // Get objects with corresponding keys
                            scene.GameObjects.Where(obj => obj is UModel).Where(obj => ((UModel)obj).TextureID == oldKey).ToList().ForEach(obj =>
                            {
                                // Replace with new key/name
                                ((UModel)obj).TextureID = Name;
                            });
                        });
                        break;
                }
            }
        }
    }
}