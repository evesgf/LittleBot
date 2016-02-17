using LittleBot.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            //ZHService zhs = new ZHService();
            //zhs.StartZHBot();

            LGService lgService=new LGService();
            lgService.StartZHBot();

            //输出时间
            sw2.Stop();
            TimeSpan ts2 = sw2.Elapsed;
            Console.WriteLine("总共花费{0}ms.", ts2.TotalMilliseconds);

            Console.ReadKey(true);
        }
    }
}
