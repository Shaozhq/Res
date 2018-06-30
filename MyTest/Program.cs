using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyTest
{
    /// <summary>
    /// 自测
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            var x = DateTime.Now.ToString("yyyy-MM-dd");
            Console.WriteLine(x);
            var str = "999123";
            var w = Convert.ToInt32(str) + 1;
            Console.WriteLine(w.ToString().PadLeft(6,'0'));

            var qqq = new List<string>();
            qqq.Add("123");
            //qqq.Add("456");
            //qqq.Add("789");
            Console.WriteLine(string.Join(" AND ",qqq));


        }
    }
}
