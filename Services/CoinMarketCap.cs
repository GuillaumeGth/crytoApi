using System.Web;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
namespace api.Services{
    public class CoinMarketCap {
        private static string API_KEY = "9f6f5baf-d671-4815-8b42-7766f3036d01";   
        public static string Call(
            CoinMarketCapRoutes route,
            Dictionary<string, string> query = null){
            try{                
                Logger.Log(route.ToString());
                var enumType = typeof(CoinMarketCapRoutes);
                var memberInfos = 
                enumType.GetMember(route.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => 
                m.DeclaringType == enumType);
                var valueAttributes = 
                enumValueMemberInfo.GetCustomAttributes(typeof(ApiRoute), false);
                var path = ((ApiRoute)valueAttributes[0]).Route;                
                string url = $"https://pro-api.coinmarketcap.com/{path}";                
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