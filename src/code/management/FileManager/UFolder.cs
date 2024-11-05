using System.Diagnostics;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UFolder"/>.</summary>
    public class UFolder : UStorage, IRenamable
    {
        /// <summary>Containes files and folders inside of the current one.</summary>
        public List<UStorage> Files;

        /// <summary>Upstream folder of the current instance.</summary>
        public UFolder? UpstreamFolder;

        /// <summary>Creates an empty instance of <see cref="UFolder"/>.</summary>
        public UFolder()
        {
            Name = "";
            Files = new List<UStorage>();
        }

        /// <summary>Creates an instance of <see cref="UFolder"/>.</summary>
        /// <param name="path">The absolute path to the folder</param>
        public UFolder(string path, List<UStorage> files) : base(path)
        {
            Name = path.Split('/').Last();
            Files = files;
        }

        /// <summary>Adds a file to the folder.</summary>
        /// <param name="file">File to add.</param>
        public void AddFile(UStorage file)
        {
            Files.Add(file);
        }

        /// <summary>Deletes a file from the folder.</summary>
        /// <param name="file">File to delete.</param>
        public void DeleteFile(UStorage file)
        {
            Files.Remove(file);
        }

        /// <summary>Renames the folder.</summary>
        /// <param name="name">The new name to give</param>
        public void Rename(string name)
        {
            // Check if text null
            if (name != "")
            {
                string oldPath = Path;
                Path = Path.Replace($"{this.Name}", $"{name}");
                Name = name;

                // Rename physical folder
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.FileName = "cmd.exe";
                processInfo.Arguments = $"/C ren \"{oldPath}\" \"{name}\"";
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;

                Process.Start(processInfo);

                // Update folder downstream tree
                FileManager.UpdateFileTree(this);

            }
        }
    }
}