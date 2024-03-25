namespace Core.DirectoryFunc {
    public class FileDirectory {
        public string Root { get; private set; }
        public string ThirdParty { get; private set; }
        public string Cache { get; private set; }
        public string Download { get; set; }
        public string? Log { get; set; }
        public string? Cookie { get; set; }
        public string? Crypto { get; set; }
        public string? UserInfo { get; set; }
        public FileDirectory() {
            Root = AppDomain.CurrentDomain.BaseDirectory + @"AppData\";
            ThirdParty = AppDomain.CurrentDomain.BaseDirectory + @"ThirdParty\";
            Download = AppDomain.CurrentDomain.BaseDirectory + @"Download\";
            Cache = Root + @"Cache\";
        }
        public void UpdateData(FileDirectory fileDirectory) {
            Log = fileDirectory.Log ?? Log;
            Cookie = fileDirectory.Cookie ?? Cookie;
            Crypto = fileDirectory.Crypto ?? Crypto;
            UserInfo = fileDirectory.UserInfo ?? UserInfo;
        }
        public void TryToResetDefault(){
            if (string.IsNullOrEmpty(Root)) { Root = AppDomain.CurrentDomain.BaseDirectory + @"AppData\"; }
            if (string.IsNullOrEmpty(ThirdParty)) { ThirdParty = AppDomain.CurrentDomain.BaseDirectory + @"ThirdParty\"; }
            if (string.IsNullOrEmpty(Download)) { Download = AppDomain.CurrentDomain.BaseDirectory + @"Download\"; }
            if (string.IsNullOrEmpty(Cache)) { Cache = Root + @"Cache\"; }

            if (string.IsNullOrEmpty(Log)) { Log = Root + @"Logs\"; }
            if (string.IsNullOrEmpty(Cookie)) { Cookie = Root + @"Cookie\"; }
            if (string.IsNullOrEmpty(Crypto)) { Crypto = Root + @"Crypto\"; }
            if (string.IsNullOrEmpty(UserInfo)) { UserInfo = Cache + @"UserInfo\"; }
        }
        public void ResetDefault(string name) {
            switch(name.ToLower()) {
                case "log":
                    Log = Root + @"Logs\";
                    break;
                case "cookie":
                    Cookie = Root + @"Cookies\";
                    break;
                case "crypto":
                    Crypto = Root + @"Crypto\";
                    break;
                case "userinfo":
                    UserInfo = Cache + @"UserInfo\";
                    break;
                default:
                    break;
            }
        }
    }
}