using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
namespace api.Controllers {
    public class BaseApiController : ControllerBase {
        protected DatabaseContext Context;
        protected IHttpContextAccessor ContextAccessor;
        protected readonly ILogger<CryptoController> Logger;

        public BaseApiController(ILogger<CryptoController> logger,
        DatabaseContext context, 
        IHttpContextAccessor contextAccessor)
        {
            Logger = logger;
            Context = context;
            ContextAccessor = contextAccessor;
        }
    }
}