using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Pirat.Model;
using Pirat.Services;

namespace Pirat.Extensions
{

    public static class ReCaptchaMiddlewareExtensions
    {
        public static IApplicationBuilder UseReCapture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ReCaptchaMiddleware>();
        }
    }

    public class ReCaptchaMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IReCaptchaService _captchaService;

        private const string key = "recaptchaResponse";

        private readonly List<string> blackList = new List<string>(){ "/resources"};

        public ReCaptchaMiddleware(RequestDelegate next, IReCaptchaService service)
        {
            _next = next;
            _captchaService = service;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;
            if (blackList.Contains(path) && method.ToUpper().Equals(WebRequestMethods.Http.Post))
            {
                context.Request.EnableBuffering();
                var stream = context.Request.Body;

                using (var reader = new StreamReader(stream))
                {
                    var body = await reader.ReadToEndAsync();
                    if (stream.CanSeek)
                        stream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        var recaptcha = JsonConvert.DeserializeObject<ReCaptchaResponse>(body);
                        if (await _captchaService.ValidateResponse(recaptcha.recaptchaResponse))
                        {
                            await _next.Invoke(context);
                        }
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("Wron ReCaptcha");
                        }
                    }
                    catch (Exception e)
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        await context.Response.WriteAsync("Missing ReCaptcha");
                    }
                }
            }

            await _next.Invoke(context);
        }

    }
}
