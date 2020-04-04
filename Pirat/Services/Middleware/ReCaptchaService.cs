using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pirat.Model;

namespace Pirat.Services
{
    class ReCaptchaService : IReCaptchaService
    {
        private readonly string _secret;

        public ReCaptchaService(string secret)
        {
            _secret = secret;
        }

        public async Task<bool> ValidateResponse(string response)
        {
            const string url = "https://www.google.com/recaptcha/api/siteverify";
            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("secret", this._secret),
                new KeyValuePair<string, string>("response", response)
            };
            HttpClient client = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(body) };
            HttpResponseMessage res = await client.SendAsync(req);
            string content = await res.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ReCaptchaVerification>(content).success;
        }
    }
}
