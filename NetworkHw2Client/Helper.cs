using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkHw2Client
{
    public static class Helper
    {
        public static void PrintHeader(this string txt)
        {
            string title = txt;
            int innerWidth = 30;
            int padding = (innerWidth - title.Length) / 2;
            string centered = "||" + new string(' ', padding) + title + new string(' ', innerWidth - padding - title.Length) + "||";
            string border = "||" + new string('=', innerWidth) + "||";

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(border);
            Console.WriteLine(centered);
            Console.ResetColor();

        }
        public static bool IsJson(string result)
        {
            if (result.Trim().StartsWith('{') || result.Trim().StartsWith('[') &&
                result.Trim().EndsWith('}') || result.Trim().EndsWith(']'))
                return true;
            return false;
        }
    }
}
