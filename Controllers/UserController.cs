using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {       
        private readonly ILogger<CryptoController> _logger;

        public UserController(ILogger<CryptoController> logger)
        {
            _logger = logger;
        }         

        [HttpPost]
        [Route("login")]
        public void Login(string email)
        {
            Logger.Log(email);
        }
    }
}
