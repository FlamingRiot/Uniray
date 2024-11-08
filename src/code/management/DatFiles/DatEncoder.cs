using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Uniray.DatFiles
{
    internal class DatEncoder
    {
        // -----------------------------------------------------------
        // Private and constant instances
        // -----------------------------------------------------------

        private const int AES_KEY_LENGTH = 32;
        private const int AES_IV_LENGTH = 16;

        private const string MODELS_SECTION = "MODEL";
        private const string TEXTURES_SECTION = "TEXTURE";
        private const string SOUNDS_SECTION = "SOUND";

        private static int _offset = 0;
        private static List<DatFileEntry> _entries = new List<DatFileEntry>();

        /// <summary>Encodes a scene's information and stores it into a .DAT file.</summary>
        /// <param name="scene">Scene to read data from.</param>
        public static void EncodeScene(Scene scene)
        {
            if (UData.CurrentProject is not null)
            {
                // Transform information to JSON object
                string[] jsons = JsonfyGos(scene.GameObjects);
                // Open file stream
                FileStream stream = new FileStream(UData.CurrentProject.ProjectFolder + $"/scenes/{scene.Name}.DAT", FileMode.Create);
                using BinaryWriter writer = new BinaryWriter(stream);
                // Write placeholder data to the beginning of the file
                writer.Write(scene.GameObjects.Count); // Object count placeholder (Int-32)
                writer.Write(_offset); // Table placeholder (Int-32)
                // Move writer after placeholder
                _offset += 8; // Int-32 + Int-32 = 8 bytes
                // Start AES algorithm
                using Aes aes = Aes.Create();
                writer.Write(aes.Key); // 32 Bytes
                writer.Write(aes.IV); // 16 Bytes
                _offset += 48; // 16 + 32 = 48 Bytes
                               // Encode and write JSON data   
                for (int i = 0; i < jsons.Length; i++)
                {
                    // Define game objects section
                    string name = "";
                    switch (i)
                    {
                        case 0: // UModel section
                            name = MODELS_SECTION;
                            break;
                        case 1: // UCamera section
                            name = TEXTURES_SECTION;
                            break;
                    }

                    // Encrypt string to byte array
                    byte[] data = Encrypt(jsons[i], aes.Key, aes.IV);
                    //string text = Decrypt(data, aes.Key, aes.IV);
                    // Create DAT file entry
                    DatFileEntry entry = new DatFileEntry(name, _offset, data.Length);
                    _entries.Add(entry);
                    // Write data to file
                    writer.Write(data);
                    _offset += data.Length;
                }
                // Write entries table
                int tableOffset = _offset;
                foreach (DatFileEntry entry in _entries)
                {
                    writer.Write(entry.Name);
                    writer.Write(entry.Index);
                    writer.Write(entry.Size);
                }
                // Go back to the beginning of the file to write important data
                stream.Seek(0, SeekOrigin.Begin);
                writer.Write(_entries.Count);
                writer.Write(tableOffset);
                /* Which gives us a similar file structure : 
                    * 
                    * Entries count
                    * Table offset
                    * Files data at specific location
                    * Entries informations (Name, size, index), aka information table
                    * */

                // Reset internal data
                _offset = 0;
                _entries.Clear();
            }
        }

        /// <summary>Encrypts data with the AES algorithm.</summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="iv">Encryption IV.</param>
        /// <returns>Encrypted data.</returns>
        /// <exception cref="ArgumentNullException">Null data exception.</exception>
        private static byte[] Encrypt(string data, byte[] key, byte[] iv)
        {
            // Null exceptions
            if (data == null || data.Length <= 0) 
                throw new ArgumentNullException(nameof(data)); 
            if (key == null || key.Length <= 0) 
                throw new ArgumentNullException(nameof(key)); 
            if (iv == null || iv.Length <= 0) 
                throw new ArgumentNullException(nameof(iv));

            // Encrypt data
            byte[] encrypted;
            // Start Aes algorithm
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            // Create encryptor based on the key and IV
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream myEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(myEncrypt, encryptor, CryptoStreamMode.Write);
            using BinaryWriter stream = new BinaryWriter(csEncrypt, Encoding.UTF8);
            // Write data to stream
            stream.Write(data);
            // Write data from stream to byte array
            csEncrypt.FlushFinalBlock();
            encrypted = myEncrypt.ToArray();
            return encrypted;
        }

        /// <summary>Decrypts data with the AES algorithm.</summary>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Decryption key.</param>
        /// <param name="iv">Decryption IV.</param>
        /// <returns>Decrypted data under the form of a string.</returns>
        /// <exception cref="ArgumentNullException">Null data exception.</exception>
        private static string Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Null exceptions
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            // Decrypted data
            string text;
            // Start Aes algorithm
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            // Create encryptor based on the key and IV
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream myDecrypt = new MemoryStream(data);
            using CryptoStream csDecrypt = new CryptoStream(myDecrypt, decryptor, CryptoStreamMode.Read);
            using BinaryReader stream = new BinaryReader(csDecrypt, Encoding.UTF8);
            // Write data to stream
            text = stream.ReadString();
            return text;
        }

        /// <summary>Transforms a list of <see cref="GameObject3D"/> to JSON information.</summary>
        /// <param name="gos">List of <see cref="GameObject3D"/> to use.</param>
        /// <returns>JSON object under a string format.</returns>
        private static string[] JsonfyGos(List<GameObject3D> gos)
        {
            // Open the jsons
            string modelsJson = "[";
            string cameraJson = "[";
            // Go through every element of the scene's list
            foreach (GameObject3D go in gos)
            {
                if (go is UModel model)
                {
                    modelsJson += "{" + "X: " + model.X + ",Y: " + model.Y + ",Z: " + model.Z + ",Yaw: " + model.Yaw +
                    ",Pitch: " + model.Pitch + ",Roll: " + model.Roll + ",ModelID: \"" + model.ModelID + "\",TextureID: \"" + model.TextureID + "\", Transform:";
                    modelsJson += JsonConvert.SerializeObject(model.Transform);
                    modelsJson += "}, ";
                }
                else if (go is UCamera camera)
                {
                    cameraJson += JsonConvert.SerializeObject(camera);
                    cameraJson += ",";
                }

            }
            // Delete the last comma of the jsons

            if (modelsJson != "[") modelsJson = modelsJson.Substring(0, modelsJson.LastIndexOf(','));
            if (cameraJson != "[") cameraJson = cameraJson.Substring(0, cameraJson.LastIndexOf(','));

            // Close the jsons
            modelsJson += "]";
            cameraJson += "]";
            return new string[] { modelsJson, cameraJson };
        }
    }
}