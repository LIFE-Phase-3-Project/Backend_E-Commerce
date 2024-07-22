//using Google.Cloud.Translation.V2;
//using System.Collections.Generic;

//public class TranslationService
//{
//    private readonly TranslationClient _client;

//    public TranslationService(string apiKey)
//    {
//        _client = TranslationClient.CreateFromApiKey(apiKey);
//    }

//    public string TranslateText(string text, string targetLanguage)
//    {
//        var response = _client.TranslateText(text, targetLanguage);
//        return response.TranslatedText;
//    }

//    public Dictionary<string, string> GetTranslations(string text)
//    {
//        var translations = new Dictionary<string, string>
//        {
//            { "en", TranslateText(text, "en") },
//            { "sq", TranslateText(text, "sq") }
//        };
//        return translations;
//    }
//}
