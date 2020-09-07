using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacServer.Config
{
    public class ServerConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        // localAE
        private string _localAeTitle = "SegAE";

        public string LocalAeTitle
        {
            get { return _localAeTitle; }
            set
            {
                _localAeTitle = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LocalAeTitle"));
            }
        }

        // remote AE
        private string _remoteAeTitle = "MyDcmServer";

        public string RemoteAeTitle
        {
            get { return _remoteAeTitle; }
            set
            {
                _remoteAeTitle = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RemoteAeTitle"));
            }
        }
        // ip
        private string _remoteIp = "169.1.133.111";

        public string RemoteIp
        {
            get { return _remoteIp; }
            set
            {
                _remoteIp = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RemoteIp"));
            }
        }

        // port
        private int _remotePort = 4242;

        public int RemotePort
        {
            get { return _remotePort; }
            set
            {
                _remotePort = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RemotePort"));
            }
        }

        // string to show
        private string _logInfo = "Test Server Connection\n";

        public string LogInfo
        {
            get { return _logInfo; }
            set
            {
                _logInfo = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LogInfo"));
            }
        }

        public void AddLogStr(string logToAdd)
        {
            LogInfo += logToAdd + Environment.NewLine;
        }
    }
}
