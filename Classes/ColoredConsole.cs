using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.UI {
    internal class ColoredConsole {
        public static void WriteLine(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            var prevForegroundColor = Console.ForegroundColor;
            var prevBackgroundColor = Console.BackgroundColor;
            
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.WriteLine(value);

            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;
        }

        public static void Write(object value, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
            var prevForegroundColor = Console.ForegroundColor;
            var prevBackgroundColor = Console.BackgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(value);

            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;
        }
    }
}
