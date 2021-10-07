namespace GlobusDesktop.Services
{
    public interface ILocalizationService
    {
        public string this[string name] { get; }
        public string Language { get; set; }
    }
}