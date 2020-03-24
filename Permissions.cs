using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyTemperature
{
    class Permissions
    {
        public static string 登錄學生體溫 { get { return "BodyTemperature.Student"; } }
        public static bool 登錄學生體溫權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[登錄學生體溫].Executable;
            }
        }

        public static string 登錄教師體溫 { get { return "BodyTemperature.Teacher"; } }
        public static bool 登錄教師體溫權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[登錄教師體溫].Executable;
            }
        }

        public static string 學生體溫紀錄 { get { return "BodyTemperature.Student.Item"; } }
        public static bool 學生體溫紀錄權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學生體溫紀錄].Executable;
            }
        }

        public static string 教師體溫紀錄 { get { return "BodyTemperature.Teacher.Item"; } }
        public static bool 紀錄體溫紀錄權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[學生體溫紀錄].Executable;
            }
        }

        public static string 匯出學生體溫紀錄 { get { return "BodyTemperature.Student.ExportBodyTemprtature"; } }
        public static bool 匯出學生體溫紀錄權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯出學生體溫紀錄].Executable;
            }
        }

        public static string 匯入學生體溫紀錄 { get { return "BodyTemperature.Student.ImportBodyTemprtature"; } }
        public static bool 匯入學生體溫紀錄權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[匯入學生體溫紀錄].Executable;
            }
        }
    }
}
