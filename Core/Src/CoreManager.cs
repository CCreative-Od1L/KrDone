using Core.DirectoryFunc;
using Core.Logger;

namespace Core {
    public static class CoreManager {
        
        static public readonly LogManager logger;
        static public readonly DirectoryManager directoryMgr;
        static CoreManager(){
            // !! 注意初始化的顺序
            directoryMgr = DirectoryManager.INSTANCE;
            logger = LogManager.INSTANCE;
        }
    }
}