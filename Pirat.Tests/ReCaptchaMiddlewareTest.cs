using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Pirat.Extensions;
using Pirat.Services;

namespace Pirat.Tests
{
    public class ReCaptchaMiddlewareTest
    {

        private HttpContext createHttpContext( string method, string path, string recaptchaValue = null)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("localhost");
            context.Request.PathBase = new PathString("/");
            context.Request.Path = new PathString(path);
            if (!(recaptchaValue is null))
            {
                context.Request.Headers.Add("recaptcha", new StringValues(recaptchaValue));
            }
            context.Request.Method = method;
            return context;
        }

        [Test]
        public async Task Test_MissingReCaptcha()
        {
            var recaptchaServiceMock = new Mock<IReCaptchaService>();
            recaptchaServiceMock.Setup(m => m.ValidateResponse(It.IsAny<string>())).Returns(Task.FromResult(false));
            var reCaptchaMiddleware = new ReCaptchaMiddleware(next: (innerHttpContext) => Task.FromResult(0),
                recaptchaServiceMock.Object);

            var context = createHttpContext("POST", "/resources");

            await reCaptchaMiddleware.Invoke(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(context.Response.Body).ReadToEnd();
            Assert.AreEqual("Missing ReCaptcha", result);
            Assert.True(StatusCodes.Status400BadRequest == context.Response.StatusCode);
        }

        [Test]
        public async Task Test_Blacklist_WrongReCaptcha()
        {
            var recaptchaServiceMock = new Mock<IReCaptchaService>();
            recaptchaServiceMock.Setup(m => m.ValidateResponse(It.IsAny<string>())).Returns(Task.FromResult(false));
            var reCaptchaMiddleware = new ReCaptchaMiddleware(next: (innerHttpContext) => Task.FromResult(0),
                recaptchaServiceMock.Object);

            var blackList = new List<string>()
            {
                "/resources",
                "/resources/consumables/124/contact",
                "/resources/manpower/2333/contact",
                "/resources/devices/2434/contact",
                "/telephone-callback"
            };

            foreach (var path in blackList)
            {
                var context = createHttpContext("POST", path, "nothingSpecial");

                await reCaptchaMiddleware.Invoke(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var result = new StreamReader(context.Response.Body).ReadToEnd();
                Assert.AreEqual("Wrong ReCaptcha", result);
                Assert.True(StatusCodes.Status403Forbidden == context.Response.StatusCode);
            }
        }

        [Test]
        public async Task Test_ShouldPass()
        {
            var recaptchaServiceMock = new Mock<IReCaptchaService>();
            recaptchaServiceMock.Setup(m => m.ValidateResponse(It.IsAny<string>())).Returns(Task.FromResult(false));
            var reCaptchaMiddleware = new ReCaptchaMiddleware(next: (innerHttpContext) => Task.FromResult(0),
                recaptchaServiceMock.Object);

            var whiteList = new List<string>()
            {
                "/resources/devices",
                "/resources/consumables",
                "/resources/manpower",
                "/resources/offers/2134",
            };
            foreach (var path in whiteList)
            {
                var context = createHttpContext("POST", path);

                await reCaptchaMiddleware.Invoke(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var result = new StreamReader(context.Response.Body).ReadToEnd();
                Assert.True(result.Length == 0);
                Assert.True(StatusCodes.Status200OK == context.Response.StatusCode);
            }

        }
    }
}