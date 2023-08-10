using System;
using System.IO;
using System.Threading.Tasks;

namespace CoreTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string token = await Utils.GetMicrosoftToken();
            TimeZoneInfo ChinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            DateTime ChinaTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, ChinaTimeZone);
            string formattedChinaTime = ChinaTime.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"update_time={formattedChinaTime} (UTC+8)");
            Console.WriteLine($"user_code={token}");
            File.WriteAllText("token.cfg", $"update_time={formattedChinaTime} (UTC+8)\nuser_code={token}");
        }
    }
}
