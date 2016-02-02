using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZHBot;

namespace LittleBot.Service
{
    public class ZHService
    {
        string loginUrl = "http://www.zhihu.com/login/email";
        string startUrl = "https://www.zhihu.com/people/yang-ding-peng";
        int depth = 1;
        ZHHelper zh = new ZHHelper();
        CookieContainer cookie = new CookieContainer();

        public void StartZHBot()
        {
            //step1、自动登录
            AutoLogin();

            //setp2、获取个人主页信息
            GetMainPage(startUrl,cookie,ZHHelper.GetHeader());

            //setp3、获取提问列表
            GetAsksList();

            //setp4、获取回答列表

            //setp5、获取关注者列表

            //setp6、获取被关注者列表
        }

        private void GetAsksList()
        {
            var url = startUrl + "/asks";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(url, cookie, ZHHelper.GetHeader()));

            var lists = doc.DocumentNode.SelectSingleNode("//*[@id='zh-profile-ask-list']").Descendants("Div");
            for (int i = 0; i < lists.Count(); i++)
            {
                if (i % 3 == 0)
                {
                    var list = lists.ElementAt(i).Descendants("Div");
                    var count = list.ElementAt(0).InnerText;
                    var quesion = list.ElementAt(2).Descendants("a").First().InnerText;
                    var qcount = list.ElementAt(3).Descendants("#text").ElementAt(4);
                    var fcount = list.ElementAt(3).Descendants("#text").ElementAt(6);
                    var b = lists.ElementAt(i).InnerHtml;
                    Console.WriteLine(b);
                }
            }

            Console.WriteLine();
        }

        private void GetMainPage(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(getUrl, cookieContainer, header));

            var name = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[1]/div[2]/span[1]").InnerText;
            var title = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[1]/div[2]/span[2]").InnerText;
            var pos = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[1]/span[1]/span[1]").InnerText;
            var industry = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[1]/span[1]/span[2]").InnerText;
            var company = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[2]/span[1]/span[1]").InnerText;
            var post = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[2]/span[1]/span[2]").InnerText;

            Console.WriteLine($"姓名：{name} 说明：{title} \r\n地址：{pos} 行业：{industry} 公司：{company} 职位：{post}");
        }

        /// <summary>
        /// 自动登录
        /// </summary>
        private void AutoLogin()
        {
            Console.Write("userName:");
            string userName = Console.ReadLine();
            Console.Write("passWord:");
            string passWord = Console.ReadLine();

            var poststr = ZHHelper.GetPost(userName, passWord, ZHHelper.Get_xsrf(loginUrl));
            cookie = HttpHelper.GetCooKie(loginUrl, poststr, ZHHelper.GetHeader());

            Console.WriteLine("===========登录成功，正在读取数据============");
        }
    }
}
