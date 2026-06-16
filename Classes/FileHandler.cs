using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Obiektuwa.Classes {
    public class FileHandler<T> {
        private readonly string filepath;
        public FileHandler(string filepath) {
            ArgumentNullException.ThrowIfNull(filepath);
            this.filepath = filepath;
        
            try {
                if (!Path.Exists(filepath)) {
                    DirectoryInfo directory = new DirectoryInfo(filepath).Parent ?? throw new Exception("Couldn't get directory from filepath");
                    Directory.CreateDirectory(directory.FullName);

                    File.Create(filepath).Close();
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException) {
                throw new IOException($"Nie udało się przygotować pliku danych: {filepath}", ex);
            }
        }

        public List<T>? ReadFile() {
            try {
                string content = File.ReadAllText(filepath);

                if (string.IsNullOrWhiteSpace(content)) {
                    return new List<T>();
                }
            
                return JsonConvert.DeserializeObject<List<T>>(content) ?? new List<T>();
            }
            catch (JsonException ex) {
                Console.WriteLine($"Plik danych jest uszkodzony ({filepath}). Program użyje pustej listy. Szczegóły: {ex.Message}");
                return new List<T>();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) {
                Console.WriteLine($"Nie udało się odczytać pliku danych ({filepath}). Program użyje pustej listy. Szczegóły: {ex.Message}");
                return new List<T>();
            }
        }

        public bool WriteFile(List<T> obj) {
            try {
                File.WriteAllText(filepath, JsonConvert.SerializeObject(obj));
                return true;
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException || ex is UnauthorizedAccessException) {
                Console.WriteLine($"Nie udało się zapisać pliku danych ({filepath}). Szczegóły: {ex.Message}");
                return false;
            }
        }

        public bool writeFile(List<T> obj) {
            return WriteFile(obj);
        }
    }
}
