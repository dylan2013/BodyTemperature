using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BodyTemperature
{
    public partial class SelectStudentForm : BaseForm
    {

        BackgroundWorker bgw_Select = new BackgroundWorker();
        BackgroundWorker bgw_Save = new BackgroundWorker();
        BackgroundWorker bgw_Load = new BackgroundWorker();
        string studentNumber;
        int studentRowIndex;

        int AllIndex = 0; //影響Row

        SuperStudent student { get; set; }

        Dictionary<string, SuperStudent> studentDic = new Dictionary<string, SuperStudent>();
        List<string> tmperatureList;
        List<string> categoryList;

        public SelectStudentForm()
        {
            InitializeComponent();

            bgw_Save.RunWorkerCompleted += Bgw_Save_RunWorkerCompleted;
            bgw_Save.DoWork += Bgw_Save_DoWork;

            bgw_Load.DoWork += Bgw_Load_DoWork;
            bgw_Load.RunWorkerCompleted += Bgw_Load_RunWorkerCompleted;
            bgw_Load.RunWorkerAsync();

            tbSelect.Enabled = false;
            tbTmperature.Enabled = false;
            btnSelect.Enabled = false;
            btnSave.Enabled = false;

            this.Text = "快速體溫登錄(資料下載中)";
            cbCategory.Enabled = false;
            cbLocation.Enabled = false;
            dataGridViewX1.Enabled = false;

            dateTimeInput1.Value = DateTime.Now;
        }

        private void Bgw_Load_DoWork(object sender, DoWorkEventArgs e)
        {
            categoryList = new List<string>();
            tmperatureList = new List<string>();
            studentDic = new Dictionary<string, SuperStudent>();
            //查詢分類
            DataTable dt = tool._Q.Select(@"select category from $body_temperature.student_tmperature group by category ");
            foreach (DataRow row in dt.Rows)
            {
                string category = "" + row["category"];
                categoryList.Add(category);
            }

            //查詢地點
            dt = tool._Q.Select(@"select location from $body_temperature.student_tmperature group by location ");
            foreach (DataRow row in dt.Rows)
            {
                string location = "" + row["location"];
                tmperatureList.Add(location);
            }

            //一般生
            dt = tool._Q.Select(string.Format(@"select student.id as student_id,student.name as student_name ,
student.student_number,student.freshman_photo,student.seat_no,class.class_name,class.id as ref_class_id
from student join class on student.ref_class_id=class.id where student.status='1'"));
            foreach (DataRow row in dt.Rows)
            {
                student = new SuperStudent(row);
                if (!studentDic.ContainsKey(student.StudentNumber))
                {
                    studentDic.Add(student.StudentNumber, student);
                }
            }
        }

        private void Bgw_Load_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Text = "快速體溫登錄";

            cbCategory.Enabled = true;
            cbLocation.Enabled = true;
            dataGridViewX1.Enabled = true;

            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    //類別
                    cbCategory.Items.AddRange(categoryList.ToArray());
                    cbCategory.Focus();

                    //地點
                    cbLocation.Items.AddRange(tmperatureList.ToArray());
                }
                else
                {
                    MsgBox.Show("發生錯誤:：\n" + e.Error.Message);
                }
            }
            else
            {
                MsgBox.Show("已取消");
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //查詢學號學生
            studentNumber = tbSelect.Text.Trim();
            if (!studentDic.ContainsKey(studentNumber))
            {
                if (!bgw_Select.IsBusy)
                {
                    bgw_Select.RunWorkerAsync();
                }
                else
                {
                    MsgBox.Show("資料取得中\n請稍後");
                }
            }
            else
            {
                student = studentDic[studentNumber];
                NextPer();
            }
        }

        private void NextPer()
        {
            lbHelp.Text = string.Format("班級「{0}」\n座號「{1}」\n姓名「{2}」", student.ClassName, student.SeatNO, student.StudentName);
            if (student.FreshmanPhoto != "")
            {
                byte[] arr = Convert.FromBase64String(student.FreshmanPhoto);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                pictureBox1.Image = bmp;
            }

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridViewX1);

            row.Cells[0].Value = student.ClassName;
            row.Cells[1].Value = student.SeatNO;
            row.Cells[2].Value = student.StudentName;
            row.Cells[3].Value = "";
            row.Cells[4].Value = "";

            dateTimeInput1.Value = DateTime.Now;

            studentRowIndex = dataGridViewX1.Rows.Add(row);

            tbTmperature.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (student != null)
            {
                if (!bgw_Save.IsBusy)
                {
                    BodyTmperature bt;
                    if (student.bt == null)
                    {
                        bt = new BodyTmperature();
                        bt.ObjID = int.Parse(student.StudentID);
                    }
                    else
                    {
                        bt = student.bt;
                    }

                    bt.BodyTag = tool.BodyState.Student.ToString();
                    bt.OccurDate = dateTimeInput1.Value;
                    bt.BodyTemperature = double.Parse(tbTmperature.Text);
                    bt.Category = cbCategory.Text;
                    if (checkBoxX1.Checked)
                        bt.MeasurementMethod = "額溫";
                    else
                        bt.MeasurementMethod = "耳溫";

                    bt.Location = cbLocation.Text;


                    bt.Remark = tbRemark.Text;

                    bgw_Save.RunWorkerAsync(bt);
                }
                else
                {
                    MsgBox.Show("資料取得中\n請稍後");
                }
            }
            else
            {
                MsgBox.Show("未查詢到學生");
            }
        }

        private void Bgw_Save_DoWork(object sender, DoWorkEventArgs e)
        {
            //開始儲存資料
            List<BodyTmperature> list = new List<BodyTmperature>();
            BodyTmperature bt = (BodyTmperature)e.Argument;
            list.Add(bt);

            List<string> listID = tool._A.SaveAll(list);
            List<BodyTmperature> btList = tool._A.Select<BodyTmperature>(listID);
            bt = btList[0];

            StringBuilder sb_log = new StringBuilder();
            sb_log.AppendLine(lbHelp.Text);
            sb_log.AppendLine(string.Format("體溫「{0}」", bt.BodyTemperature));
            sb_log.AppendLine(string.Format("量測方式「{0}」", bt.MeasurementMethod));
            sb_log.AppendLine(string.Format("分類「{0}」", bt.Category));
            sb_log.AppendLine(string.Format("地點「{0}」", bt.Location));
            sb_log.AppendLine(string.Format("備註「{0}」", bt.Remark));
            FISCA.LogAgent.ApplicationLog.Log("體溫登錄", "掃碼登錄", "student", student.StudentID, sb_log.ToString());

            e.Result = bt;
        }

        private void Bgw_Save_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    BodyTmperature bt = (BodyTmperature)e.Result;

                    DataGridViewRow row = dataGridViewX1.Rows[studentRowIndex];
                    row.Tag = student;

                    row.Cells[3].Value = tbTmperature.Text;
                    double checkTmp = double.Parse(tbTmperature.Text);
                    if (checkTmp >= 37)
                    {
                        row.Cells[3].Style.ForeColor = Color.Red;
                    }

                    if (student.bt != null)
                    {
                        row.Cells[4].Value = "體溫重掃修正";
                    }
                    else
                    {
                        //若是額溫,以37.5為發燒定義
                        if (checkBoxX1.Checked)
                        {
                            if (checkTmp >= 37.5)
                            {
                                row.Cells[3].Style.ForeColor = Color.Red;
                                row.Cells[4].Value = "體溫偏高,建議重新量測";
                            }
                            else
                            {
                                row.Cells[4].Value = "儲存成功";
                            }
                        }
                        else
                        {
                            if (checkTmp >= 38)
                            {
                                row.Cells[3].Style.ForeColor = Color.Red;
                                row.Cells[4].Value = "體溫偏高,建議重新量測";
                            }
                            else
                            {
                                row.Cells[4].Value = "儲存成功";
                            }
                        }

                    }

                    if (tool.CheckFeature(tool.URL學生體溫記錄))
                    {
                        FISCA.Features.Invoke(tool.URL學生體溫記錄);
                    }
                    student.bt = bt;

                    //恢復預設
                    pictureBox1.Image = null;
                    lbHelp.Text = "班級\n座號\n學生";
                    tbSelect.Text = "";
                    tbTmperature.Text = "";
                    student = null;

                    //回到學號
                    tbSelect.Focus();
                }
                else
                {
                    MsgBox.Show("發生錯誤:\n" + e.Error.Message);
                }
            }
            else
            {
                MsgBox.Show("已取消");
            }
        }

        private void tbSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSelect_Click(null, null);
            }
        }

        private void tbSave_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave_Click(null, null);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                //叫出資料進行修改
                if (dataGridViewX1.Rows[e.RowIndex].Tag != null)
                {
                    SuperStudent stud = (SuperStudent)dataGridViewX1.Rows[e.RowIndex].Tag;
                    EditTmperatureForm erf = new EditTmperatureForm(tool.BodyState.Student, int.Parse(stud.StudentID), stud.bt);
                    erf.ShowDialog();

                    //修正後
                    dataGridViewX1.Rows[e.RowIndex].Cells[3].Value = erf._BT.BodyTemperature;
                    dataGridViewX1.Rows[e.RowIndex].Cells[4].Value = "已手動修改";
                }
            }
        }

        private void tbTmperature_TextChanged(object sender, EventArgs e)
        {
            if (tbTmperature.Text != "")
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
            else
            {
                errorProvider1.SetError(tbTmperature, "");
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbCategory_TextChanged(object sender, EventArgs e)
        {
            if (cbCategory.Text != "")
            {
                lbHelp3.Visible = false;
                tbSelect.Enabled = true;
                tbTmperature.Enabled = true;
                btnSelect.Enabled = true;
                btnSave.Enabled = true;
            }
            else
            {
                tbSelect.Enabled = false;
                tbTmperature.Enabled = false;
                btnSelect.Enabled = false;
                btnSave.Enabled = false;
            }
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewX1.FirstDisplayedScrollingRowIndex = dataGridViewX1.Rows.Count - 1;

            //清除選擇
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                row.Selected = false;
            }

            //增加選到最下方一筆
            dataGridViewX1.Rows[dataGridViewX1.Rows.Count - 1].Selected = true;
        }
    }

    class SuperStudent
    {
        public SuperStudent(DataRow row)
        {
            //student.id as student_id,student.name as student_name ,
            //student.student_number,student.freshman_photo,student.seat_no,class.class_name

            StudentID = "" + row["student_id"];
            StudentName = "" + row["student_name"];
            StudentNumber = "" + row["student_number"];
            FreshmanPhoto = "" + row["freshman_photo"];
            SeatNO = "" + row["seat_no"];
            ClassName = "" + row["class_name"];
        }

        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }
        public string FreshmanPhoto { get; set; }
        public string SeatNO { get; set; }
        public string ClassName { get; set; }
        public string RefClassID { get; set; }

        public BodyTmperature bt { get; set; }

    }
}
