using System.Web;
using System.Net;
using System;
using System.Collections.Generic;
namespace api.Services{
    public class CoinMarketCap {
        private static string API_KEY = "9f6f5baf-d671-4815-8b42-7766f3036d01";    
        public static string Call(
            string path, 
            Dictionary<string, string> query = null){
            try{
                string url = $"https://pro-api.coinmarketcap.com/{path}"; 
                Logger.Log(url);
                var URL = new UriBuilder(url);            
                if (query != null){
                    var queryString = HttpUtility.ParseQueryString(string.Empty);            
                    foreach(KeyValuePair<string, string> kvp in query){
                        queryString[kvp.Key] = kvp.Value;
                    }            
                    URL.Query = queryString.ToString();
                }            
                var client = new WebClient();
                client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
                client.Headers.Add("Accepts", "application/json");      
                return client.DownloadString(URL.ToString());  
            }
            catch(Exception e)  {
                Logger.Error(e.Message);
                Logger.Error(e.StackTrace);    
                return null;                          
            }            
        }
    }
}