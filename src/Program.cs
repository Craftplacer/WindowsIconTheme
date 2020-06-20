using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace IconConv
{
    class Program
    {
        static Config Config { get; set; }

        static string ExportDirectory { get; set; }

        static string ConfigPath { get; set; }

        static string ResourceDirectory { get; set; }

        static Config LoadConfig(string path)
        {
            var json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<Config>(json);
            return config;
        }

        static void Main(string[] args)
        {
            Logger.Log("Icon Converter, by Craftplacer");

            if (args.Length < 3)
            {
                Logger.Error("Too few arguments provided.");
                Logger.Log(Constants.Syntax);
                return;
            }

            ConfigPath = args[0];
            if (File.Exists(ConfigPath))
            {
                Config = LoadConfig(ConfigPath);
            }
            else
            {
                Logger.Error("Config file path provided not found.");
                return;
            }

            ResourceDirectory = args[1];
            if (!Directory.Exists(ResourceDirectory))
            {
                Logger.Error("Resource directory provided not found.");
                return;
            }

            ExportDirectory = args[2];
            if (!Directory.Exists(ExportDirectory))
                Directory.CreateDirectory(ExportDirectory);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ConvertIcons();
            ResolveDuplicates();
            GenerateIndex();

            stopwatch.Stop();
            Logger.Log($"Finished in {stopwatch.Elapsed}!");
        }

        static void GenerateIndex()
        {
            List<string> getDirectoryList()
            {
                var directories = new List<string>();

                foreach (var size in Config.Sizes)
                foreach (var context in Constants.Contexts)
                    directories.Add($"{size}x{size}/{context.Item1}");

                directories.Add(string.Empty);

                return directories;
            }

            var indexPath = Path.Combine(ExportDirectory, "index.theme");
            using var writer = new StreamWriter(indexPath, false);

            writer.WriteLine("[Icon Theme]");
            writer.WriteLine("Name=" + Config.Name);
            writer.WriteLine("Directories=" + string.Join(',', getDirectoryList()));

            if (!string.IsNullOrWhiteSpace(Config.Comment))
                writer.WriteLine($"Example={Config.Comment}");

            if (!string.IsNullOrWhiteSpace(Config.Example))
                writer.WriteLine($"Example={Config.Example}");

            foreach (var size in Config.Sizes)
            foreach (var context in Constants.Contexts)
            {
                writer.WriteLine($"[{size}x{size}/{context.Item1}]");
                writer.WriteLine("Context=" + context.Item2);
                writer.WriteLine("Size=" + size);
                writer.WriteLine("Type=Fixed");
            }
        }

        static void ConvertIcons()
        {
            foreach (var kv1 in Config.Mappings)
            {
                var dll = kv1.Key;

                Logger.Log($"Translating {dll}...");

                foreach (var kv2 in kv1.Value)
                {
                    var iconNumber = kv2.Key;
                    var iconFilePath = Path.Combine(ResourceDirectory, $"{dll}_{iconNumber}.ico");

                    if (!File.Exists(iconFilePath))
                    {
                        Logger.Warning("Couldn't find referenced file: " + iconFilePath);
                        continue;
                    }

                    var iconName = kv2.Value;

                    foreach (var iconSize in Config.Sizes)
                    {
                        using var icon = new Icon(iconFilePath, iconSize, iconSize);
                        using var bitmap = icon.ToBitmap();

                        var exportPath = Path.Combine(ExportDirectory, $"{iconSize}x{iconSize}", iconName + ".png");

                        var iconDirectory = Path.GetDirectoryName(exportPath);
                        Directory.CreateDirectory(iconDirectory);

                        bitmap.Save(exportPath);
                    }
                }
            }
        }

        static void ResolveDuplicates()
        {
            Logger.Log("Solving duplicates...");
            foreach (var kv1 in Config.Duplicates)
            foreach (var iconSize in Config.Sizes)
            {
                var source = kv1.Key;
                var sourcePath = Path.Combine(ExportDirectory, $"{iconSize}x{iconSize}", source + ".png");

                var destinations = kv1.Value;

                foreach (var destination in destinations)
                {
                    var destinationPath = Path.Combine(ExportDirectory, $"{iconSize}x{iconSize}", destination + ".png");
                        
                    if (File.Exists(destinationPath))
                    {
                        Logger.Warning($"Skipping {source} -> {destination}, file exists...");
                        continue;
                    }

                    var destinationDirectory = Path.GetDirectoryName(destinationPath);
                    Directory.CreateDirectory(destinationDirectory);

                    // HACK: Replace with symlink to reduce disk usage
                    File.Copy(sourcePath, destinationPath);
                }
            }
        }
    }
}
