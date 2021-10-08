using System.IO;
using GlobusDesktop.Configs;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GlobusDesktop.Services
{
    class ConfigurationService : IConfigurationService
    {
        public AppConfiguration App { get; private set; }

        private readonly string _configPath;

        public ConfigurationService(IConfiguration configuration)
        {
            _configPath = configuration["ConfigurationPath"];
            Load();
        }
        
        public void Load()
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                App = JsonSerializer.Deserialize<AppConfiguration>(json);
            }
            catch (IOException e)
            {
                App = AppConfiguration.Default;
                if (Directory.Exists(_configPath.Split('/')[0]) == false)
                    Directory.CreateDirectory(_configPath.Split('/')[0]);
                
                Save();
            }
            
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(App);
            File.WriteAllText(_configPath, json);
        }
    }
}