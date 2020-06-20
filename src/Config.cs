using System.Collections.Generic;

namespace IconConv
{
    public class Config
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public string Example { get; set;}

        public Dictionary<string, Dictionary<string, string>> Mappings {get; set;}

        public Dictionary<string, string[]> Duplicates {get; set;}

        public int[] Sizes { get; set; }
    }
}
