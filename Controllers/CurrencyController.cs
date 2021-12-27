using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using api.Services;
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : BaseApiController
    {
        public CurrencyController(ILogger<CryptoController> logger,
        DatabaseContext context, 
        IHttpContextAccessor contextAccessor) : base(logger, context, contextAccessor)
        {}
        [HttpGet]
        public string Get(string symbol)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            if(symbol == null) symbol = "btc,eth,ada,vet,doge,matic,ltc,sand,mana";
            query.Add("symbol", symbol);         
            return CoinMarketCap.Call(CoinMarketCapRoutes.Info, query);                  
        }       
    }
}
