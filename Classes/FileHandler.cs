using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Obiektuwa.Classes {
    internal class FileHandler<T> {
        private readonly string filepath;
        public FileHandler(string filepath) {
            ArgumentNullException.ThrowIfNull(filepath);
            this.filepath = filepath;
        
            if (!Path.Exists(filepath)) {
                DirectoryInfo directory = new DirectoryInfo(filepath).Parent ?? throw new Exception("Couldn't get directory from filepath");
                Directory.CreateDirectory(directory.FullName);

                File.Create(filepath).Close();
            }
        }

        public List<T>? ReadFile() {
            string content = File.ReadAllText(filepath);
            
            return JsonConvert.DeserializeObject<List<T>>(content);
        }

        public bool writeFile(List<T> obj) {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(obj));
            return true;
        }
    }
}
