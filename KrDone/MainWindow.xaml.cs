using Core.Src.DBFunc;
using Core.Src.DBFunc.DBEntiries;
using Core.Utils;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KrDone {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            CoreInit();
        }
        private static void CoreInit() {
            DbMgr.CreateTable();
            
            if(DbMgr.TodoInfo != null) {
                TodoInfoEntry entry = new()
                {
                    Id = DateTimeUtils.GetCurrentTimestampMilliSecond(),
                    TodoDate = DateOnly.FromDateTime(DateTime.Now),
                    IsDone = false,
                    TodoContent = "Hello! This is the first todo item."
                };

                DbMgr.TodoInfo.InsertDataIntoTable([entry]);
            }
        }
    }
}