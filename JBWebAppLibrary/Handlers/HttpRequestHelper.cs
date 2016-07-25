using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace JBWebappLibrary.Handlers
{
    public static class HttpRequestHelper
    {
        private static string mobileUA = "Mozilla/5.0 (iPhone; CPU iPhone OS 8_0 like Mac OS X) AppleWebKit/600.1.3 (KHTML, like Gecko) Version/8.0 Mobile/12A4345d Safari/600.1.4";

        public static string GET(string url, CookieContainer cookieJar = null, bool fakeMobileUA = false, bool noCache = false, bool post = false, Dictionary<string, string> data = null, Dictionary<string, string> headers = null)
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
                return HttpContext.Current.Cache["Get_" + (fakeMobileUA ? "M_" : "D_") + url].ToString();
            }

            #endregion

            Debug.WriteLine("REQUESTING: " + url);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookieJar != null)
            {
                request.CookieContainer = cookieJar;
            }
            if (fakeMobileUA)
            {
                request.UserAgent = mobileUA;
            }
            if (post)
            {
                request.Method = "POST";
            }

            request.Credentials = CredentialCache.DefaultCredentials;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var responseString = reader.ReadToEnd();
                    if (noCache) return responseString;

                    HttpContext.Current.Cache["Get_" + (fakeMobileUA ? "M_" : "D_") + url] = responseString;
                    HttpContext.Current.Cache.Insert("Get_" + (fakeMobileUA ? "M_" : "D_") + url,
                        responseString,
                        null,
                        DateTime.Now.AddHours(2),
                        Cache.NoSlidingExpiration);
                    return responseString;

                }
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
