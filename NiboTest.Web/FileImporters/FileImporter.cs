using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiboTest.Web.FileImporters
{
    internal abstract class FileImporter
    {
        internal abstract void ImportarArquivo(string conteudo);
    }
}