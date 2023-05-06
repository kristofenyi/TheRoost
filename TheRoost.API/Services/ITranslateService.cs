using TheRoost.API.Models.Json;

namespace TheRoost.API.Services
{
    public interface ITranslateService
    {
        TranslateJsonList TranslateToCZandSK(string text);
        void WriteToResourceFileSK(string key, string value);
        void WriteToResourceFileCS(string key, string value);
    }
}
