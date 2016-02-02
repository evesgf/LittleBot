using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZHBot
{
    public class ZHHelper
    {
        /// <summary>
        /// 组装formdata
        /// </summary>
        /// <param name="email">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="xsrf">Lgin页面的xsrf</param>
        /// <param name="remember"></param>
        /// <returns></returns>
        public static string GetPost(string email,string password,string xsrf,bool remember=true)
        {
            return$"_xsrf={xsrf}&password={password}&remember_me={remember}&email={email}";
        }

        /// <summary>
        /// 从Login页面的xsrf标签获取参数
        /// </summary>
        /// <param name="loginUrl">用户登录页面地址</param>
        /// <returns></returns>
        public static string Get_xsrf(string loginUrl)
        {
            var re=HttpHelper.GetHtml(loginUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(re);
            return doc.DocumentNode.SelectSingleNode("/html/body/input").Attributes
                ["value"].Value;
        }

        /// <summary>
        /// 组装知乎的header头
        /// </summary>
        /// <returns></returns>
        public static HttpHeader GetHeader()
        {
            HttpHeader header = new HttpHeader();
            header.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            header.acceptEncoding = "gzip, deflate, sdch";
            header.acceptLaguage = "en-US,en;q=0.8,zh-CN;q=0.6,zh;q=0.4,zh-TW;q=0.2";
            header.connection = "keep-alive";
            header.host = "www.zhihu.com";
            header.userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36";
            header.referer = "http://www.zhihu.com/";
            return header;
        }
    }
}
