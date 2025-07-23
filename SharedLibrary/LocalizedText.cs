using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLibrary
{
    /// <summary>
    /// Represents text that supports French and English translations
    /// </summary>
    public class LocalizedText
    {
        [JsonPropertyName("en")]
        public string English { get; set; } = string.Empty;

        [JsonPropertyName("fr")]
        public string French { get; set; } = string.Empty;

        public LocalizedText() { }

        public LocalizedText(string english, string french)
        {
            English = english;
            French = french;
        }

        public LocalizedText(string text)
        {
            English = text;
            French = text;
        }

        /// <summary>
        /// Gets the text for the specified language
        /// </summary>
        /// <param name="language">Language code: "en" or "fr"</param>
        /// <returns>The localized text, falls back to English if language not found</returns>
        public string GetText(string language = "en")
        {
            return language.ToLower() switch
            {
                "fr" => !string.IsNullOrEmpty(French) ? French : English,
                _ => English
            };
        }

        /// <summary>
        /// Sets the text for the specified language
        /// </summary>
        /// <param name="language">Language code: "en" or "fr"</param>
        /// <param name="text">The text to set</param>
        public void SetText(string language, string text)
        {
            switch (language.ToLower())
            {
                case "fr":
                    French = text;
                    break;
                case "en":
                default:
                    English = text;
                    break;
            }
        }

        /// <summary>
        /// Checks if the text is available in the specified language
        /// </summary>
        /// <param name="language">Language code: "en" or "fr"</param>
        /// <returns>True if text is available in the language</returns>
        public bool HasTranslation(string language)
        {
            return language.ToLower() switch
            {
                "fr" => !string.IsNullOrEmpty(French),
                "en" => !string.IsNullOrEmpty(English),
                _ => false
            };
        }

        public override string ToString() => GetText();

        public static implicit operator string(LocalizedText localizedText) => localizedText.GetText();
        public static implicit operator LocalizedText(string text) => new(text);
    }
}