using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.V1.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body>TestV2Controller</body></html>", "text/html");
        }
    }
}
