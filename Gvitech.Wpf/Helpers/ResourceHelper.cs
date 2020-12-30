using Mmc.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;

namespace Helpers
{
    public class TimeHelper
    {
        /// <summary>
        /// 花费时间计算
        /// </summary>
        /// <param name="startTime"></param>
        /// DateTime startTime = DateTime.Now;
        public static void ElapsedTime(DateTime startTime,string timerLog = "ElapsedTimeLog")
        {
            DateTime stopTime = DateTime.Now;
            TimeSpan elapsedTime = stopTime - startTime;
            Console.WriteLine("ElapsedTimeLog-{0}: {1}", timerLog, elapsedTime.Seconds);
        }
    }

    public class StrHelper
    {

        /// <summary>
        /// 字符串长度限制
        /// </summary>
        /// <param name="stringToSub"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string StrPadRight(string stringToSub, int length)
        {
            if (string.IsNullOrEmpty(stringToSub))
                return stringToSub;

            Regex regex = new Regex(@"[\u4e00-\u9fa5]+", RegexOptions.Compiled);

            char[] stringChar = stringToSub.ToCharArray();
            StringBuilder sb = new StringBuilder();
            int nLength = 0;
            for (int i = 0; i < stringChar.Length; i++)
            {
                if (regex.IsMatch((stringChar[i]).ToString()))
                    nLength += 2;
                else
                    nLength = nLength + 1;

                if (nLength <= length)
                    sb.Append(stringChar[i]);
                else
                    break;
            }
            if (sb.ToString() != stringToSub)
                sb.Append("...");

            return sb.ToString();
        }
    }


    public class ResourceHelper
    {       

        public static string FindKey(string keyName)
        {
            try
            {                  
                if (string.IsNullOrWhiteSpace(keyName)) return "";
                return (string)Application.Current.FindResource(keyName);
            }
            catch (Exception ex)
            {
                //todo 打印日志   
                SystemLog.Log(ex);
                return null;
            }
        }

        public static object FindResourceByKey(string keyName)
        {
            var resourceDict = Application.Current.Resources;
            if (resourceDict == null || resourceDict.Count == 0)
                return null;
            var result = Application.Current.TryFindResource(keyName);
            return result;
        }
    }
}
