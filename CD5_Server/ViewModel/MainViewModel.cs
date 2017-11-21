using CD5_Server.Model;
using GalaSoft.MvvmLight;
using System.Windows;

namespace CD5_Server.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private Server server;
        private const int port = 10100;
        private const string ip = "127.0.0.1";

        public MainViewModel()
        {
            server = new Server(ip, port);
        }
    }
}