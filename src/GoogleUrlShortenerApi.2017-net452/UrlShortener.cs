using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KF.GoogleUrlShortenerApi
{
    public class UrlShortener
    {
        private readonly string _apiKey;
        private readonly string _uri;

        private const string DefaultUri = "https://www.googleapis.com/urlshortener/v1/url";

        public UrlShortener(string apiKey, string uri = DefaultUri)
        {
            _apiKey = apiKey;
            _uri = uri;
        }

        public async Task<ShortenResult> Shorten(string longUrl)
        {
            var httpClient = new HttpClient();
            var uri = $"{_uri}?key={_apiKey}";

            var content = new { longUrl };

            var typeFormatter = new JsonMediaTypeFormatter();
            var response = await httpClient.PostAsync(uri, content, typeFormatter);

            var json = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(json);

            if (response.IsSuccessStatusCode)
            {
                return new ShortenResult
                {
                    Ok = true,

                    Kind = jObject.Value<string>("kind"),
                    Id = jObject.Value<string>("id"),
                    LongUrl = jObject.Value<string>("longUrl"),
                    Status = jObject.Value<string>("status")
                };
            }

            var error = jObject.Value<JObject>("error");
            var errors = error.Value<JArray>("errors");

            return new ShortenResult
            {
                Error = new Error
                {
                    Errors = errors.Select(x => new ErrorDescription
                    {
                        Domain = x.Value<string>("domain"),
                        Reason = x.Value<string>("reason"),
                        Message = x.Value<string>("message"),
                        Location = x.Value<string>("location"),
                        LocationType = x.Value<string>("locationType")
                    }).ToArray(),
                    Code = error.Value<int>("code"),
                    Message = error.Value<string>("message"),
                }
            };
        }
    }

    public class ShortenResult
    {
        public bool Ok { get; set; } = false;

        public string Kind { get; set; }

        public string Id { get; set; }

        public string LongUrl { get; set; }

        public string Status { get; set; }

        public Error Error { get; set; }
    }

    public class Error
    {
        public ErrorDescription[] Errors { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ErrorDescription
    {
        public string Domain { get; set; }

        public string Reason { get; set; }

        public string Message { get; set; }

        public string LocationType { get; set; }

        public string Location { get; set; }
    }
}
