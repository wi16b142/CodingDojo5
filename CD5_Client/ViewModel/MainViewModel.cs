using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;
using CD5_Client.Model;
using System.Windows.Input;

namespace CD5_Client.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private const int port = 10100;
        private const string ip = "127.0.0.1";
        public string OutgoingUnamne { get; set; }
        
        public string OutgoingMsg { get; set; }
        public RelayCommand ConnectBtnCLickCommand { get; set; }
        public RelayCommand SendBtnCLickCommand { get; set; }
        public RelayCommand DisconnectBtnCLickCommand { get; set; }
        public ObservableCollection<string> History { get; set; }

        private Client client;
        private bool isConnected = false;

        public MainViewModel()
        {
            OutgoingUnamne = "";
            OutgoingMsg = "";
            History = new ObservableCollection<string>();
            ConnectBtnCLickCommand = new RelayCommand(() =>
            {
                client = new Client(ip, port, new Action<string>(IncomingMsgReceived), AfterDisconnect);
                isConnected = true;
            }, () => { return !isConnected; });

            SendBtnCLickCommand = new RelayCommand(() => 
            {
                client.Send(OutgoingUnamne+": "+OutgoingMsg);
                History.Add("YOU: " + OutgoingMsg);
            }, () => { return (isConnected && OutgoingMsg.Length > 0); });

            DisconnectBtnCLickCommand = new RelayCommand(() =>
            {
                client.Send(OutgoingUnamne+": @quit");
                isConnected = false;
            }, () => { return (isConnected); });

        }

        private void IncomingMsgReceived(string newMsg)
        {
            App.Current.Dispatcher.Invoke(() => 
            {
                History.Add(newMsg);
            });
        }

        private void AfterDisconnect()
        {
            isConnected = false;
            CommandManager.InvalidateRequerySuggested(); //force requery event to update button visibility
        }

    }
}