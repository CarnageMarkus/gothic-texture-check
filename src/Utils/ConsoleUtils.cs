using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GothicTextureCheck.Utils
{
    public static class ConsoleUtils
    {

        public static void AwaitingMessage(string message)
        {
            Console.WriteLine(message);
            Console.ReadKey(true);
            Console.WriteLine();
        }

        public static void Message(string message, bool newline = true)
        {
            if (newline)
                Console.WriteLine(message);
            else
                Console.Write(message);
        }

        public static void MessageColored(string message, ConsoleColor color, bool newline = true)
        {
            var colorOriginal = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Message(message, newline);
            Console.ForegroundColor = colorOriginal;
        }

        public static bool YesNoQuestion(string message, bool yesIsDefault)
        {
            int retries = 0;
            string hint = yesIsDefault ? " [Y/n] " : " [N/y] ";
            string defaultOption = yesIsDefault ? "Y" : "N";

            ConsoleKey response;
            do
            {
                retries++;
                Console.Write(message + hint);
                response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                if (response != ConsoleKey.Y && response != ConsoleKey.N &&  response != ConsoleKey.Enter && retries >= 3)
                {
                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    Console.Write(defaultOption);
                }
                Console.WriteLine();

            } while (response != ConsoleKey.Y && response != ConsoleKey.N && response != ConsoleKey.Enter && retries < 3);

            if (response == ConsoleKey.Y)
            {
                return true;
            }

            if (response == ConsoleKey.N)
            {
                return false;
            }

            //Console.Write(message + hint);
//Console.WriteLine(" " + defaultOption);
            return yesIsDefault;
        }

    }
}
