using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geometric
{
    public partial class BotConfig
    {
        //-----------------Current fix. I wish do use a JSON file, but this will do. :(
        //public static string Token = "MzM4MDcyODcwMzY4NTc1NTE3.DFQLcg.AHo3OUdGz44qtNLK1kGpTHLC0-4";
        //public static string Version = "v0.91";


        //-----------------This is for using config.json.
        [JsonProperty("Token")]
        public static string Token { get; set; }

        [JsonProperty("Version")]
        public static string Version { get; set; }

        [JsonProperty("EmbedColor")]
        public static EmbedColor Stats { get; set; } // This calls the data set from the JSON, this is required for the class below to actually work.

        public partial class EmbedColor // These use long types so that they can be edited in the process of command usage. Integers didn't update for an unknown reason.
        {
            [JsonProperty("R")]
            public static int R { get; set; }

            [JsonProperty("G")]
            public static int G { get; set; }

            [JsonProperty("B")]
            public static int B { get; set; }
        }
    }
}
