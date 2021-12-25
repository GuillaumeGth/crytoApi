using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using api.Models;
using System.Reflection;
using api.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoController : ControllerBase
    {       
        private DatabaseContext _context;
        private IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CryptoController> _logger;

        public CryptoController(ILogger<CryptoController> logger,
        DatabaseContext context, 
        IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _context = context;
            _contextAccessor = contextAccessor;
        }
        [HttpPost]
        [Route("transactions")]
        public List<Transaction> LoadTransactions(IFormFile file)
        {            
            try {                
                List<Transaction> transactions = new List<Transaction>();           
                if (file.Length > 0)
                {
                    var filePath = System.IO.Directory.GetCurrentDirectory() + "/" + file.FileName;
                    Logger.Log(filePath);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);                        
                    }
                    FileInfo fi = new FileInfo(filePath);                        
                    int i = 0;
                    Dictionary<string, int> columns = new Dictionary<string, int>();    
                    using(var reader = new StreamReader(filePath))
                    {                                
                        while (!reader.EndOfStream)
                        { 
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            if (i == 0){
                                int y = 0;
                                foreach(string value in values){
                                    string col = string.Join('_', value.Trim().ToLower().Split(' ')).Replace("(utc+1)", string.Empty);                                
                                    Logger.Log(col);
                                    if (!columns.ContainsKey(col))
                                        columns.Add(col, y);

                                    y++;
                                }                                
                                i++;
                            }
                            else{
                                string transacId = values[values.Length -1];
                                Transaction transac = new Transaction();   
                                transactions.Add(transac);                                                           
                                int y = 0;                             
                                foreach(string value in values){
                                    string column = columns.Where(c => c.Value == y).First().Key;
                                    PropertyInfo[] pi =  transac.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);                                    
                                    foreach (PropertyInfo property in pi)
                                    {                                                                             
                                        ColumnAttribute attr = property.GetCustomAttributes().OfType<ColumnAttribute>().Where(a => a.Name == column).FirstOrDefault();
                                        if (attr != null){
                                            property.SetValue(transac, value);
                                        }
                                    }
                                    y++;
                                    
                                } 
                                Logger.Log(transac.ToString());
                            }                                                                 
                        }                        
                    }
                }
                foreach(Transaction t in transactions){
                    Transaction transaction = _context.Transactions.Where(tr => tr.ID == t.ID).FirstOrDefault();
                    if (t != null){
                        transaction = t;
                    }
                    else{
                        _context.Transactions.Add(t);
                    }

                }
                _context.SaveChanges();                       
                return transactions;

            }     
            catch(Exception e)  {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);    
                return null;                          
            }
        }
        [HttpGet]
        [Route("currencies")]
        public object Currencies(string slug = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (slug == null){
                slug = "btc,eth,ada,vet,doge,matic,ltc,sand";
            }
            dic.Add("symbol", slug);
            return CoinMarketCap.Call(CoinMarketCapRoutes.Info, dic);                     
        }
                   
        [HttpGet("Info")]
        public string Info(string currency)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("symbol", currency);
            return CoinMarketCap.Call(CoinMarketCapRoutes.Info, query);                  
        }
        [HttpGet]
        public string Get(string currency)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("symbol", currency);
            return CoinMarketCap.Call(CoinMarketCapRoutes.Quote, query);                  
        }
    }
}
