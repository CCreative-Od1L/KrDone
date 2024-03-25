using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Core.FileFunc;
using Microsoft.VisualBasic.FileIO;

namespace Core.Utils {
    // ! 因为 FileUtils 会在CoreManager初始化完成前被调用，所以 logger 是可能为空的。
    // ! 所以要注意在 FileUtils 进行日志记录的时候，需要小心再小心。
    public class FileUtils {
        #region 便捷文件操作
        /// <summary>
        /// * 检查文件夹是否存在
        /// </summary>
        /// <param name="directoryPath"></param>
        static public void CheckAndCreateDirectory(string directoryPath) {
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
                CoreManager.logger?.Info(string.Format("创建文件夹 {0}", directoryPath));
            }
        }
        /// <summary>
        /// * 检查文件的文件夹是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        static public void CheckAndCreateFileDirectory(string filePath) {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath)) {
                if (directoryPath != null) {
                    Directory.CreateDirectory(directoryPath);
                    CoreManager.logger?.Info(string.Format("创建文件 {0} 的文件夹 {1}", Path.GetFileName(filePath), Path.GetDirectoryName(filePath)));
                }
            }
        }
        /// <summary>
        /// * 创建空文件
        /// ! 会导致文件路径上的文件被复写为空文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        static public void CreateEmptyFile(string filePath) {
            CheckAndCreateFileDirectory(filePath);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            using var fs = File.Create(filePath);
            CoreManager.logger?.Info(string.Format("创建空文件 {0}", Path.GetFileName(filePath)));
        }
        /// <summary>
        ///  * 尝试创建文件
        ///  * 如果文件存在则不做操作
        /// </summary>
        /// <param name="filePath"></param>
        static public void TryToCreateFile(string filePath) {
            CheckAndCreateFileDirectory(filePath);
            if (!File.Exists(filePath)) {
                using var fs = File.Create(filePath);
                CoreManager.logger?.Info(string.Format("创建文件 {0}", Path.GetFileName(filePath)));
            }
        }
        static public void RenameFile(string filePath, string newName) {
            StringBuilder titleBuilder = new(newName);
            foreach(var invalidChar in Path.GetInvalidFileNameChars()) {
                titleBuilder = titleBuilder.Replace(invalidChar.ToString(), string.Empty);
            }
            FileSystem.RenameFile(filePath, titleBuilder.ToString());
        }
        static public void RemoveFile(List<string> filePath) {
            for(int i = 0; i < filePath.Count; i++) {
                File.Delete(filePath[i]);
            }
        }
        #endregion
        #region 写文本文件(log之类的)
        /// <summary>
        /// * 在文件结尾添加文本
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="content">内容</param>
        /// <param name="exceptionCallback">异常处理回调函数</param>
        static public void AppendText(string filePath, string content, Action<Exception>? exceptionCallback = null) {
            try {
                CheckAndCreateFileDirectory(filePath);
                using var systemWrite = File.AppendText(filePath);
                systemWrite.Write(content);
            } catch (Exception e) {
                CoreManager.logger?.Error(nameof(AppendText), e);
                exceptionCallback?.Invoke(e);
            }
        }
        #endregion
        #region 无特别规定协议
        /// <summary>
        /// * 读纯二进制-无协议文件 -> 二进制数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>二进制数据</returns>
        static public byte[] ReadBytes(string filePath) {
            if (!File.Exists(filePath)) {
                CoreManager.logger?.Info(string.Format("文件 {0} 不存在", Path.GetFileName(filePath)));
                return [];
            }
            using var fs = File.OpenRead(filePath);
            BinaryReader binaryReader = new(fs);
            binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
            List<byte> bytes = [];
            long streamLen = binaryReader.BaseStream.Length;
            if (streamLen > int.MaxValue) {
                while(binaryReader.BaseStream.Position < streamLen) {
                    bytes.AddRange(binaryReader.ReadBytes(int.MaxValue));
                }
            } else {
                bytes.AddRange(binaryReader.ReadBytes((int)binaryReader.BaseStream.Length));
            }
            return [.. bytes];
        }
        /// <summary>
        /// * 写文件，写入二进制数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <param name="exceptionCallback"></param>
        static public void WriteBytes(string filePath, byte[] content, Action<Exception>? exceptionCallback = null) {
            try {
                CheckAndCreateFileDirectory(filePath);
                using var fs = File.OpenWrite(filePath);
                using MemoryStream ms = new();
                ms.Write(content);
                ms.Position = 0;
                ms.CopyTo(fs);
            } catch (Exception e) {
                CoreManager.logger?.Error(nameof(WriteBytes), e);
                exceptionCallback?.Invoke(e);
            }
        }
        #endregion
        #region 自定文件协议
        /// <summary>
        /// * 将数据写入到 MemoryStream
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="data"></param>
        private static void WriteDataToMemoryStream(
            MemoryStream ms,
            DataForm data
        ) {
            byte bIsCrypto =  Convert.ToByte(data.EnableCrypt);
            byte[] bDataName;
            byte[] bData;
            if (!data.EnableCrypt)
            {
                NormalProcess(
                    data.Name, data.Content,
                    out bDataName, out bData);
            } else {
                EncryptProcess(
                    data.Name, data.Content,
                    out bDataName, out bData);
            }

            byte[] bDataNameLen = BitConverter.GetBytes(bDataName.Length);                
            byte[] bDataLen = BitConverter.GetBytes(bData.Length);

            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bDataNameLen);
                Array.Reverse(bDataLen);
            }
            ms.WriteByte(bIsCrypto);
            ms.Write(bDataNameLen);
            ms.Write(bDataName);
            ms.Write(bDataLen);
            ms.Write(bData);
        }        
        /// <summary>
        /// * 清空文件
        /// </summary>
        /// <param name="filePath"></param>
        private static void CleanFile(string filePath) {
            using FileStream fs = File.OpenWrite(filePath);
            fs.SetLength(0);
        }
        /// <summary>
        /// * 普通文本数据处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="bName"></param>
        /// <param name="bContent"></param>
        static private void NormalProcess(
            string name,
            string content,
            out byte[] bName,
            out byte[] bContent
        ) {
            bName = Encoding.UTF8.GetBytes(name);
            bContent = Encoding.UTF8.GetBytes(content);
            if (!BitConverter.IsLittleEndian) {
                Array.Reverse(bName);
                Array.Reverse(bContent);
            }
        }        

        /// <summary>
        /// * 加密处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="bName"></param>
        /// <param name="bContent"></param>
        static private void EncryptProcess(
            string name,
            string content,
            out byte[] bName,
            out byte[] bContent
        ) {
            bName = CryptoUtils.AesEncryptStringToBytes(name);
            bContent = CryptoUtils.AesEncryptStringToBytes(content);
        }
        /// <summary>
        /// * 异步重写文件
        /// * 以二进制格式存储 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        static public async Task AsyncWriteFile(
            string filePath, 
            List<DataForm> contents
        ) {
            CheckAndCreateFileDirectory(filePath);
            using MemoryStream ms = new();
            for(int i = 0; i < contents.Count; ++i) {
                WriteDataToMemoryStream(ms, contents[i]);
            }
            ms.Seek(0, SeekOrigin.Begin);
            CleanFile(filePath);
            using FileStream fs = File.Create(filePath);
            await ms.CopyToAsync(fs);
        }
        /// <summary>
        /// * 异步添加文件数据
        /// * 以二进制格式存储
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contents">[Name-Content-IsCrypto]</param>
        static public async Task AsyncAppendFile(
            string filePath,
            List<DataForm> contents
        ) {
            if (!File.Exists(filePath)) {
                CoreManager.logger.Error(nameof(AsyncAppendFile), new Exception(string.Format("{0} 文件不存在", filePath)));
                await AsyncWriteFile(filePath, contents);
                return;
            }
            CheckAndCreateFileDirectory(filePath);
            var FileData = ReadFile(filePath);
            using MemoryStream ms = new();
            for(int i = 0; i < contents.Count; ++i) {
                // * 不添加文件已有内容
                if (!FileData.ContainsKey(contents[i].Name)) {
                    WriteDataToMemoryStream(ms, contents[i]);
                }
            }
            ms.Seek(0, SeekOrigin.Begin);
            
            using FileStream fs = File.OpenWrite(filePath);
            fs.Seek(0, SeekOrigin.End);
            await ms.CopyToAsync(fs);
        }
        /// <summary>
        /// * 异步更新文件数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        static public async Task AsyncUpdateFile(
            string filePath,
            List<DataForm> contents
        ) {
            if (!File.Exists(filePath)) {
                CoreManager.logger.Error(nameof(AsyncUpdateFile), new Exception(string.Format("{0} 文件不存在", filePath)));
                await AsyncWriteFile(filePath, contents);
                return;
            }
            CheckAndCreateFileDirectory(filePath);
            var FileData = ReadFile(filePath);
            using MemoryStream ms = new();

            for(int i = 0; i < contents.Count; ++i) {
                FileData.Remove(contents[i].Name);
                WriteDataToMemoryStream(ms, contents[i]);
            }
            foreach(var pair in FileData) {
                WriteDataToMemoryStream(ms, pair.Value);
            }
            ms.Seek(0, SeekOrigin.Begin);
            
            CleanFile(filePath);
            using FileStream fs = File.OpenWrite(filePath);
            await ms.CopyToAsync(fs);
        }
        /// <summary>
        /// * 读文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        static public Dictionary<string, DataForm> ReadFile(string filePath, int count = -1) {
            if (!File.Exists(filePath)) { return []; }
            bool isReadAll = count < 0;
            Dictionary<string, DataForm> fileData = [];
            using (FileStream fs = File.OpenRead(filePath)) {
                BinaryReader binaryReader = new(fs);
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                long overallLen = binaryReader.BaseStream.Length;
                
                while(binaryReader.BaseStream.Position < overallLen) {
                    bool isCrypto = Convert.ToBoolean(binaryReader.ReadByte());

                    byte[] nameLen = binaryReader.ReadBytes(4);
                    byte[] name = binaryReader.ReadBytes(BitConverter.ToInt32(nameLen, 0));
                    
                    byte[] dataLen = binaryReader.ReadBytes(4);
                    byte[] data = binaryReader.ReadBytes(BitConverter.ToInt32(dataLen, 0));
                    
                    // * 固定以小端模式
                    if (!isCrypto) {
                        fileData.Add(
                            Encoding.UTF8.GetString(name),
                            new (Encoding.UTF8.GetString(name), Encoding.UTF8.GetString(data), isCrypto)
                        );
                    } else {
                        fileData.Add(
                            CryptoUtils.AesDecryptBytesToString(name),
                            new (CryptoUtils.AesDecryptBytesToString(name), CryptoUtils.AesDecryptBytesToString(data), isCrypto)
                        );
                    }

                    if (!isReadAll) {
                        if (--count == 0) { break; }
                    }
                }
            }
            return fileData;
        }
        #endregion
    }
}