using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using api.Models;
using System.Reflection;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoController : ControllerBase
    {       
        private readonly ILogger<CryptoController> _logger;

        public CryptoController(ILogger<CryptoController> logger)
        {
            _logger = logger;
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
                        file.CopyToAsync(stream);                        
                    }
                    FileInfo fi = new FileInfo(filePath);                        
                    int i = 0;
                    Dictionary<string, int> columns = new Dictionary<string, int>();    
                    using(var reader = new StreamReader(filePath))
                    {                                
                        while (!reader.EndOfStream)
                        { 
                            var line = reader.ReadLine();
                            //Logger.Log(line);
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
                                            // Logger.Log(value);
                                            // Logger.Log(property.Name);
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
                return transactions;
            }     
            catch(Exception e)  {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);    
                return null;                          
            }
        }
        private static string API_KEY = "9f6f5baf-d671-4815-8b42-7766f3036d01";               

        [HttpGet]
        public string Get()
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest");
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = "eth";
            URL.Query = queryString.ToString();
            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");      
            return client.DownloadString(URL.ToString());                    
        }
    }
}
