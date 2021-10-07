using System;
using System.IO;
using System.Linq;

namespace GlobusDesktop.Helpers
{
    public static class CultureHelper
    {
        public static readonly string[] SupportedCultures =  {"ru-RU", "en-US", "de-DE", "fr-FR", "zh-CN" };

        public static string DefaultCulture()
        {
            if (!Directory.Exists(CultureInfoFileLocation + "/GlobusConfig"))
                Directory.CreateDirectory(CultureInfoFileLocation + "/GlobusConfig");
            
            if (File.Exists(Path))
                return File.ReadAllText(Path);
            

            File.WriteAllText(Path, SupportedCultures[0]);
            
            Serilog.Log.Information("Got default culture from " + Path);
            
            return SupportedCultures[0];
        } 

        private static readonly string CultureInfoFileLocation = 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static string Path => CultureInfoFileLocation + "/GlobusConfig/Language.txt";

        public static void UpdateDefaultLanguage(string language)
        {
            Serilog.Log.Information("Write new default to " + Path);
            if (SupportedCultures.Contains(language))
                File.WriteAllText(Path, language);
        }
    }
}