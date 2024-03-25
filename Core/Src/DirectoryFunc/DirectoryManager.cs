using Core.Utils;
using System.IO;

namespace Core.DirectoryFunc {
    public sealed class DirectoryManager {
        public static DirectoryManager INSTANCE { get; } = new();
        public FileDirectory fileDirectory;
        readonly string dataDefaultName = "content";
        readonly string relativeFileDirectoryJsonPath = @"26BYhvPy.dat";
        private DirectoryManager() {
            fileDirectory = new();
            FileUtils.ReadFile(Path.Combine(fileDirectory.Root, relativeFileDirectoryJsonPath)).TryGetValue(dataDefaultName, out var item);
            if (item == null) { 
                TryToInit(); 
            } else {
                string dirDictionaryContent = item.Content;
                UpdateFileDirectory(JsonUtils.ParseJsonString<FileDirectory>(dirDictionaryContent));
            }
            _ = FileUtils.AsyncWriteFile(
                Path.Combine(fileDirectory.Root, relativeFileDirectoryJsonPath),
                [new(dataDefaultName, JsonUtils.SerializeJsonObj(fileDirectory), true)]);
        }
        void TryToInit() {
            fileDirectory.TryToResetDefault();
        }

        void UpdateFileDirectory(FileDirectory? newFileDirectory) {
            if (newFileDirectory == null) { 
                fileDirectory.TryToResetDefault();
                return; 
            }
            fileDirectory.UpdateData(newFileDirectory);
        }
        public void ResetToDefault(string name) {
            fileDirectory.ResetDefault(name);
        }
        public string GetLogDirectory() {
            TryToInit();
            return fileDirectory.Log!;
        }
        public string GetCookieDirectory() {
            TryToInit();
            return fileDirectory.Cookie!;
        }
    }
}