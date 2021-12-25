
using System;
namespace api.Services{
    public class ApiRoute : Attribute {
        public string Route;
        public override string ToString()
        {
            return $"ApiRoute: {Route}";
        }
    }
    public enum CoinMarketCapRoutes {
        [ApiRoute(Route = "v1/cryptocurrency/info")]
        Info,
        [ApiRoute(Route = "v1/cryptocurrency/quotes/latest")]
        Quote
    }
}