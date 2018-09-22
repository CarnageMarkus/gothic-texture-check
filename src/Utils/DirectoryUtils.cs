using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GothicTextureCheck
{

    public static class DirectoryUtils
    {
        private static void DirConsolePrint(DirectoryInfo dir, string indent, bool last)
        {
            Console.WriteLine(indent + "+- " + dir.Name);
            indent += last ? "   " : "|  ";

            DirectoryInfo[] children = dir.GetDirectories();

            for (int i = 0; i < children.Length; i++)
            {
                DirConsolePrint(children[i], indent, i == children.Length - 1);
            }
        }

        public static void DirConsolePrint(DirectoryInfo dir)
        {
            Console.WriteLine(dir.FullName);
            DirConsolePrint(dir, string.Empty, true);
        }

        public static void DirConsolePrint(string directoryPath)
        {
            Console.WriteLine(directoryPath);
            DirConsolePrint(new DirectoryInfo(directoryPath), string.Empty, true);
        }

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, List<string> extensions, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (extensions == null || extensions.Count == 0)
                throw new ArgumentException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles("*", searchOption);
            return files.Where(f => extensions.Contains(f.Extension.ToUpperInvariant()));
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
