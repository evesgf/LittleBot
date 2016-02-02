using LittleBot.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ZHService zhs = new ZHService();
            zhs.StartZHBot();

            Console.ReadKey(true);
        }
    }
}
