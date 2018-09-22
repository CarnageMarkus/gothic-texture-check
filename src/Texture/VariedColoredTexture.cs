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
    class VariedColoredTexture : ICompositeTexture
    {
        internal static string VC_TextureRegexEnd = @"_V[0-9]+_C[0-9]+\.tga";
        internal static string VC_TextureRegexEndTex = @"_V[0-9]+_C[0-9]+-c\.tex";

        internal static string VC_TextureRegex = @"_V[0-9]+_C[0-9]+";
        internal static string VC_C_Regex = @"_C[0-9]+";
        internal static string VC_V_Regex = @"_V[0-9]+_";

        internal static string NullElement = "_V{0}_C{1}.TGA";
        internal static string NullElementTex = "_V{0}_C{1}-C.TEX";

        Dictionary<VarIDColID, FileInfo> Variations = new Dictionary<VarIDColID, FileInfo>();

        public string Name { get; }

        public string NameFormatable { get; }

        public string GetNameBase()
        {
            return Name;
        }

        public string GetCompositeStatus()
        {
            return String.Format("({0} variations, {1} colors)", VariationsCount, ColorsCount);
        }

        public int ColorsCount
        {
            get
            {
                try
                {
                    return Variations.Keys.Select(t => t.C).Distinct().Count();
                }
                catch
                {
                    return Variations.Keys.Max(t => t.C);
                }
            }
        }

        public int VariationsCount
        {
            get
            {
                return Variations.Keys.Count;
            }
        }

        public VariedColoredTexture(FileInfo textureFile)
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
                int variation, color;
                if (TryGetVariationAndColor(textureFile.Name, out variation, out color))
                {
                    if (!Variations.ContainsKey(new VarIDColID(variation, color)))
                    {
                        Variations.Add(new VarIDColID(variation, color), textureFile);
                    }
                    return;
                }
                else
                {
                    throw new ArgumentException("Texture file could not be parsed and added into Varied Colored texture!!!");
                }
            }
            throw new ArgumentException("Texture file could not be added into Varied Colored texture, names mismatch!!!");
        }

        private bool TryGetVariationAndColor(string name, out int variation, out int color)
        {
            var match = Regex.Match(name, VC_TextureRegex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var cMatch = Regex.Match(match.Value, VC_C_Regex, RegexOptions.IgnoreCase);
                if (cMatch.Success)
                {
                    var vMatch = Regex.Match(match.Value, VC_V_Regex);
                    if (vMatch.Success)
                    {
                        var cStr = cMatch.Value.Substring(2, cMatch.Value.Length - 2);
                        int cIndex = -1;
                        if (Int32.TryParse(cStr, out cIndex))
                        {
                            var vStr = vMatch.Value.Substring(2, vMatch.Value.Length - 1 - 2);
                            int vIndex = -1;
                            if (Int32.TryParse(vStr, out vIndex))
                            {
                                variation = vIndex;
                                color = cIndex;
                                return true;
                            }
                        }
                    }
                }
            }
            variation = 0;
            color = 0;
            return false;
        }

        public bool IsComplete()
        {
            int maxColorIndex = Variations.Keys.Max(t => t.C);
            int[] variationRange = new int[maxColorIndex + 1];

            for (int c = 0; c <= maxColorIndex; c++)
            {
                int maxVariationIndex = Variations.Keys.Where(t => t.C == c).Max(t => t.V);
                variationRange[c] = maxVariationIndex;

                if (c != 0 && variationRange[c] > variationRange[c - 1])
                {
                    return false;
                }

                for (int v = 0; v <= maxVariationIndex; v++)
                {
                    if (!Variations.ContainsKey(new VarIDColID(v, c)))
                        return false;
                }
            }
            return true;
        }

        public List<string> MissingTextures()
        {
            List<string> missing = new List<string>();

            int maxVariationIndexGlobal = Variations.Keys.Max(t => t.V);
            int maxColorIndex = Variations.Keys.Max(t => t.C);
            int[] variationRange = new int[maxColorIndex + 1];

            for (int c = 0; c <= maxColorIndex; c++)
            {
                int maxVariationIndex = Variations.Keys.Where(t => t.C == c).Max(t => t.V);
                variationRange[c] = maxVariationIndex;
                // missing to the total maximum in variations
                if (maxVariationIndexGlobal > maxVariationIndex)
                {
                    var diff = maxVariationIndexGlobal - maxVariationIndex;
                    for (int i = 1; i <= diff; i++)
                    {
                        missing.Add(string.Format(NameFormatable, maxVariationIndex + i, c));
                    }
                }
                // missing bellow current maximum variation
                for (int v = 0; v < maxVariationIndex; v++)
                {
                    if (!Variations.ContainsKey(new VarIDColID(v, c)))
                        missing.Add(string.Format(NameFormatable, v, c));
                }
            }

            if (missing.Count == 0)
                missing = null;
            return missing;
        }

        public string ZeroElement => string.Format(NameFormatable, 0, 0);

        public void PrintTable()
        {
            int columnsCount = ColorsCount;
            if (columnsCount == 0)
                return;

            List<string> columns = new List<string>();
            columns.Add("V\\C");
            for (int i = 0; i < columnsCount; i++)
            {
                columns.Add("C" + i);
            }

            ConsoleTable table = new ConsoleTables.ConsoleTable(new ConsoleTableOptions
            {
                Columns = columns,
                EnableCount = false,
            });

            int variationValidMax = int.MaxValue;
            int colorValidMax = int.MaxValue;

            var maxVariationIndexGlobal = Variations.Keys.Max(t => t.V);

            for (int v = 0; v <= maxVariationIndexGlobal; v++)
            {
                List<string> row = new List<string>();
                row.Add("V" + v);

                for (int c = 0; c < columnsCount; c++)
                {
                    if (Variations.ContainsKey(new VarIDColID(v, c)))
                    {
                        table.SetColor(c + 1, v, (v >= variationValidMax && c >= colorValidMax) ? ConsoleColor.Red : ConsoleColor.Green);
                        row.Add("ok");
                    }
                    else
                    {
                        if (c < colorValidMax)
                            colorValidMax = c;

                        if (v < variationValidMax)
                            variationValidMax = v;

                        table.SetColor(c + 1, v, ConsoleColor.Red);
                        row.Add("--");
                    }
                }
                table.AddRow(row.ToArray());
            }

            table.Write(Program.ConsoleTableFormat);
        }

        struct VarIDColID
        {
            public readonly int V;
            public readonly int C;

            public VarIDColID(int v, int c)
            {
                V = v;
                C = c;
            }

            public override int GetHashCode()
            {
                return C * 100000 + V;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is VarIDColID))
                    return false;

                VarIDColID VCobj = (VarIDColID)obj;

                return (this.V == VCobj.V && this.C == VCobj.C);
            }

            public override string ToString()
            {
                return "_" + V + "_" + C;
            }
        }

    }
}
