using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utils;

namespace Core.Logger {
    sealed public partial class LogManager {
        public static LogManager INSTANCE { get; } = new();
        /// <summary>
        /// * 存放日志任务。
        /// item1: 日志文件名称
        /// item2: 
        /// </summary> <summary>
        readonly ConcurrentQueue<Tuple<string, string>> logMessageQueue = new();
        AutoResetEvent Pause => new(false);

        CancellationTokenSource loggerCTS;
        AutoResetEvent? loggerStopLock;

        public event Action<LogInfo>? OnLogAction;  // * 可自定义Log事件，会在日志记录发生时触发。
        private LogManager() {
            loggerCTS = new();
            CheckAndCreateLogDirectory();
            StartTask();
        }        
        void LoggerMainTask() {
            Dictionary<string, string> logMessageBuf = [];
            foreach (var logItem in logMessageQueue) {
                string logPath = logItem.Item1;
                string mergeMessage = string.Concat(
                    logItem.Item2,
                    Environment.NewLine,
                    "=================================================================================",
                    Environment.NewLine);
                bool addResult = logMessageBuf.TryAdd(logPath, mergeMessage);
                if (!addResult){
                    logMessageBuf[logPath] = string.Concat(
                        logMessageBuf[logPath],
                        mergeMessage);  // * 合并同一文件路径下的Log信息
                }
                _ = logMessageQueue.TryDequeue(out _);  // * 处理完成后的消息出队列。
            }
            foreach (var logMessage in logMessageBuf) {
                FileUtils.AppendText(logMessage.Key, logMessage.Value, (e) => {
                    Console.WriteLine("Logger Error Happen./日志输出到文件出现错误");
                });
            }
        }
        public void StartTask() {
            loggerStopLock = new(false);
            var loggerCancellationToken = loggerCTS.Token;
            Task logTask = new(obj => {
                while(true) {
                    if (loggerCancellationToken.IsCancellationRequested) {
                        LoggerMainTask();
                        loggerStopLock.Set();
                        break;
                    }
                    Pause.WaitOne(2000, true);
                    LoggerMainTask();
                    
                }
            }, null, TaskCreationOptions.LongRunning);
            logTask.Start();
        }
        public void StopTask() {
            if (loggerStopLock == null) { return; }
            Task stopTask = new (() => {
                loggerCTS.Cancel();
                loggerStopLock.WaitOne();
                loggerCTS.Dispose();
            });
            stopTask.Start();
        }
        public static bool CheckAndCreateLogDirectory() {
            if (string.IsNullOrEmpty(CoreManager.directoryMgr.GetLogDirectory())) {
                CoreManager.directoryMgr.ResetToDefault("log");
            }
            if (!Directory.Exists(CoreManager.directoryMgr.GetLogDirectory())) {
                Directory.CreateDirectory(CoreManager.directoryMgr.GetLogDirectory());
            }
            return true;
        }
        
        [GeneratedRegex("(?<=\\()(\\d+)(?=\\))")]
        private partial Regex LogNoRegex();

        /// <summary>
        /// 返回构造出来的日志文件名称，格式:yyyyMMdd([pattern]).log
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns> <summary>
        static string ConstructLogFileName(string pattern) {
            StringBuilder stringBuilder = new();
            stringBuilder.AppendFormat("{0}({1}).log", DateTime.Now.ToString("yyyyMMdd"), pattern);
            return stringBuilder.ToString();
        }
        string GetLogFilePath() {
            string newFilePath = string.Empty;
            CheckAndCreateLogDirectory();
            string logFolderPath = CoreManager.directoryMgr.GetLogDirectory();
            string fileNamePattern = ConstructLogFileName("*");
            List<string> filePaths = Directory.GetFiles(
                logFolderPath, 
                fileNamePattern, 
                SearchOption.TopDirectoryOnly)
                .ToList();
            if (filePaths.Count > 0) {
                int fileNameMaxLen = filePaths.Max(path => path.Length);
                var latestLogFilePath = filePaths.Where(path => path.Length == fileNameMaxLen).OrderDescending().First();
                if (new FileInfo(latestLogFilePath).Length > 1 * 1024 * 1024) {
                    var strLogNo = LogNoRegex().Match(Path.GetFileName(latestLogFilePath)).Value;
                    var parse = int.TryParse(strLogNo, out int intLogNo);
                    var logFileNo = $"{(parse ? intLogNo + 1 : intLogNo)}";
                    newFilePath = Path.Combine(logFolderPath, ConstructLogFileName(logFileNo));
                } else {
                    newFilePath = latestLogFilePath;
                }
            } else {
                newFilePath = Path.Combine(logFolderPath, ConstructLogFileName("0"));
            }
            return newFilePath;
        }
        
        void PushBackLogMessageQueue(string message) {
            logMessageQueue.Enqueue(new Tuple<string, string>(
                GetLogFilePath(),
                message
            ));
        }
        #region Level: Info
        public void Info(string info) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.INFO,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Info(string source, string info) {
            var logInfo = new LogInfo(){
                LogLevel = LOG_LEVEL.INFO,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source                
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Info(Type source, string info) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.INFO,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source.FullName
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        #endregion

        #region Level: Debug
        public void Debug(string info) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.DEBUG,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Debug(string source, string info) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.DEBUG,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Debug(Type source, string info) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.DEBUG,
                Message = info,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source.FullName,
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        #endregion
        
        #region Level: Error
        public void Error(Exception error) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = error.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = error.Source,
                ExceptionObj = error,
                ExceptionType = error.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString()) ;
            OnLogAction?.Invoke(logInfo);
        }
        public void Error(string source, Exception error) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = error.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source,
                ExceptionObj = error,
                ExceptionType = error.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString()) ;
            OnLogAction?.Invoke(logInfo);
        }
        public void Error(Type source, Exception error) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = error.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source.FullName,
                ExceptionObj = error,
                ExceptionType = error.GetType().FullName
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Error(string source, string error) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = error,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source,
                ExceptionType = error.GetType().FullName
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Error(Type source, string error) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = error,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                ExceptionObj = null,
                ExceptionType = error.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        #endregion

        #region Level: Fatal
        public void Fatal(Exception fatal) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = fatal.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                ExceptionObj = fatal,
                ExceptionType = fatal.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Fatal(string source, Exception fatal) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = fatal.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source,
                ExceptionObj = fatal,
                ExceptionType = fatal.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Fatal(Type source, Exception fatal) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = fatal.Message,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source.FullName,
                ExceptionObj = fatal,
                ExceptionType = fatal.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Fatal(string source, string fatal) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = fatal,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source,
                ExceptionType = fatal.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        public void Fatal(Type source, string fatal) {
            var logInfo = new LogInfo() {
                LogLevel = LOG_LEVEL.ERROR,
                Message = fatal,
                Time = DateTime.Now,
                ThreadID = Environment.CurrentManagedThreadId,
                Source = source.FullName,
                ExceptionType = fatal.GetType().Name
            };
            PushBackLogMessageQueue(logInfo.ToString());
            OnLogAction?.Invoke(logInfo);
        }
        #endregion
    }
}