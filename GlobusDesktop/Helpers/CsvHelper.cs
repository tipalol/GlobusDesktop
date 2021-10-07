using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace GlobusDesktop.Helpers
{
    internal static class CsvHelper
    {
        public static Dictionary<string, string> ReadTranslationFromFile(string absolutePath)
        {
            var translation = new Dictionary<string, string>();
            var table = new DataTable();

            using var csvReader = new CsvReader(new StreamReader(File.OpenRead(absolutePath)), true);
            
            table.Load(csvReader);

            var rows = table.Rows;

            for (var i = 0; i < rows.Count; i++)
            {
                var name = rows[i][0] as string;
                var value = rows[i][1] as string;

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                    throw new Exception("Translation parsing error");
                
                translation.Add(name, value);
            }

            return translation;
        }
    }
}