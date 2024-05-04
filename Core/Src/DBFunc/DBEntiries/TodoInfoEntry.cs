using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBEntiries {
    public class TodoInfoEntry : DbEntryBase {
        // * 在Table类内进行属性的装填
        // * 主键 ID -> INTEGER
        // * 当前时间戳
        public long Id { get; set; }
        // * DateOnly -> TEXT
        public DateOnly TodoDate { get; set; }
        // * Boolean -> INTEGER
        public bool IsDone { get; set; } = false;
        // * string -> TEXT
        public string TodoContent { get; set; } = string.Empty;
        /// <summary>
        /// * 数据转换为字符串的形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"'{Id}', '{TodoDate}',");
            stringBuilder.Append(IsDone ? "1," : "0,");
            stringBuilder.Append($"'{TodoContent}'");

            return stringBuilder.ToString();
        }
        /// <summary>
        /// * 获取主键数据
        /// </summary>
        /// <returns></returns>
        public List<string> GetPrimeKeyString() {
            List<string> res = [];
            res.Add(Id.ToString());
            return res;
        }

        public override void PushValues(object[] values)
        {
            // * 长度不符合
            if(values.Length != 4) { return; }
            
            Id = (long)values[0];
            TodoDate = DateOnly.Parse((string)values[1]);
            IsDone = (long)values[2] == 1;
            TodoContent = (string)values[3];
        }
    }
}
