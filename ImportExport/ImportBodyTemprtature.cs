using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.Import;
using FISCA.Presentation.Controls;
using Campus.DocumentValidator;
using K12.Data;
using FISCA.DSAUtil;
using System.Xml;
using System.Data;

namespace BodyTemperature
{
    class ImportBodyTemprtature : ImportWizard
    {
        //設定檔
        private ImportOption mOption;

        /// <summary>
        /// 學生紀錄 By 學號
        /// </summary>
        public Dictionary<string, StudRecord> StudentByNumberDic { get; set; }

        /// <summary>
        /// 學生紀錄 By StudentID
        /// </summary>
        public Dictionary<string, StudRecord> StudentDic { get; set; }

        /// <summary>
        /// 競賽項目 
        /// </summary>
        public Dictionary<string, BodyTmperature> BodyTmperatureDic { get; set; }

        List<string> student_numberList { get; set; }

        public override string GetValidateRule()
        {
            return Properties.Resources.ImportBodyTmperatureRule;
        }

        public override string Import(List<Campus.DocumentValidator.IRowStream> Rows)
        {
            if (mOption.Action == ImportAction.InsertOrUpdate)
            {
                List<BodyTmperature> InsertList = new List<BodyTmperature>();
                List<BodyTmperature> UpdateList = new List<BodyTmperature>();

                //學號:學生紀錄
                GetStudentByNumber(Rows);

                GetBodyTmperature();

                foreach (IRowStream Row in Rows)
                {
                    string studentNumber = Row.GetValue("學號");
                    string body_tag = Row.GetValue("對象");
                    string category = Row.GetValue("類別");
                    string measurement_method = Row.GetValue("量測方式");
                    string location = Row.GetValue("地點");
                    string remark = Row.GetValue("備註");

                    DateTime occurDate;
                    DateTime.TryParse("" + Row.GetValue("量測日期"), out occurDate);

                    double body_temperature;
                    double.TryParse("" + Row.GetValue("體溫"), out body_temperature);

                    //學生是否存在
                    if (StudentByNumberDic.ContainsKey(studentNumber))
                    {
                        StudRecord stud = StudentByNumberDic[studentNumber];

                        //學號 + 類別 + 日期
                        string name_key = studentNumber + "_" + category + "_" + occurDate.ToString("yyyy/MM/dd"); ;

                        if (BodyTmperatureDic.ContainsKey(name_key))
                        {
                            //更新,取得原檔
                            BodyTmperature body_tmperature = BodyTmperatureDic[name_key];

                            //20202/3/24 - 建立修改前的副本內容
                            body_tmperature.NowClone();

                            //類別
                            body_tmperature.BodyTag = "Student";
                            body_tmperature.Category = category;
                            body_tmperature.BodyTemperature = body_temperature;
                            body_tmperature.MeasurementMethod = measurement_method;
                            body_tmperature.Location = location;
                            body_tmperature.Remark = remark;


                            //證書日期
                            if (occurDate != null)
                                body_tmperature.OccurDate = occurDate;

                            UpdateList.Add(body_tmperature);
                        }
                        else
                        {

                            //新增
                            BodyTmperature body_tmperature = new BodyTmperature();
                            if (occurDate != null)
                                body_tmperature.OccurDate = occurDate;

                            body_tmperature.BodyTag = "Student";
                            body_tmperature.Category = category;
                            body_tmperature.BodyTemperature = body_temperature;
                            body_tmperature.MeasurementMethod = measurement_method;
                            body_tmperature.Location = location;
                            body_tmperature.Remark = remark;

                            body_tmperature.ObjID = int.Parse(stud.id);

                            InsertList.Add(body_tmperature);
                        }
                    }
                }

                if (InsertList.Count > 0)
                {
                    tool._A.InsertValues(InsertList);

                    StringBuilder sb_log_new = new StringBuilder();
                    sb_log_new.AppendLine("新增學生體溫紀錄");

                    foreach (BodyTmperature each in InsertList)
                    {
                        if (StudentDic.ContainsKey(each.ObjID.ToString()))
                        {
                            StudRecord stud = StudentDic[each.ObjID.ToString()];
                            sb_log_new.AppendLine(string.Format("班級「{0}」座號「{1}」學生「{2}」", stud.class_name, stud.seat_no, stud.name));
                            sb_log_new.AppendLine("對象「學生」");
                            sb_log_new.AppendLine("類別「" + each.Category + "」");
                            sb_log_new.AppendLine("量測日期「" + each.OccurDate.ToString("yyyy/MM/dd") + "」");

                            sb_log_new.AppendLine("體溫「" + each.BodyTemperature + "」");
                            sb_log_new.AppendLine("量測方式「" + each.MeasurementMethod + "」");
                            sb_log_new.AppendLine("地點「" + each.Location + "」");
                            sb_log_new.AppendLine("備註「" + each.Remark + "」");
                            sb_log_new.AppendLine("");
                        }
                    }
                    FISCA.LogAgent.ApplicationLog.Log("匯入學生體溫紀錄", "新增", sb_log_new.ToString());
                }

                if (UpdateList.Count > 0)
                {
                    tool._A.UpdateValues(UpdateList);

                    StringBuilder sb_log_update = new StringBuilder();
                    sb_log_update.AppendLine("更新學生體溫紀錄");
                    foreach (BodyTmperature each in UpdateList)
                    {
                        if (StudentDic.ContainsKey(each.ObjID.ToString()))
                        {
                            StudRecord stud = StudentDic[each.ObjID.ToString()];

                            StringBuilder sb_aing = new StringBuilder();
                            bool change = false;

                            sb_aing.AppendLine(string.Format("班級「{0}」座號「{1}」學生「{2}」", stud.class_name, stud.seat_no, stud.name));
                            sb_aing.AppendLine(string.Format("日期「{0}」", each.OccurDate.ToString("yyyy/MM/dd")));

                            if (each.bt.BodyTemperature != each.BodyTemperature)
                            {
                                change = true;
                                sb_aing.AppendLine("體溫紀錄由「" + each.bt.BodyTemperature + "」修改為「" + each.BodyTemperature + "」");
                            }

                            if (each.bt.Location != each.Location)
                            {
                                change = true;
                                sb_aing.AppendLine("地點由「" + each.bt.Location + "」修改為「" + each.Location + "」");
                            }

                            if (each.bt.Remark != each.Remark)
                            {
                                change = true;
                                sb_aing.AppendLine("備註由「" + each.bt.Remark + "」修改為「" + each.Remark + "」");
                            }

                            if (each.bt.MeasurementMethod != each.MeasurementMethod)
                            {
                                change = true;
                                sb_aing.AppendLine("量測方式由「" + each.bt.MeasurementMethod + "」修改為「" + each.MeasurementMethod + "」");
                            }

                            sb_aing.AppendLine("");

                            if (change)
                            {
                                sb_log_update.Append(sb_aing.ToString());
                            }
                        }
                    }
                    if (sb_log_update.ToString() != "更新學生體溫紀錄")
                        FISCA.LogAgent.ApplicationLog.Log("匯入學生體溫紀錄", "更新", sb_log_update.ToString());
                }
            }

            return "";
        }

