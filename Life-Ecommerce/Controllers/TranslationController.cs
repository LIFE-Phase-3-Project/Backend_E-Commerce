using Application.Services.TranslationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;
        private readonly IStringLocalizer<TranslationController> _localizer;

        public TranslationController(ITranslationService translationService, IStringLocalizer<TranslationController> localizer)
        {
            _translationService = translationService;
            _localizer = localizer;
        }

        [HttpGet("translate")]
        public async Task<IActionResult> TranslateText(string text, string targetLanguage)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(targetLanguage))
            {
                return BadRequest("Text and target language must be provided.");
            }

            try
            {
                var translatedText = await _translationService.TranslateTextAsync(text, targetLanguage);
                return Ok(new { OriginalText = text, TranslatedText = translatedText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("greeting")]
        public IActionResult GetGreeting()
        {
            var greeting = _localizer["Greeting"];
            return Ok(new { Greeting = greeting });
        }
    }
}

