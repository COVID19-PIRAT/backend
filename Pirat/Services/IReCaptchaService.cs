using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pirat.Services
{
    public interface IReCaptchaService
    {
        public Task<bool> ValidateResponse(string response);
    }

}
