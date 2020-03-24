using Campus.DocumentValidator;
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

            //驗證規則
            FactoryProvider.FieldFactory.Add(new BodyTmperatureFactory());

            FISCA.Permission.FeatureAce UserPermission;
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.學生體溫紀錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<StudentTemperatureItem>());

            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.教師體溫紀錄];
            if (UserPermission.Editable || UserPermission.Viewable)
                K12.Presentation.NLDPanels.Teacher.AddDetailBulider(new FISCA.Presentation.DetailBulider<TeacherTemperatureItem>());

            RibbonBarItem Results = FISCA.Presentation.MotherForm.RibbonBarItems["學務作業", "日常作業"];
            Results["學生體溫記錄"].Size = RibbonBarButton.MenuButtonSize.Large;
            Results["學生體溫記錄"].Image = Properties.Resources.barcode_search_128;
            Results["學生體溫記錄"].Enable = Permissions.登錄學生體溫權限;
            Results["學生體溫記錄"].Click += delegate
            {
                SelectStudentForm ssf = new SelectStudentForm();
                ssf.ShowDialog();
            };

            Results = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"];
            Results["匯出"]["其它相關匯出"]["匯出學生體溫紀錄"].Enable = Permissions.匯出學生體溫紀錄權限;
            Results["匯出"]["其它相關匯出"]["匯出學生體溫紀錄"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ExportBodyTemprtature();
                ExportStudentV2 wizard = new ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            Results = FISCA.Presentation.MotherForm.RibbonBarItems["學生", "資料統計"];
            Results["匯入"]["其它相關匯入"]["匯入學生體溫紀錄"].Enable = Permissions.匯出學生體溫紀錄權限;
            Results["匯入"]["其它相關匯入"]["匯入學生體溫紀錄"].Click += delegate
            {
                new ImportBodyTemprtature().Execute();
            };

            //Results = K12.Presentation.NLDPanels.Teacher.RibbonBarItems["日常"];
            //Results["教師體溫記錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //Results["教師體溫記錄"].Enable = Permissions.登錄教師體溫權限;
            //Results["教師體溫記錄"].Click += delegate
            //{
            //    if (K12.Presentation.NLDPanels.Teacher.SelectedSource.Count == 1)
            //    {
            //        string teacherID = K12.Presentation.NLDPanels.Teacher.SelectedSource[0];
            //        EditTmperatureForm editor = new EditTmperatureForm(tool.BodyState.Teacher, int.Parse(teacherID));
            //        editor.ShowDialog();
            //    }
            //    else
            //    {
            //        MsgBox.Show("請選擇教師");
            //    }
            //};

            //列印學生體溫登錄表
            //Excel




            Catalog item = RoleAclSource.Instance["學生"]["資料項目"];
            item.Add(new FISCA.Permission.DetailItemFeature(Permissions.學生體溫紀錄, "學生體溫紀錄"));

            item = RoleAclSource.Instance["教師"]["資料項目"];
            item.Add(new FISCA.Permission.DetailItemFeature(Permissions.教師體溫紀錄, "教師體溫"));

            Catalog ribbon = RoleAclSource.Instance["學務作業"]["功能按鈕"];
            ribbon.Add(new RibbonFeature(Permissions.登錄學生體溫, "登錄學生體溫"));

            ribbon = RoleAclSource.Instance["學生"]["匯出/匯入"];
            ribbon.Add(new RibbonFeature(Permissions.匯出學生體溫紀錄, "匯出學生體溫紀錄"));
            ribbon.Add(new RibbonFeature(Permissions.匯入學生體溫紀錄, "匯入學生體溫紀錄"));
        }
    }
}
