using System.Threading.Tasks;

namespace Pirat.Services.Middleware
{
    public interface IReCaptchaService
    {
        public Task<bool> ValidateResponse(string response);
    }

}
