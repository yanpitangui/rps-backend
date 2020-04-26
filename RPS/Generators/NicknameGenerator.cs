using Newtonsoft.Json;
using RPS.Generators.Helpers;
using System;
using System.Globalization;
using System.IO;

namespace RPS.Generators
{
    /// <summary>
    /// Source of files: https://github.com/dariusk/corpora
    /// </summary>
    public class NicknameGenerator
    {
        private readonly AdjsJson adjs;
        private readonly AdverbsJson adverbs;
        private readonly NounsJson nouns;
        static Random rnd;
        public NicknameGenerator()
        {
            var adjsString = File.ReadAllText("Generators/adjs.json");
            adjs = JsonConvert.DeserializeObject<AdjsJson>(adjsString);
            var adverbsString = File.ReadAllText("Generators/adverbs.json");
            adverbs = JsonConvert.DeserializeObject<AdverbsJson>(adverbsString);
            var nounsString = File.ReadAllText("Generators/nouns.json");
            nouns = JsonConvert.DeserializeObject<NounsJson>(nounsString);
            rnd = new Random();
        }

        public string Generate()
        {
            //this is terrible
            return $"{ToTitleCase(adjs.adjs[rnd.Next(adjs.adjs.Count)])}{ToTitleCase(adverbs.adverbs[rnd.Next(adverbs.adverbs.Count)])}{ToTitleCase(nouns.nouns[rnd.Next(nouns.nouns.Count)])}";
        }
        internal static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
    }
}
