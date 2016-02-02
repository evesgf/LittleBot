using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ZHBot
{
    /// <summary>
    /// 用于抓取网页
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 获取CooKie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postdata"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCooKie(string loginUrl, string postdata, HttpHeader header)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            CookieContainer cc = new CookieContainer();
            request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.Method = "POST";
            request.ContentType = header.contentType;
            byte[] postdatabyte = Encoding.UTF8.GetBytes(postdata);
            request.ContentLength = postdatabyte.Length;
            request.AllowAutoRedirect = false;
            request.CookieContainer = cc;
            request.KeepAlive = true;

            //提交请求
            Stream stream;
            stream = request.GetRequestStream();
            stream.Write(postdatabyte, 0, postdatabyte.Length);
            stream.Close();

            //接收响应
            response = (HttpWebResponse)request.GetResponse();
            response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

            CookieCollection cook = response.Cookies;
            //Cookie字符串格式
            string strcrook = request.CookieContainer.GetCookieHeader(request.RequestUri);

            return cc;
        }

        public static string GetHtml(string getUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
            WebResponse response = request.GetResponse();
            Stream resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream,Encoding.UTF8);
            string html = sr.ReadToEnd();
            sr.Close();
            response.Close();
            request.Abort();
            resStream.Close();

            return html;
        }

        /// <summary>
        /// 带Cookie获取html
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static string GetHtml(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(getUrl);
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = header.contentType;
            //httpWebRequest.ServicePoint.ConnectionLimit = header.maxTry;
            httpWebRequest.Referer = getUrl;
            httpWebRequest.Accept = header.accept;
            httpWebRequest.UserAgent = header.userAgent;
            httpWebRequest.Method = "GET";
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            string html = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();
            return html;
        }
    }

    public class HttpHeader
    {
        public string contentType { get; set; }

        public string accept { get; set; }

        public string acceptEncoding { get; set; }

        public string acceptLaguage { get; set; }

        public string connection { get; set; }

        public string host { get; set; }

        public string referer { get; set; }

        public string userAgent { get; set; }

        public string method { get; set; }

        public int maxTry { get; set; }
    }
}