        /// <summary>
        /// 取得體溫紀錄
        /// </summary>
        public void GetBodyTmperature()
        {
            BodyTmperatureDic = new Dictionary<string, BodyTmperature>();
            List<string> studentIDList = new List<string>();
            Dictionary<string, DataRow> studentDataRow = new Dictionary<string, DataRow>();

            //用學號取得學生ID
            //再由學生ID取得體溫紀錄
            DataTable dt = tool._Q.Select(string.Format(@"select id,student_number from student 
where student_number in ('{0}')", string.Join("','", student_numberList)));
            foreach (DataRow row in dt.Rows)
            {
                string id = "" + row["id"];
                if (!studentIDList.Contains(id))
                {
                    studentIDList.Add(id);
                }

                if (!studentDataRow.ContainsKey(id))
                {
                    studentDataRow.Add(id, row);
                }
            }

            //所有體溫紀錄
            List<BodyTmperature> CompetitionList = tool._A.Select<BodyTmperature>(string.Format("ref_obj_id in ('{0}') and body_tag='Student'", string.Join("','", studentIDList)));

            foreach (BodyTmperature each in CompetitionList)
            {
                //用學生ID取得物件
                if (studentDataRow.ContainsKey(each.ObjID.ToString()))
                {
                    DataRow row = studentDataRow[each.ObjID.ToString()];

                    string student_number = "" + row["student_number"];

                    string name = student_number + "_" + each.Category + "_" + each.OccurDate.ToString("yyyy/MM/dd");

                    //組合KEY - 學號 + 類別 + 日期
                    if (!BodyTmperatureDic.ContainsKey(each.ObjID.ToString()))
                    {
                        BodyTmperatureDic.Add(name, each);
                    }
                }
            }
        }

        private void GetStudentByNumber(List<IRowStream> Rows)
        {
            StudentByNumberDic = new Dictionary<string, StudRecord>();
            StudentDic = new Dictionary<string, StudRecord>();

            student_numberList = new List<string>();

            foreach (IRowStream Row in Rows)
            {
                string studentNumber = Row.GetValue("學號");
                if (studentNumber != "")
                {
                    if (!student_numberList.Contains(studentNumber))
                    {
                        student_numberList.Add(studentNumber);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(@"select student.id,student.name,student.student_number,class.class_name,student.seat_no 
from student 
left join class on class.id=student.ref_class_id
where student_number in ('{0}')", string.Join("','", student_numberList)));
            DataTable dt = tool._Q.Select(sb.ToString());
            foreach (DataRow row in dt.Rows)
            {
                StudRecord stud = new StudRecord(row);
                if (!StudentByNumberDic.ContainsKey(stud.student_number))
                {
                    StudentByNumberDic.Add(stud.student_number, stud);
                }

                if (!StudentDic.ContainsKey(stud.id))
                {
                    StudentDic.Add(stud.id, stud);
                }
            }

        }

        /// <summary>
        /// 準備資料
        /// </summary>
        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }

        public override ImportAction GetSupportActions()
        {
            //新增或更新
            return ImportAction.InsertOrUpdate;
        }
    }

    class StudRecord
    {
        public StudRecord(DataRow row)
        {
            id = "" + row["id"];
            name = "" + row["name"];
            student_number = "" + row["student_number"];
            class_name = "" + row["class_name"];
            seat_no = "" + row["seat_no"];
        }

        public string id { get; set; }
        public string name { get; set; }
        public string student_number { get; set; }
        public string class_name { get; set; }
        public string seat_no { get; set; }
    }
}
