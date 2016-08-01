using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using JBWebappLibrary.Enum;

namespace JBWebappLibrary.Handlers
{
    public static class HttpRequestHelper
    {
        private static readonly string mobileUA = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4";

        public static async Task<string> Request(string url,
            CookieContainer cookieJar = null,
            bool fakeMobileUA = false,
            bool noCache = false,
            RequestMethod method = RequestMethod.Get,
            Dictionary<string, string> data = null,
            Dictionary<string, string> headers = null,
            bool removeLineBreak = false,
            HttpContent content = null)
        {
            #region DATA

            if (data != null)
            {
                StringBuilder postData = new StringBuilder();
                foreach (var d in data)
                {
                    postData.Append("&");
                    postData.Append(HttpUtility.UrlEncode(d.Key));
                    postData.Append("=");
                    postData.Append(HttpUtility.UrlEncode(d.Value));
                }
                if (url.Contains("?"))
                {
                    url = url += postData;
                }
                else
                {
                    url = url + "?" + postData;
                }
            }

            #endregion

            #region CACHE


            if (!noCache && HttpContext.Current.Cache["Get_" + (fakeMobileUA ? "M_" : "D_") + url] != null)
            {
                var responseString = HttpContext.Current.Cache["Get_" + (fakeMobileUA ? "M_" : "D_") + url].ToString();
                return removeLineBreak
                    ? StripWhiteSpaces(responseString)
                    : responseString;
            }

            #endregion

            var handler = new HttpClientHandler();

            if (cookieJar != null)
            {
                handler.CookieContainer = cookieJar;
            }

            var client = new HttpClient(handler);

            if (fakeMobileUA)
            {
                client.DefaultRequestHeaders.Add("User-Agent",mobileUA);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            try
            {
                Task<HttpResponseMessage> result;
                switch (method)
                {
                    case RequestMethod.Get:
                        result = client.GetAsync(url);
                        break;
                    case RequestMethod.Post:
                        result = client.PostAsync(url, content);
                        break;
                    case RequestMethod.Put:
                        result = client.PutAsync(url, content);
                        break;
                    case RequestMethod.Delete:
                        result = client.DeleteAsync(url);
                        break;
                    default:
                        throw new ArgumentException("Invalid Argument Method");

                }
                HttpResponseMessage response = await result;

                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                if (!noCache)
                {

                    HttpContext.Current.Cache["Get_" + (fakeMobileUA ? "M_" : "D_") + url] = responseString;
                    HttpContext.Current.Cache.Insert("Get_" + (fakeMobileUA ? "M_" : "D_") + url,
                        responseString,
                        null,
                        DateTime.Now.AddHours(2),
                        Cache.NoSlidingExpiration);
                }
                return removeLineBreak
                    ? StripWhiteSpaces(responseString)
                    : responseString;
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    Debug.WriteLine(errorText);
                }
                throw;
            }
        }


        public static string StripWhiteSpaces(string input)
        {
            string s = input.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            Regex r = new Regex(@">\s+<");
            return r.Replace(s, "><");
        }
    }
}
