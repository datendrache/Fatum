using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FatumCore;

namespace Fatum.FatumCore
{
    public interface ITreeAdapter
    {
        Tree Read(string Filename);
        Tree Read(Stream instream, string filename);

    }
}
