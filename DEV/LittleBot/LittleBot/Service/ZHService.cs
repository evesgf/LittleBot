using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        DataTable AskDT=new DataTable("AskDT");
        DataTable AnswersDT=new DataTable("AnswersDT");

        public void StartZHBot()
        {
            //step1、自动登录
            AutoLogin();

            //setp2、获取个人主页信息
            GetMainPage(startUrl,cookie,ZHHelper.GetHeader());

            //setp3、获取提问列表
            GetAsksList();

            //setp4、获取回答列表
            StartGetAnswersList();

            //setp5、获取关注者列表

            //setp6、获取被关注者列表
        }

        /// <summary>
        /// 开始获取回答列表
        /// </summary>
        private void StartGetAnswersList()
        {
            //初始化表结构

            Console.WriteLine("================开始抓取回答列表==================");
            //递归获取回答列表
            GetAswersList(1);

        }

        /// <summary>
        /// 递归获取回答列表
        /// </summary>
        /// <param name="pageCount"></param>
        private void GetAswersList(int pageCount)
        {
            var url = startUrl + $"/answers?order_by=created&page={pageCount}";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(url, cookie, ZHHelper.GetHeader()));

            //获取当前页的问题列表
            var lists = doc.DocumentNode.SelectNodes("//*[@class='zm-item']");

            //遍历问题列表并存入AnswersDT
            for (var i = 1; i < lists.Count; i++)
            {
                //回答列表节点
                var title = doc.DocumentNode.SelectSingleNode($"//*[@id='zh-profile-answer-list']/div[{i}]/h2/a");
                //回答的问题链接
                var quesionLink = title.Attributes["href"].Value;
                //回答的问题题目
                var quesionName = title.InnerText;
                //赞同数
                var endorse = doc.DocumentNode.SelectSingleNode($"//*[@id='zh-profile-answer-list']/div[{i}]/div/div[1]/a").InnerText;

                Console.WriteLine($"quesionName:{quesionName} quesionLink:{quesionLink} endorse:{endorse}");
            }

            Console.WriteLine($"--------第 {pageCount} 页读取完毕---------");

            //如果当前页有20个回答则递归下一页
            if (lists.Count == 20)
                GetAswersList(pageCount + 1);
            else
                Console.WriteLine("================回答列表抓取结束==================");
        }

        /// <summary>
        /// 获取提问列表
        /// </summary>
        private void GetAsksList()
        {
            Console.WriteLine("================开始抓取提问列表==================");

            var url = startUrl + "/asks";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(url, cookie, ZHHelper.GetHeader()));

            var lists = doc.DocumentNode.SelectSingleNode("//*[@id='zh-profile-ask-list']").Descendants("Div");
            for (var i = 0; i < lists.Count()/5; i++)
            {
                var browse = doc.DocumentNode.SelectSingleNode($"//*[@id='zh-profile-ask-list']/div[{i+1}]/span/div[1]").InnerText;
                var quesion = doc.DocumentNode.SelectSingleNode($"//*[@id='zh-profile-ask-list']/div[{i + 1}]/div/h2/a").InnerText;
                var temp = doc.DocumentNode.SelectSingleNode($"//*[@id='zh-profile-ask-list']/div[{i + 1}]/div/div").InnerText;
                var list=Regex.Matches(temp, @"\d+(\.\d+)?").OfType<Match>().Select(t => t.Value).ToList();

                Console.WriteLine($"问题：{quesion} 浏览：{ browse} 回答：{list[0]} 关注：{list[1]}");
            }

            Console.WriteLine("================提问列表抓取结束==================");
        }

        /// <summary>
        /// 获取个人主页信息
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="header"></param>
        private void GetMainPage(string getUrl, CookieContainer cookieContainer, HttpHeader header)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HttpHelper.GetHtml(getUrl, cookieContainer, header));

            //昵称
            var name = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[1]/div[2]/span[1]").InnerText;
            //一句话描述
            var title = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[1]/div[2]/span[2]").InnerText;
            //坐标
            var pos = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[1]/span[1]/span[1]").InnerText;
            //行业
            var industry = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[1]/span[1]/span[2]").InnerText;
            //公司
            var company = doc.DocumentNode.SelectSingleNode("/html/body/div[3]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div/div[1]/div[2]/span[1]/span[1]").InnerText;
            //职位
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
