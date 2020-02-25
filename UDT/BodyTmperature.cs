using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyTemperature
{
    /// <summary>
    /// �ǥ���Ŭ���
    /// </summary>
    [TableName("body_temperature.student_tmperature")]
    public class BodyTmperature : ActiveRecord
    {
        /// <summary>
        /// �t�νs��(�Ѯv/�ǥ�)
        /// </summary>
        [Field(Field = "ref_obj_id", Indexed = false)]
        public int ObjID { get; set; }

        /// <summary>
        /// ��H(�Ѯv,�ǥ�)
        /// </summary>
        [Field(Field = "body_tag", Indexed = false)]
        public string BodyTag { get; set; }

        /// <summary>
        /// ���
        /// </summary>
        [Field(Field = "body_temperature", Indexed = false)]
        public double BodyTemperature { get; set; }

        /// <summary>
        /// �q���覡(�B��,�J�զ�)
        /// </summary>
        [Field(Field = "measurement_method", Indexed = false)]
        public string MeasurementMethod { get; set; }

        /// <summary>
        /// ���O(������ŰO���@�Ӥ���)
        /// </summary>
        [Field(Field = "category", Indexed = false)]
        public string Category { get; set; }

        /// <summary>
        /// �a�I
        /// </summary>
        [Field(Field = "location", Indexed = false)]
        public string Location { get; set; }

        /// <summary>
        /// �n�����(yyyy/MM/dd HH:mm)
        /// </summary>
        [Field(Field = "occur_date", Indexed = false)]
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// �Ƶ�
        /// </summary>
        [Field(Field = "remark", Indexed = false)]
        public string Remark { get; set; }



    }

}
