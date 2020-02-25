using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyTemperature
{
    /// <summary>
    /// 學生體溫紀錄
    /// </summary>
    [TableName("body_temperature.student_tmperature")]
    public class BodyTmperature : ActiveRecord
    {
        /// <summary>
        /// 系統編號(老師/學生)
        /// </summary>
        [Field(Field = "ref_obj_id", Indexed = false)]
        public int ObjID { get; set; }

        /// <summary>
        /// 對象(老師,學生)
        /// </summary>
        [Field(Field = "body_tag", Indexed = false)]
        public string BodyTag { get; set; }

        /// <summary>
        /// 體溫
        /// </summary>
        [Field(Field = "body_temperature", Indexed = false)]
        public double BodyTemperature { get; set; }

        /// <summary>
        /// 量測方式(額溫,入耳式)
        /// </summary>
        [Field(Field = "measurement_method", Indexed = false)]
        public string MeasurementMethod { get; set; }

        /// <summary>
        /// 類別(給予體溫記錄一個分類)
        /// </summary>
        [Field(Field = "category", Indexed = false)]
        public string Category { get; set; }

        /// <summary>
        /// 地點
        /// </summary>
        [Field(Field = "location", Indexed = false)]
        public string Location { get; set; }

        /// <summary>
        /// 登錄日期(yyyy/MM/dd HH:mm)
        /// </summary>
        [Field(Field = "occur_date", Indexed = false)]
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        [Field(Field = "remark", Indexed = false)]
        public string Remark { get; set; }



    }

}
