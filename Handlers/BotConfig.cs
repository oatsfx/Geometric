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
        //-----------------This is for using config.json.
        [JsonProperty("Token")]
        public static string Token { get; set; }

        [JsonProperty("Version")]
        public static string Version { get; set; }

        [JsonProperty("EmbedColor")]
        public static EmbedColor Stats { get; set; } // This calls the data set from the JSON, this is required for the class below to actually work.

        public partial class EmbedColor
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
