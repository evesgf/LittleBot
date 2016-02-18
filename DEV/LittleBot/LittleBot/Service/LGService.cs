using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LittleBot.Common;
using Newtonsoft.Json.Linq;
using ZHBot;
using System.Data;
using System.IO;
using FileHandler;

namespace LittleBot.Service
{
    class LGService
    {
        private static string kd = ".net";
        private static string city = "上海";
        private static string px = "new";

        private int totalCount;
        private int totalPageCount;
        private int pageSize;

        static DataTable Dt = new DataTable();
        private const string Urlstr = "./AllFile";

        public void StartZHBot()
        {
            //URL拼接
            var encodecity = HttpHelper.UrlEncode(city);
            string starturl = $"http://www.lagou.com/jobs/positionAjax.json?px={px}&city={encodecity}&kd={kd}&pn=";

            //创建dt表头
            //创建表结构
            Dt.Columns.Add("positionName");
            Dt.Columns.Add("salary");
            Dt.Columns.Add("workYear");
            Dt.Columns.Add("companyName");
            Dt.Columns.Add("companyShortName");
            Dt.Columns.Add("companySize");
            Dt.Columns.Add("industryField");
            Dt.Columns.Add("workYfinanceStageear");
            //插入Title
            Dt.Rows.Add("岗位名", "岗位工资", "工作年限", "公司简称", "公司全称", "公司人数", "行业性质", "当前阶段");

            //获取totalCount等
            string reJson = HttpHelper.GetHtml(starturl+"1");
            var jobject = JObject.Parse(reJson);

            totalCount = jobject["content"]["totalCount"].Value<int>();
            totalPageCount= jobject["content"]["totalPageCount"].Value<int>();
            pageSize = jobject["content"]["pageSize"].Value<int>();

            for (int i = 1; i < totalPageCount; i++)
            {
                string pageJson = HttpHelper.GetHtml(starturl + i);
                var pageJobject = JObject.Parse(pageJson);
                var lists = pageJobject["content"]["result"];

                //开始循环抓取数据
                foreach (var lgListResult in lists)
                {
                    //读取岗位基本信息
                    //岗位名
                    var positionName = lgListResult["positionName"];
                    //岗位工资
                    var salary = lgListResult["salary"];
                    //工作年限
                    var workYear = lgListResult["workYear"];

                    //读取公司
                    //公司简称
                    var companyName = lgListResult["companyName"];
                    //公司全称
                    var companyShortName = lgListResult["companyShortName"];
                    //公司人数
                    var companySize = lgListResult["companySize"];
                    //行业性质
                    var industryField = lgListResult["industryField"];
                    //当前阶段
                    var workYfinanceStageear = lgListResult["financeStage"];

                    //存入dt
                    Dt.Rows.Add(positionName, salary, workYear, companyName, companyShortName, companySize, industryField, workYfinanceStageear);
                }
                Console.WriteLine($"=======第{i}页读取完成=======");
            }

            //存入Excel
            #region 插入EXcel

            //插入Excel
            ExcelHelper eh = new ExcelHelper();
            string path2 = Path.GetFullPath("./AllFile/context.xlsx");
            eh.CreateExcel(path2);
            eh.OpenWorkbook(path2);
            var temp = eh.SelectWorksheet("数据集") ?? eh.InsertWorksheet("数据集");

            Console.WriteLine("正在存入Excel，请稍等...");
            eh.WriteData(Dt, 1, 1);

            eh.SaveCloseWorkBook(path2);
            #endregion

            Console.WriteLine("======Excel写入完成======");
        }
    }
}
