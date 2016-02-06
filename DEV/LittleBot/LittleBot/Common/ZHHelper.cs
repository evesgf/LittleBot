using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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

        /// <summary>
        /// 通过问题链接读取问题信息
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        public void GetQuesionInfo(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            QuestionModel qm=new QuestionModel();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(getUrl, cookieContainer, header));

            //读取问题信息
            qm.Qname= doc.DocumentNode.SelectSingleNode("//*[@id='zh-question-title']/h2/text()").InnerText;

            //标签组循环获取
            var tipLists = doc.DocumentNode.SelectNodes("//*[@class='zm-item-tag']");
            foreach (var tip in tipLists)
            {
                var qtip = tip.InnerText;
                qm.Qtip.Add(qtip);
            }

            //循环读取回答信息

            //存储
        }
    }

    /// <summary>
    /// 问题属性类
    /// </summary>
    public class QuestionModel
    {
        /// <summary>
        /// 问题名
        /// </summary>
        public string Qname { get; set; }
        /// <summary>
        /// 问题标签组
        /// </summary>
        public List<string> Qtip { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int FollowerCount { get; set; }
        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime LastEditTime { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int WatchCount { get; set; }
        /// <summary>
        /// 相关话题关注者
        /// </summary>
        public int CorrelationerCount { get; set; }
    }

    /// <summary>
    /// 回答属性类
    /// </summary>
    public class AnswerModel
    {
        /// <summary>
        /// 赞同数
        /// </summary>
        public int AgreeCount { get; set; }
        /// <summary>
        /// 回答者昵称
        /// </summary>
        public string AnswerPeople { get; set; }
        /// <summary>
        /// 最后编辑时间
        /// </summary>
        public DateTime LastEditTime { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }
    }
}
