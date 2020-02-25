using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyTemperature
{
    public class Program
    {
        [MainMethod()]
        public static void Main()
        {
            FISCA.UDT.SchemaManager sch = new FISCA.UDT.SchemaManager(FISCA.Authentication.DSAServices.DefaultConnection);
            sch.SyncSchema(new BodyTmperature());

            FISCA.Permission.FeatureAce UserPermission;
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.學生體溫紀錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<StudentTemperatureItem>());

            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.教師體溫紀錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Teacher.AddDetailBulider(new FISCA.Presentation.DetailBulider<TeacherTemperatureItem>());

            RibbonBarItem Results = K12.Presentation.NLDPanels.Student.RibbonBarItems["日常"];
            Results["學生體溫記錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Results["學生體溫記錄"].Enable = Permissions.登錄學生體溫權限;
            Results["學生體溫記錄"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Student.SelectedSource.Count == 1)
                {
                    string studentID = K12.Presentation.NLDPanels.Student.SelectedSource[0];
                    EditTmperatureForm editor = new EditTmperatureForm(tool.BodyState.Student, int.Parse(studentID));
                    editor.ShowDialog();
                }
                else
                {
                    MsgBox.Show("請選擇學生");
                }
            };

            Results = K12.Presentation.NLDPanels.Teacher.RibbonBarItems["日常"];
            Results["教師體溫記錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Results["教師體溫記錄"].Enable = Permissions.登錄教師體溫權限;
            Results["教師體溫記錄"].Click += delegate
            {
                if (K12.Presentation.NLDPanels.Teacher.SelectedSource.Count == 1)
                {
                    string teacherID = K12.Presentation.NLDPanels.Teacher.SelectedSource[0];
                    EditTmperatureForm editor = new EditTmperatureForm(tool.BodyState.Teacher, int.Parse(teacherID));
                    editor.ShowDialog();
                }
                else
                {
                    MsgBox.Show("請選擇教師");
                }
            };

            //列印學生體溫登錄表
            //Excel

            //匯出體溫清單



            //匯入體溫清單
            //Key 日期 + 分類




            Catalog item = RoleAclSource.Instance["學生"]["資料項目"];
            item.Add(new FISCA.Permission.DetailItemFeature(Permissions.學生體溫紀錄, "學生體溫紀錄"));

            item = RoleAclSource.Instance["教師"]["資料項目"];
            item.Add(new FISCA.Permission.DetailItemFeature(Permissions.教師體溫紀錄, "教師體溫"));

            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.登錄學生體溫, "登錄學生體溫"));

            ribbon = RoleAclSource.Instance["教師"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.登錄教師體溫, "登錄教師體溫"));
        }
    }
}
