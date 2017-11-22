using CD5_DataHandling;
using CD5_Server.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;

using System.Linq;

namespace CD5_Server.ViewModel
{

    public class MainViewModel : ViewModelBase
    {           
        private Server server;
        private const int port = 10100;
        private const string ip = "127.0.0.1";
        public bool isConnected = false;
        private FileHandler fileHandler;
        public string SelectedClient { get; set; }
        public string SelectedLogfile { get; set; }
        public ObservableCollection<string> Msgs { get; set; }
        public ObservableCollection<string> Clients { get; set; }
        public ObservableCollection<string> LogMsgs { get; set; }
        public RelayCommand StartBtnClickCommand { get; set; }
        public RelayCommand StopBtnClickCommand { get; set; }
        public RelayCommand DropClientBtnClickCommand { get; set; }
        public RelayCommand SaveBtnClickCommand { get; set; }
        public RelayCommand DropLogBtnClickCommand { get; set; }
        public RelayCommand ShowBtnClickCommand { get; set; }
        public ObservableCollection<string> LogFiles
        {
            get
            {
                return new ObservableCollection<string>(fileHandler.SearchLogFiles());
            }
        }

        public MainViewModel()
        {
            Clients = new ObservableCollection<string>();
            Msgs = new ObservableCollection<string>();
            fileHandler = new FileHandler();
            LogMsgs = new ObservableCollection<string>();
            
            StartBtnClickCommand = new RelayCommand(() => 
            {
                server = new Server(ip, port, GUIBroadcast); //instantiate server
                server.StartAccepting(); //start listening
                isConnected = true;
            }, () => { return !isConnected; }); //can execute?

            StopBtnClickCommand = new RelayCommand(() => 
            {
                server.StopAccepting(); //stop listening, close conn
                isConnected = false;
            }, () => { return isConnected; }); //can execute?

            ShowBtnClickCommand = new RelayCommand(() =>
            {
                LogMsgs = new ObservableCollection<string>(fileHandler.Read(SelectedLogfile));
                RaisePropertyChanged("LogMessages");
            }, () => { return SelectedLogfile != null; });

            DropLogBtnClickCommand = new RelayCommand(() =>
            {
                fileHandler.Delete(SelectedLogfile);
                RaisePropertyChanged("LogFiles");
            }, () => { return SelectedLogfile != null; });

            SaveBtnClickCommand = new RelayCommand(() =>
            {
                fileHandler.Write(Msgs.ToList());
                RaisePropertyChanged("LogFiles");
            }, () => { return Msgs.Count >= 1; } );

            DropClientBtnClickCommand = new RelayCommand(DropClientBtnClickMethod);

        }

        public int MsgCounter { get { return Msgs.Count; } }

        private void GUIBroadcast(string newMsg)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string uname = newMsg.Split(':')[0]; //name is everything before ':'
                if (!Clients.Contains(uname))
                {
                    Clients.Add(uname);
                }
                Msgs.Add(newMsg);
                RaisePropertyChanged("MsgCounter"); //invokes reload
            });
        }

        public void DropClientBtnClickMethod()
        {
            server.KickClient(SelectedClient); //kick from server
            Clients.Remove(SelectedClient); //remove from GUI
        }
    }
}