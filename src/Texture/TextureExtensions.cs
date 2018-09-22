using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GothicTextureCheck
{
    static class TextureExtensions
    {
        public static string GetNameBase(this FileInfo textureFile)
        {
            var UpperFileName = textureFile.Name.ToUpperInvariant();
            var match = Regex.Match(UpperFileName, VariedColoredTexture.VC_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            match = Regex.Match(UpperFileName, VariedColoredTexture.VC_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            match = Regex.Match(UpperFileName, VariedTexture.V_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            match = Regex.Match(UpperFileName, VariedTexture.V_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            match = Regex.Match(UpperFileName, AnimatedTexture.A_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            match = Regex.Match(UpperFileName, AnimatedTexture.A_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return UpperFileName.Substring(0, match.Index);
            }

            return UpperFileName;
        }

        public static string GetFormatableElementName(this FileInfo textureFile, out Type type)
        {
            var UpperFileName = textureFile.Name.ToUpperInvariant();
            var match = Regex.Match(UpperFileName, VariedColoredTexture.VC_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(VariedColoredTexture);
                return UpperFileName.Substring(0, match.Index) + VariedColoredTexture.NullElement;
            }

            match = Regex.Match(UpperFileName, VariedColoredTexture.VC_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(VariedColoredTexture);
                return UpperFileName.Substring(0, match.Index) + VariedColoredTexture.NullElementTex;
            }

            match = Regex.Match(UpperFileName, VariedTexture.V_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(VariedTexture);
                return UpperFileName.Substring(0, match.Index) + VariedTexture.NullElement;
            }

            match = Regex.Match(UpperFileName, VariedTexture.V_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(VariedTexture);
                return UpperFileName.Substring(0, match.Index) + VariedTexture.NullElementTex;
            }

            match = Regex.Match(UpperFileName, AnimatedTexture.A_TextureRegexEnd, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(AnimatedTexture);
                return UpperFileName.Substring(0, match.Index) + AnimatedTexture.NullElement + textureFile.Extension.ToUpperInvariant();
            }

            match = Regex.Match(UpperFileName, AnimatedTexture.A_TextureRegexEndTex, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                type = typeof(AnimatedTexture);
                return UpperFileName.Substring(0, match.Index) + AnimatedTexture.NullElementTex;
            }
            type = null;
            return null;
        }

        public static string GetZeroElementName(this FileInfo texturefile, out Type type)
        {
            var formatable = texturefile.GetFormatableElementName(out type);
            if (formatable != null)
            {
                if (type == typeof(VariedColoredTexture))
                    return string.Format(formatable, 0, 0);
                if (type == typeof(VariedTexture))
                    return string.Format(formatable, 0);
                if (type == typeof(AnimatedTexture))
                    return string.Format(formatable, 0);
            }
            type = null;
            return texturefile.Name;
        }
    }
}
