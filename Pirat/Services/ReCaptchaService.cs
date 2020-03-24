using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pirat.Services
{
    class ReCaptchaService : IReCaptchaService
    {
        private readonly string _secret;

        public ReCaptchaService()
        {
            this._secret = Environment.GetEnvironmentVariable("PIRAT_GOOGLE_RECAPTCHA_SECRET");
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
            Console.WriteLine(content);
            // TODO Max, kannst du das Parsen machen?
            // Content should be:
            // {
            //     "success": true,
            //     "challenge_ts": "2020-03-24T18:48:51Z",
            //     "hostname": "localhost"
            // }
            // The value of success should be returned.
            return true;
        }
    }
}
