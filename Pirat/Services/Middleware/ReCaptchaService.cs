using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pirat.Model;

namespace Pirat.Services.Middleware
{
    class ReCaptchaService : IReCaptchaService
    {
        private readonly string _secret;

        public ReCaptchaService(string secret)
        {
            _secret = secret;
        }

        public async Task<bool> ValidateResponseAsync(string response)
        {
            const string url = "https://www.google.com/recaptcha/api/siteverify";
            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("secret", this._secret),
                new KeyValuePair<string, string>("response", response)
            };
            using var client = new HttpClient();
            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
            var res = await client.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ReCaptchaVerification>(content).success;
        }
    }
}
