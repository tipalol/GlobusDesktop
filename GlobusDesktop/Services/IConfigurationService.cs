using GlobusDesktop.Configs;

namespace GlobusDesktop.Services
{
    public interface IConfigurationService
    {
        public AppConfiguration App { get; }
        public void Load();
        public void Save();
    }
}