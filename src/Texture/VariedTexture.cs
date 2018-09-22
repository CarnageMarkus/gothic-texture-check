using ConsoleTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GothicTextureCheck
{

    class VariedTexture : ICompositeTexture
    {
        internal static string V_TextureRegexEnd = @"_V[0-9]+\.tga";
        internal static string V_TextureRegexEndTex = @"_V[0-9]+-c\.tex";
        internal static string V_TextureRegex = @"_V[0-9]+";

        internal static string NullElement = "_V{0}.TGA";
        internal static string NullElementTex = "_V{0}-C.TEX";

        SortedDictionary<int, FileInfo> Variations = new SortedDictionary<int, FileInfo>();

        public string Name { get; }

        public string NameFormatable { get; }

        public string GetNameBase()
        {
            return Name;
        }

        public string GetCompositeStatus()
        {
            return String.Format("({0} variations)", VariationCount);
        }

        public int VariationCount
        {
            get
            {
                return Variations.Keys.Count;
            }
        }

        public VariedTexture(FileInfo textureFile)
        {
            Name = textureFile.GetNameBase();
            NameFormatable = textureFile.GetFormatableElementName(out var type);
            Add(textureFile);
        }

        public void Add(FileInfo textureFile)
        {
            var name = textureFile.GetNameBase();
            if (Name.Equals(name))
            {
                if (TryGetVariation(textureFile.Name, out int index))
                {
                    if (!Variations.ContainsKey(index))
                    {
                        Variations.Add(index, textureFile);
                    }
                    return;
                }
                else
                {
                    throw new ArgumentException("Texture file could not be parsed and added into Varied texture!!!");
                }
            }
            throw new ArgumentException("Texture file could not be added into Varied texture, names mismatch!!!");
        }

        private bool TryGetVariation(string name, out int variation)
        {
            var match = Regex.Match(name, V_TextureRegex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var vStr = match.Value.Substring(2, match.Value.Length - 2);
                int vIndex = -1;
                if (Int32.TryParse(vStr, out vIndex))
                {
                    variation = vIndex;
                    return true;
                }
            }
            variation = 0;
            return false;
        }

        public bool IsComplete()
        {
            for (int i = 0; i < Variations.Keys.Count; i++)
            {
                if (!Variations.ContainsKey(i))
                {
                    return false;
                }
            }
            return true;
        }

        public List<string> MissingTextures()
        {
            List<string> missing = new List<string>();

            int last = Variations.Keys.Last();

            for (int i = 0; i < last; i++)
            {
                if (!Variations.ContainsKey(i))
                {
                    missing.Add(string.Format(NameFormatable, i));
                }
            }

            if (missing.Count == 0)
                missing = null;
            return missing;
        }

        public string ZeroElement => string.Format(NameFormatable, 0);

        public void PrintTable()
        {
            List<string> columns = new List<string>
            {
                "Visual Skin"
            };
            columns.Add("");

            ConsoleTable table = new ConsoleTables.ConsoleTable(new ConsoleTableOptions
            {
                Columns = columns,
                EnableCount = false
            });

            var maxVariation = Variations.Keys.Last();

            int variationValidMax = int.MaxValue;

            for (int v = 0; v <= maxVariation; v++)
            {
                List<string> row = new List<string>();
                row.Add("V" + v);

                if (Variations.ContainsKey(v))
                {
                    table.SetColor(1, v, (v >= variationValidMax) ? ConsoleColor.Red : ConsoleColor.Green);
                    row.Add("ok");
                }
                else
                {
                    if (v < variationValidMax)
                        variationValidMax = v;

                    table.SetColor(1, v, ConsoleColor.Red);
                    row.Add("--");
                }
                table.AddRow(row.ToArray());
            }

            table.Write(Program.ConsoleTableFormat);
        }
    }
}
