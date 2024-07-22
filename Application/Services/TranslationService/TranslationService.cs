using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.TranslationService
{
    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TranslationService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            var requestBody = new
            {
                text,
                target_lang = targetLanguage
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.deepl.com/v2/translate")
            {
                Content = content,
                Headers =
            {
                { "Authorization", $"DeepL-Auth-Key {_apiKey}" }
            }
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<DeepLResponse>(jsonResponse);

            return result.Translations[0].Text;
        }
    }
}
