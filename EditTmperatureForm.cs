using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BodyTemperature
{
    public partial class EditTmperatureForm : BaseForm
    {
        public tool.BodyState _state;
        int _ref_id = 0;

        StudentRecord student;
        TeacherRecord teacher;
        public BodyTmperature _BT;

        public EditTmperatureForm(tool.BodyState state, int ref_id)
        {
            InitializeComponent();

            _state = state;
            _ref_id = ref_id;
            dateTimeInput1.Value = DateTime.Now;
            if (_ref_id == 0)
            {
                btnSave.Enabled = false;
            }
        }

        public EditTmperatureForm(tool.BodyState state, int ref_id, BodyTmperature BT)
        {
            InitializeComponent();

            _state = state;
            _ref_id = ref_id;
            _BT = BT;
            dateTimeInput1.Value = DateTime.Now;

            BindData();

            if (_ref_id == 0)
            {
                btnSave.Enabled = false;
            }
        }

        private void StudentTmperatureForm_Load(object sender, EventArgs e)
        {
            //查詢分類
            DataTable dt = tool._Q.Select(@"select category from $body_temperature.student_tmperature group by category ");
            List<string> list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string category = "" + row["category"];
                list.Add(category);
            }

            cbCategory.Items.AddRange(list.ToArray());

            //查詢地點
            dt = tool._Q.Select(@"select location from $body_temperature.student_tmperature group by location ");
            list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string location = "" + row["location"];
                list.Add(location);
            }

            cbLocation.Items.AddRange(list.ToArray());

            if (_state == tool.BodyState.Student)
            {
                student = K12.Data.Student.SelectByID(_ref_id.ToString());
                lbHelp.Text = string.Format("班級「{0}」座號「{1}」姓名「{2}」", student.RefClassID != "" ? student.Class.Name : "", student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "", student.Name);
            }
            else
            {
                teacher = K12.Data.Teacher.SelectByID(_ref_id.ToString());
                lbHelp.Text = string.Format("姓名「{0}」暱稱「{1}」", teacher.Name, teacher.Nickname);
            }


        }

        private void BindData()
        {
            tbTmperature.Text = _BT.BodyTemperature.ToString();
            cbCategory.Text = _BT.Category;
            if (_BT.MeasurementMethod == "額溫")
                checkBoxX1.Checked = true;

            cbLocation.Text = _BT.Location;
            tbRemark.Text = _BT.Remark;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Check())
            {
                BodyTmperature bt = new BodyTmperature();

                if (_BT != null)
                {
                    bt = _BT;
                }
                else
                {
                    bt.ObjID = _ref_id;
                    bt.BodyTag = _state.ToString();
                }

                bt.OccurDate = dateTimeInput1.Value;
                bt.BodyTemperature = double.Parse(tbTmperature.Text);
                bt.Category = cbCategory.Text;

                if (checkBoxX1.Checked)
                    bt.MeasurementMethod = "額溫";
                else
                    bt.MeasurementMethod = "耳溫";

                bt.Location = cbLocation.Text;


                bt.Remark = tbRemark.Text;

                bt.Save();

                _BT = bt;

                StringBuilder sb_log = new StringBuilder();
                sb_log.AppendLine(lbHelp.Text);
                sb_log.AppendLine(string.Format("體溫「{0}」", bt.BodyTemperature));
                sb_log.AppendLine(string.Format("量測方式「{0}」", bt.MeasurementMethod));
                sb_log.AppendLine(string.Format("分類「{0}」", bt.Category));
                sb_log.AppendLine(string.Format("地點「{0}」", bt.Location));
                sb_log.AppendLine(string.Format("備註「{0}」", bt.Remark));
                if (_state == tool.BodyState.Student)
                {
                    FISCA.LogAgent.ApplicationLog.Log("體溫登錄", "新增", "student", student.ID, sb_log.ToString());
                }
                else
                {
                    FISCA.LogAgent.ApplicationLog.Log("體溫登錄", "新增", "teacher", teacher.ID, sb_log.ToString());
                }

                if (_state == tool.BodyState.Student)
                {
                    if (tool.CheckFeature(tool.URL學生體溫記錄))
                    {
                        FISCA.Features.Invoke(tool.URL學生體溫記錄);
                    }
                }
                else
                {
                    if (tool.CheckFeature(tool.URL教師體溫記錄))
                    {
                        FISCA.Features.Invoke(tool.URL教師體溫記錄);
                    }
                }


                MsgBox.Show("儲存成功");

                this.Close();

            }
            else
            {
                MsgBox.Show("請修正錯誤");
            }
        }

        /// <summary>
        /// 檢查輸入的體溫是否為數字
        /// </summary>
        private bool Check()
        {
            //檢查輸入的體溫是否為數字
            if (errorProvider1.GetError(tbTmperature) == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbTmperature_TextChanged(object sender, EventArgs e)
        {
            double x = 0;
            if (!double.TryParse(tbTmperature.Text, out x))
            {
                errorProvider1.SetError(tbTmperature, "請輸入數字");
            }
            else
            {
                if (x >= 37)
                    tbTmperature.ForeColor = Color.Red;
                else
                    tbTmperature.ForeColor = Color.Black;

                errorProvider1.SetError(tbTmperature, "");
            }
        }
    }
}
