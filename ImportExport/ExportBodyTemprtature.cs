using K12.Data;
using SmartSchool.API.PlugIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyTemperature
{
    class ExportBodyTemprtature : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public ExportBodyTemprtature()
        {
            this.Image = null;
            this.Text = "匯出學生體溫紀錄";
        }

        //覆寫
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            List<BodyTmperature> PersonList = tool._A.Select<BodyTmperature>("ref_obj_id in ('" + string.Join("','", K12.Presentation.NLDPanels.Student.SelectedSource) + "')");
            PersonList.Sort(SortSchool);

            Dictionary<string, string> StudentNumberByID = new Dictionary<string, string>();
            foreach (StudentRecord stud in K12.Data.Student.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource))
            {
                if (!StudentNumberByID.ContainsKey(stud.ID))
                {
                    StudentNumberByID.Add(stud.ID, stud.StudentNumber);
                }

            }


            wizard.ExportableFields.AddRange("學生系統編號", "學號", "對象", "體溫", "量測方式", "類別", "地點", "量測日期", "備註");

            wizard.ExportPackage += (sender, e) =>
            {
                for (int i = 0; i < PersonList.Count; i++)
                {
                    RowData row = new RowData();
                    row.ID = PersonList[i].UID;

                    foreach (string field in e.ExportFields)
                    {
                        if (wizard.ExportableFields.Contains(field))
                        {
                            switch (field)
                            {
                                case "學生系統編號": row.Add(field, "" + PersonList[i].ObjID); break;
                                case "學號":
                                    if (StudentNumberByID.ContainsKey(PersonList[i].ObjID.ToString()))
                                        row.Add(field, "" + StudentNumberByID[PersonList[i].ObjID.ToString()]);
                                    else
                                        row.Add(field, "");
                                    break;
                                case "對象": row.Add(field, "" + PersonList[i].BodyTag); break;
                                case "體溫": row.Add(field, "" + PersonList[i].BodyTemperature); break;
                                case "量測方式": row.Add(field, "" + PersonList[i].MeasurementMethod); break;
                                case "類別": row.Add(field, "" + PersonList[i].Category); break;
                                case "地點": row.Add(field, "" + PersonList[i].Location); break;
                                case "量測日期": row.Add(field, "" + PersonList[i].OccurDate.ToString("yyyy/MM/dd")); break;
                                case "備註": row.Add(field, "" + PersonList[i].Remark); break;
                            }
                        }
                    }

                    e.Items.Add(row);
                }
            };
        }

        private int SortSchool(BodyTmperature cpr1, BodyTmperature cpr2)
        {
            return cpr1.OccurDate.CompareTo(cpr2.OccurDate);
        }
    }
}
