using System;
using System.Collections.Generic;
using System.Text;

namespace Obiektuwa.Classes {
    internal class Helpers {
        public static readonly string mainDirectory = "/cache";
        public static readonly string workingDirectory = Environment.CurrentDirectory;

        public static string GetClassCacheDirectory<T>() {
            string className = typeof(T).Name;

            return Path.Join(workingDirectory, mainDirectory, $"{className}.json");
        }
    }
}
