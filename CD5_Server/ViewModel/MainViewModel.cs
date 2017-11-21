using CD5_Server.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace CD5_Server.ViewModel
{

    public class MainViewModel : ViewModelBase
    {           
        private Server server;
        private const int port = 10100;
        private const string ip = "127.0.0.1";
        public bool isConnected = false;
        public string SelectedClient { get; set; }
        public ObservableCollection<string> Msgs { get; set; }
        public ObservableCollection<string> Clients { get; set; }
        
        public RelayCommand StartBtnClickCommand { get; set; }
        public RelayCommand StopBtnClickCommand { get; set; }
        public RelayCommand DropClientBtnClickCommand { get; set; }
        public RelayCommand SaveBtnClickCommand { get; set; }

        public MainViewModel()
        {
            Clients = new ObservableCollection<string>();
            Msgs = new ObservableCollection<string>();

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

            DropClientBtnClickCommand = new RelayCommand(DropClientBtnClickMethod);
            SaveBtnClickCommand = new RelayCommand(SaveBtnClickMethod);

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

        public void SaveBtnClickMethod()
        {
            //bonus
        }
    }
}