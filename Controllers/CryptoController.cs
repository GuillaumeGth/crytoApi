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
using api;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoController : BaseApiController
    {    
        public CryptoController(ILogger<CryptoController> logger,
        DatabaseContext context, 
        IHttpContextAccessor contextAccessor): base(logger, context, contextAccessor){}       
        [HttpGet]
        public List<Transaction> Get(string user) {
            ApiLogger.Log(user);
            return Context.Transactions
                .Where(t => t.User == user)
                .Where(t => t.Status == TransactionStatus.Completed)
                .ToList();
        }
        [HttpPost]
        [Route("transactions")]
        public void LoadTransactions(IFormFile file, string user)
        {            
            try {                
                List<Transaction> transactions = new List<Transaction>();           
                if (file.Length > 0)
                {
                    var filePath = System.IO.Directory.GetCurrentDirectory() + "/" + file.FileName;
                    ApiLogger.Log(filePath);
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
                                    if (!columns.ContainsKey(col))
                                        columns.Add(col, y);

                                    y++;
                                }                                
                                i++;
                            }
                            else{
                                string transacId = values[values.Length -1];
                                Transaction transac = new Transaction() {User = user};   
                                transactions.Add(transac);                                                           
                                int y = 0;                             
                                foreach(string value in values){
                                    string column = columns.Where(c => c.Value == y).First().Key;
                                    PropertyInfo[] pi =  transac.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);                                    
                                    foreach (PropertyInfo property in pi)
                                    {                                                                             
                                        ColumnAttribute attr = 
                                            property
                                            .GetCustomAttributes()
                                            .OfType<ColumnAttribute>()
                                            .Where(a => a.Name == column).FirstOrDefault();
                                        object propertyValue = value;
                                        if (attr != null){
                                            if(property.PropertyType == typeof(TransactionStatus)){
                                                propertyValue = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), value);                                                
                                            }                                            
                                            property.SetValue(transac, propertyValue);                                            
                                        }                                    
                                    }
                                    y++;
                                    
                                }                                
                            }                                                                 
                        }                        
                    }
                }
                Context.Database.EnsureCreated();
                ApiLogger.Log(transactions.Count());
                foreach(Transaction t in transactions){  
                    ApiLogger.Log(t.User);                      
                    Transaction transaction = Context.Transactions.Where(tr => tr.ID == t.ID).FirstOrDefault();                    
                    if (string.IsNullOrEmpty(transaction?.ID)){                    
                        Context.Transactions.Add(t);
                    }                    
                }
                ApiLogger.Log(Context.Transactions.Count());
                Context.SaveChanges();                       
            }     
            catch(Exception e)  {
                ApiLogger.Error(e.Message);
                ApiLogger.Error(e.StackTrace);                
            }
        }
        [HttpGet]
        [Route("quotes")]
        public string Quotes(string symbol = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (symbol == null){
                symbol = "btc,eth,ada,vet,doge,matic,ltc,sand";
            }
            dic.Add("symbol", symbol);
            string res = CoinMarketCap.Call(CoinMarketCapRoutes.Quote, dic);            
            return res;                     
        }        
    }
}
