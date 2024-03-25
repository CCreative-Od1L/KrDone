using System.Text;

namespace Core.Logger {
    /// <summary>
    /// * 日志信息（数据类） 
    /// </summary> 
    public class LogInfo {
        public DateTime Time {get; set;}
        public int ThreadID {get; set;}
        /// <summary>
        /// * LogLevel : 日志等级 
        /// </summary>
        public LOG_LEVEL LogLevel {get; set;}
        /// <summary>
        /// * Source : 源对象全名
        /// </summary> <summary>
        public string? Source {get; set;}
        public string? Message {get; set;}
        /// <summary>
        /// * Exception Info 
        /// </summary>
        /// <value>
        /// * ExceptionObj : 异常对象
        /// * ExceptionType : 异常类型
        /// </value>
        public Exception? ExceptionObj {get; set;}
        public string? ExceptionType {get; set;}

        // * Web Info
        public string? RequestUrl {get; set;}
        public string? UserAgent {get; set;}

        readonly private static string format = "{0} ";

        private static void AppendUseFormat(string value, ref StringBuilder stringBuilder, string format) {
            stringBuilder.AppendFormat(format, value);
        }
        private static void MultiAppendUseFormat(List<Tuple<string, string>> valueList, ref StringBuilder stringBuilder) {
            foreach(var value in valueList) {
                if (string.IsNullOrEmpty(value.Item1)) {
                    AppendUseFormat(value.Item2, ref stringBuilder, format);
                } else {
                    AppendUseFormat(value.Item2, ref stringBuilder, value.Item1);
                }
            }
        }
        public override string ToString() {
            StringBuilder stringBuilder = new();
            var list = new List<Tuple<string, string>>(){
                new("Time:{0} ",Time.ToString()),
                new("[{0}] ", ThreadID.ToString()),
                new(string.Empty, LogLevel.ToString()),
            };
            
            if (!string.IsNullOrEmpty(Source)) {
                list.Add(new("Src:{0} ", Source));
            }

            if (!string.IsNullOrEmpty(Message)) {
                list.Add(new("Mes:{0} ", Message));
            }
            
            if (!string.IsNullOrEmpty(ExceptionType)) {
                list.Add(new("ErrT:{0} ", ExceptionType));
            }

            // * 最后一个添加
            if (ExceptionObj != null) {
                if (ExceptionObj.StackTrace != null) {
                    list.Add(new(Environment.NewLine + "{0}", ExceptionObj.StackTrace));
                }
            }

            MultiAppendUseFormat(list, ref stringBuilder);
            return stringBuilder.ToString();
        }
    }
}