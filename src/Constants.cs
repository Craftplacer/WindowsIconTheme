using System;

namespace IconConv
{
    public class Constants
    {
        public const string Syntax = @"icon-conv <config file> <resource directory> <export directory>";

        public static readonly Tuple<string, string>[] Contexts = new Tuple<string, string>[] {
            Tuple.Create("emblems", "Emblems"),
            // Tuple.Create("legacy", "Legacy"),
            Tuple.Create("actions", "Actions"),
            Tuple.Create("apps", "Applications"),
            Tuple.Create("categories", "Categories"),
            Tuple.Create("devices", "Devices"),
            Tuple.Create("emotes", "Emotes"),
            Tuple.Create("mimetypes", "MimeTypes"),
            Tuple.Create("places", "Places"),
            Tuple.Create("status", "Status"),
            // Tuple.Create("ui", "UI"),
        };
    }
}