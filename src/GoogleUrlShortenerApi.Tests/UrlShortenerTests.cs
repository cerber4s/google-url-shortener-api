using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GoogleUrlShortenerApi.Tests
{

    public class UrlShortenerTests
    {
        [Test]
        public async Task Shorten_ShouldFailWithInvalidApiKeyTest()
        {
            const string apiKey = "INVALID_API_KEY";
            const string longUrl = "https://developers.google.com/url-shortener/v1/getting_started";

            var urlShortener = new UrlShortener(apiKey);
            var result = await urlShortener.Shorten(longUrl);

            Assert.False(result.Ok);
        }

        [Test]
        public async Task ShortenTest()
        {
            var apiKey = ConfigurationManager.AppSettings.Get("apiKey");
            const string longUrl = "https://developers.google.com/url-shortener/v1/getting_started";

            var urlShortener = new UrlShortener(apiKey);
            var result = await urlShortener.Shorten(longUrl);

            Assert.True(result.Ok);
        }
    }
}
