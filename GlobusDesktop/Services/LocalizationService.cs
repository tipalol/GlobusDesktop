using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GlobusDesktop.Helpers;
using Newtonsoft.Json;

namespace GlobusDesktop.Services
{
    class LocalizationService : ILocalizationService
    {
        private const string DefaultLanguage = "ru-RU";
        
        private static readonly string TranslationFilesLocation = 
            Environment.CurrentDirectory + "/Languages/";
        
        private readonly string[] _supportedTranslations;
        private Dictionary<string, string> _translation;
        private string _currentLanguage;

        public LocalizationService()
        {
            _supportedTranslations = new []{ "ru-RU", "en-US", "de-DE", "fr-FR", "zh-CN" };
            
            Serilog.Log.Information("Loading localization... System is: " + CultureInfo.CurrentCulture.EnglishName);

            var supportedCultures = _supportedTranslations;

            if (supportedCultures.Contains(CultureInfo.CurrentCulture.EnglishName))
            {
                Serilog.Log.Information($"System culture is supported. Loading {CultureInfo.CurrentCulture.EnglishName}");
                Language = CultureInfo.CurrentCulture.EnglishName;
            }
            else
            {
                Serilog.Log.Information($"System culture is not supported. Loading default language {DefaultLanguage}");
                Language = DefaultLanguage;
            }
        }

        public string this[string name] => _translation[name];

        public string Language
        {
            get => _currentLanguage;
            set
            {
                if (_supportedTranslations.Contains(value))
                {
                    _translation = LoadTranslation(value);
                    _currentLanguage = value;
                }
                else
                    throw new ArgumentException($"Language - {value} - is not supported");
            }
        }

        private Dictionary<string, string> LoadTranslation(string language)
        {
            Serilog.Log.Information($"Loading translations from {TranslationFilesLocation}");

            var translations = Directory.GetFiles(
                TranslationFilesLocation, 
                "*.csv", 
                SearchOption.AllDirectories);
            
            Serilog.Log.Information($"Loaded {JsonConvert.SerializeObject(translations)}");

            foreach (var translation in translations)
            {
                var name = Path.GetFileName(translation).Split('.')[0];

                if (language.Contains(translation))
                    return CsvHelper.ReadTranslationFromFile(translation);
            }

            throw new ArgumentException(
                $"This language is not supported - {language} " +
                $"Supported languages are {JsonConvert.SerializeObject(translations)}", 
                language);
        }
    }
}