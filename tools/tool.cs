using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyTemperature
{
    public class tool
    {
        public static FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();

        public static FISCA.Data.QueryHelper _Q = new FISCA.Data.QueryHelper();

        public enum BodyState { Student, Teacher }

        public static string URL學生體溫記錄 = "ischool/日常系統/學生/學生體溫記錄";

        public static string URL教師體溫記錄 = "ischool/日常系統/教師/教師體溫記錄";

        public static bool CheckFeature(string Message)
        {
            foreach (FISCA.IFeature each in FISCA.Features.EnumerateFeatures())
            {
                if (Message == each.Code)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
