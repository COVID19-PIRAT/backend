using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Controllers
{
    public class DemandController : ControllerBase
    {

        private readonly ILogger<DemandController> _logger;

        private readonly ApplicationContext _context;

        public DemandController(ILogger<DemandController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }
    }
}
