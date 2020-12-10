using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;

namespace BodyTemperature
{

    [FISCA.Permission.FeatureCode("BodyTemperature.Teacher.Item", "教師體溫記錄")]
    internal partial class TeacherTemperatureItem : DetailContentBase
    {
        BackgroundWorker BGW = new BackgroundWorker();
        bool BkWBool = false;
        private List<BodyTmperature> _records = new List<BodyTmperature>();
        TeacherRecord teacher;

        public TeacherTemperatureItem()
        {
            InitializeComponent();

            this.Group = "教師體溫記錄";

            BGW.DoWork += BGW_DoWork;
            BGW.RunWorkerCompleted += BGW_RunWorkerCompleted;

            if (!tool.CheckFeature(tool.URL教師體溫記錄))
            {
                FISCA.Features.Register(tool.URL教師體溫記錄, arg =>
                {
                    BGW.RunWorkerAsync();
                });
            }

        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.PrimaryKey != "")
            {
                teacher = K12.Data.Teacher.SelectByID(this.PrimaryKey);

                _records.Clear();
                _records = tool._A.Select<BodyTmperature>(string.Format("body_tag='Teacher' and ref_obj_id='{0}'", this.PrimaryKey));
            }
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool)
            {
                BkWBool = false;
                BGW.RunWorkerAsync();
                return;
            }

            refreshUIData();

            this.Loading = false;
        }

        private void refreshUIData()
        {
            if (!this.SaveButtonVisible && !this.CancelButtonVisible && this.PrimaryKey.Contains(PrimaryKey))
            {
                //從Cache Manager 找到該學生的懲戒記錄，並更新到畫面上。
                this.listView.Items.Clear();

                _records.Sort(new Comparison<BodyTmperature>(DateTimeComparer));

                foreach (BodyTmperature record in _records)
                {
                    #region 填值


                    ListViewItem subItem = new ListViewItem(record.OccurDate.ToString("yyyy/MM/dd HH:mm"));
                    subItem.SubItems.Add(record.BodyTemperature.ToString());
                    subItem.SubItems.Add(record.MeasurementMethod);
                    subItem.SubItems.Add(record.Category);
                    subItem.SubItems.Add(record.Location);
                    subItem.SubItems.Add(record.Remark);
                    //將資料加入ListView
                    subItem.Tag = record;

                    if (record.MeasurementMethod == "額溫")
                    {
                        if (record.BodyTemperature >= 37.5)
                        {
                            foreach (ListViewItem.ListViewSubItem sub in subItem.SubItems)
                            {
                                sub.BackColor = Color.LightCoral;
                            }
                        }
                    }
                    else if (record.MeasurementMethod == "耳溫")
                    {
                        if (record.BodyTemperature >= 38)
                        {
                            foreach (ListViewItem.ListViewSubItem sub in subItem.SubItems)
                            {
                                sub.BackColor = Color.LightCoral;
                            }
                        }
                    }

                    listView.Items.Add(subItem);

                    #endregion

                }
                //this.Loading = false;
                this.CancelButtonVisible = false;
                this.SaveButtonVisible = false;
                this.ContentValidated = true;
            }

            this.Loading = false;   //畫面就回覆資料已下載完成的畫面
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            this.Loading = true;

            if (BGW.IsBusy)
            {
                BkWBool = true;
            }
            else
            {
                BGW.RunWorkerAsync();
            }
        }

        private int DateTimeComparer(BodyTmperature x, BodyTmperature y)
        {
            return y.OccurDate.CompareTo(x.OccurDate);

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            EditTmperatureForm editor = new EditTmperatureForm(tool.BodyState.Teacher, int.Parse(this.PrimaryKey));
            editor.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                EditTmperatureForm editor = new EditTmperatureForm(tool.BodyState.Teacher, int.Parse(this.PrimaryKey), (BodyTmperature)listView.SelectedItems[0].Tag);
                editor.ShowDialog();
            }
            else
            {
                MsgBox.Show("請選擇一筆資料");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DialogResult dr = MsgBox.Show("要刪除所選擇紀錄?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    StringBuilder sb_log = new StringBuilder();
                    sb_log.AppendLine("刪除體溫紀錄：");
                    sb_log.AppendLine(string.Format("姓名「{0}」暱稱「{1}」", teacher.Name, teacher.Nickname));

                    List<BodyTmperature> btlist = new List<BodyTmperature>();
                    foreach (ListViewItem item in listView.SelectedItems)
                    {
                        BodyTmperature bt = (BodyTmperature)item.Tag;
                        btlist.Add(bt);

                        sb_log.AppendLine(string.Format("日期「{0}」", bt.OccurDate.ToString("yyyy/MM/dd HH:mm")));
                        sb_log.AppendLine(string.Format("體溫「{0}」", bt.BodyTemperature));
                        sb_log.AppendLine(string.Format("量測方式「{0}」", bt.MeasurementMethod));
                        sb_log.AppendLine(string.Format("分類「{0}」", bt.Category));
                        sb_log.AppendLine(string.Format("地點「{0}」", bt.Location));
                        sb_log.AppendLine(string.Format("備註「{0}」", bt.Remark));
                        sb_log.AppendLine("");
                    }

                    tool._A.DeletedValues(btlist);

                    FISCA.Features.Invoke(tool.URL教師體溫記錄);
                    FISCA.LogAgent.ApplicationLog.Log("體溫登錄", "刪除", "teacher", teacher.ID, sb_log.ToString());
                    MsgBox.Show("刪除成功");
                }
                else
                {
                    MsgBox.Show("已取消");
                }
            }
            else
            {
                MsgBox.Show("請選擇紀錄");
            }
        }
    }
}
