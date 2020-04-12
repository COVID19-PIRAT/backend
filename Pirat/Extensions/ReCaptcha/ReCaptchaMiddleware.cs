using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Pirat.Model;
using Pirat.Other;
using Pirat.Services;
using Pirat.Services.Middleware;

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

        private const string HeaderKey = "recaptcha";

        private readonly List<string> resourceEndings = new List<string>(){"consumables", "devices", "manpower"};

        private readonly List<string> blackList = new List<string>(){ "/resources", "/telephone-callback", "/subscription" };

        public ReCaptchaMiddleware(RequestDelegate next, IReCaptchaService service)
        {
            _next = next;
            _captchaService = service;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            NullCheck.ThrowIfNull<HttpContext>(context);

            var path = context.Request.Path.ToString();
            var method = context.Request.Method;
            if ((blackList.Contains(path) && method.ToUpper(CultureInfo.InvariantCulture).Equals(WebRequestMethods.Http.Post, StringComparison.Ordinal)) 
                || isContactEnding(path) && method.ToUpper(CultureInfo.InvariantCulture).Equals(WebRequestMethods.Http.Post, StringComparison.Ordinal))
            {

                var headerValue = context.Request.Headers[HeaderKey].ToString();
                if (string.IsNullOrEmpty(headerValue))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Missing ReCaptcha");
                    return;
                }
                if (await _captchaService.ValidateResponseAsync(headerValue))
                {
                    await _next(context);
                    return;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Wrong ReCaptcha");
                    return;
                }
            }

            await _next(context);
        }

        private bool isContactEnding(string path)
        {
            string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 4)
            {
                foreach (var r in resourceEndings)
                {
                    if (segments[0].Equals("resources", StringComparison.Ordinal) 
                        && resourceEndings.Contains(segments[1]) 
                        && int.TryParse(segments[2], out _) 
                        && segments[3].Equals("contact", StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
