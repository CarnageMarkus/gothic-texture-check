using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GothicTextureCheck
{
    public interface ICompositeTexture
    {
        string ZeroElement { get; }

        string GetNameBase();

        string GetCompositeStatus();

        void Add(FileInfo textureFile);

        bool IsComplete();

        List<string> MissingTextures();

        void PrintTable();
    }
}
