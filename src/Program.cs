﻿using CommandLine;
using GothicTextureCheck.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GothicTextureCheck
{
    class Program
    {
        static string sb = "___ ____ _  _ ___ _  _ ____ ____    ____ _  _ ____ ____ _  _\r\n" +
                            " |  |___  \\/   |  |  | |__/ |___    |    |__| |___ |    |_/\r\n" +
                            " |  |___ _/\\_  |  |__| |  \\ |___    |___ |  | |___ |___ | \\_\r\n" +
                            "\r\n";

        static Dictionary<string, FileInfo> FileNames = new Dictionary<string, FileInfo>();

        public static ConsoleTables.Format ConsoleTableFormat = ConsoleTables.Format.Default;

        public class Options
        {
            [Option('i', "interactive", Required = false, HelpText = "Interactive mode of the tool. Also works if no arguments are specified.")]
            public bool Interactive { get; set; }

            [Option('d', "directory", Required = false, HelpText = "Directory in which to check files. If not specified, searches folder of the executable")]
            public string Directory { get; set; }

            [Option('r', "recursive", Required = false, HelpText = "Include subdirectiries when searching for files.")]
            public bool Recursive { get; set; }

            [Option("TGA", Required = false, HelpText = "Search for .TGA file extensions.")]
            public bool Targa { get; set; }

            [Option("TEX", Required = false, HelpText = "Search for .TEX file extensions.")]
            public bool Tex { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('t', "tableStyle", Required = false, HelpText = "Set style of the table in verbose mode.(Default, MarkDown, Alternative, Minimal)")]
            public ConsoleTables.Format TableStyle { get; set; }
        }

        static void Main(string[] args)
        {
#if DEBUG
            //args = new string[] { "-d", @"F:\Games\Gothic\_Work\Data\Textures", "-i", /*"--TEX", "-i", "-v", "-t", "Alternative"*/ };
            //args = new string[] { "--version" };
            //
            //args = new string[] { "-d", @"F:\Games\Gothic\_Work\Data\Textures\", "-i", /*"--TEX", "-i", "-v", "-t", "Alternative"*/ };
#endif
            Console.WriteLine(sb);

            DirectoryInfo dir = null;
            List<string> extensions = new List<string>();

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       ConsoleTableFormat = o.TableStyle;

                       if (!string.IsNullOrEmpty(o.Directory))
                       {
                           dir = new DirectoryInfo(o.Directory);
                       }

                       // use working if not supplied
                       if (dir == null)
                       {
                           //var assemblyPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                           //var dirPath = new FileInfo(assemblyPath).Directory.FullName;
                           var currentPath = Environment.CurrentDirectory;
                           dir = new DirectoryInfo(currentPath);
                       }

                       if (dir == null || !dir.Exists)
                       {
                           ConsoleUtils.AwaitingMessage("Directory could not be found! Bye!");
                           return;
                       }

                       if (o.Targa)
                           extensions.Add(".TGA");

                       if (o.Tex)
                           extensions.Add(".TEX");

                       if (o.Interactive || args.Length == 0)
                       {
                           if (!o.Verbose)
                               o.Verbose = ConsoleUtils.YesNoQuestion("Show additional output ?", false);

                           if (!o.Recursive)
                               o.Recursive = ConsoleUtils.YesNoQuestion("Do you want to include subdirectories ?", true);

                           if (o.Verbose)
                               if (ConsoleUtils.YesNoQuestion("Display direcotries searched?", false))
                                   DirectoryUtils.DirConsolePrint(dir);

                           Console.WriteLine();
                           ConsoleUtils.Message("File extensions:");

                           if (!o.Tex)
                               if (ConsoleUtils.YesNoQuestion("Search for .TEX ?", true))
                                   extensions.Add(".TEX");

                           if (!o.Targa)
                               if (ConsoleUtils.YesNoQuestion("Search for .TGA ?", false))
                                   extensions.Add(".TGA");
                           Console.WriteLine();

                       }

                       if (extensions.Count == 0)
                       {
                           ConsoleUtils.AwaitingMessage("Error, no file extension chosen! (Please specify them in arguments, or start interactive mode...)");
                           return;
                       }

                       CheckForDuplicates(dir, extensions, o.Recursive, o.Verbose, o.Interactive);

                       if (o.Interactive)
                           ConsoleUtils.AwaitingMessage("Press any key to continue");

                       CheckTextures(dir, extensions, o.Recursive, o.Verbose, o.Interactive);

                   })
                   .WithNotParsed<Options>((errs =>
                   {
                       //Console.WriteLine("Uups!");
                       return;
                   }));

            Console.WriteLine();
            ConsoleUtils.AwaitingMessage("Goodbye");
        }

        public static void CheckForDuplicates(DirectoryInfo dir, List<string> extensions, bool includeSubdirs = true, bool verbose = false, bool interactive = false)
        {
            int total = 0;
            int duplicates = 0;

            Dictionary<string, List<FileInfo>> duplicateFiles = new Dictionary<string, List<FileInfo>>();

            SearchOption searchOption = includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var files = dir.GetFilesByExtensions(extensions, searchOption);

            if (files == null)
            {
                ConsoleUtils.AwaitingMessage("Direcory is empty!!!");
                return;
            }

            foreach (var file in files)
            {
                var UpperFileName = file.Name.ToUpperInvariant();
                if (FileNames.ContainsKey(UpperFileName))
                {
                    duplicates++;

                    if (duplicateFiles.ContainsKey(UpperFileName))
                    {
                        var listOfDuplicates = duplicateFiles[UpperFileName];
                        listOfDuplicates.Add(file);
                    }
                    else
                    {
                        var originalFile = FileNames[UpperFileName];
                        duplicateFiles.Add(UpperFileName, new List<FileInfo> { originalFile, file });
                    }

                }
                else
                {
                    FileNames.Add(UpperFileName, file);
                }
            }
            total = files.Count();
            ConsoleUtils.Message(String.Format("Found {0} files, {1} duplicates", total, duplicates));
            Console.WriteLine();

            if (duplicateFiles != null && duplicateFiles.Count != 0)
            {
                bool diplayDups = true;
                if (interactive)
                {
                    diplayDups = ConsoleUtils.YesNoQuestion("Display duplicates ?", false);
                    Console.WriteLine();
                }

                if (diplayDups)
                {
                    ConsoleUtils.Message("List of duplicates:");

                    string indent = " \\-> ";
                    foreach (var duplicateEntry in duplicateFiles.Keys)
                    {
                        Console.WriteLine(duplicateEntry);
                        if (verbose)
                        {
                            var duplicateEntryList = duplicateFiles[duplicateEntry];
                            foreach (var duplicateFile in duplicateEntryList)
                            {
                                Console.WriteLine(indent + duplicateFile.FullName);
                            }
                            Console.WriteLine();
                        }
                    }
                    Console.WriteLine();
                }
            }
            return;
        }



        public static void CheckTextures(DirectoryInfo dir, List<string> extensions, bool includeSubdirs = true, bool verbose = false, bool interactive = false)
        {
            SearchOption searchOption = includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var files = dir.GetFilesByExtensions(extensions, searchOption);

            if (files == null)
            {
                ConsoleUtils.AwaitingMessage("Direcory is empty!!!");
                return;
            }

            Dictionary<string, ICompositeTexture> compositeTextureIndex = new Dictionary<string, ICompositeTexture>();

            foreach (var file in files)
            {
                string baseName = file.GetZeroElementName(out Type type);
                if (compositeTextureIndex.ContainsKey(baseName))
                {
                    compositeTextureIndex[baseName]?.Add(file);
                }
                else
                {
                    ICompositeTexture compositeTexture = null;
                    if (type == typeof(VariedColoredTexture))
                        compositeTexture = new VariedColoredTexture(file);

                    if (type == typeof(VariedTexture))
                        compositeTexture = new VariedTexture(file);

                    if (type == typeof(AnimatedTexture))
                        compositeTexture = new AnimatedTexture(file);

                    if (compositeTexture != null)
                        compositeTextureIndex.Add(baseName, compositeTexture);
                }
            }

            ConsoleUtils.Message("Found total " + compositeTextureIndex.Count + " composite textures.");
            Console.WriteLine();

            foreach (var compositeTexture in compositeTextureIndex.Values.ToList())
            {
                var isComplete = compositeTexture.IsComplete();

                ConsoleUtils.Message(compositeTexture.ZeroElement + " " + compositeTexture.GetCompositeStatus(), false);

                if (isComplete)
                {
                    ConsoleUtils.MessageColored(" OK.", ConsoleColor.Green);
                }
                else
                {
                    ConsoleUtils.MessageColored(" Missing textures.", ConsoleColor.Red, false);
                }

                if (!isComplete)
                {
                    // print table if verbose
                    bool showTable = verbose;

                    // let user decide in interactive
                    if (interactive)
                        showTable = ConsoleUtils.YesNoQuestion(" Show table?", false);

                    if (showTable)
                    {
                        Console.WriteLine();
                        compositeTexture.PrintTable();
                        Console.WriteLine();

                        if (interactive)
                            ConsoleUtils.AwaitingMessage("continue (any key)");
                    }

                    Console.WriteLine();

                    var missing = compositeTexture.MissingTextures();

                    if (missing != null || missing.Count != 0)
                    {
                        missing.ForEach((item) => ConsoleUtils.MessageColored(" \\--> " + item, ConsoleColor.Yellow));
                        Console.WriteLine();
                    }


                    if (interactive)
                        ConsoleUtils.AwaitingMessage("continue (any key)");
                }

            }

        }

    }

}