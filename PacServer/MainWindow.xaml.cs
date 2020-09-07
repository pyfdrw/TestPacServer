using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dicom;
using Dicom.Log;
using Dicom.Network;
using PacServer.Config;
using DicomClient = Dicom.Network.Client.DicomClient;

using NLog.Config;
using NLog.Targets;
using Path = System.IO.Path;
using PacServer.ScuFuncs;

namespace PacServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ServerConfig TheServerConfig = new ServerConfig();

        public QueryConfig TheQueryConfig = new QueryConfig();

        public MainWindow()
        {
            InitializeComponent();

            serverCfgGrid.DataContext = TheServerConfig;
            QueryLogGrid.DataContext = TheServerConfig;
            QueryConfigGrid.DataContext = TheQueryConfig;

        }

        private void echoServer_ButtonClick(object sender, RoutedEventArgs e)
        {
            TheServerConfig.LogInfo = "Test Server Connection\n";
            echoServer_Do();
        }

        private async void echoServer_Do()
        {
            // var server = new DicomServer<DicomCEchoProvider>();
            var client = new DicomClient(TheServerConfig.RemoteIp, TheServerConfig.RemotePort,
                false, TheServerConfig.LocalAeTitle, TheServerConfig.RemoteAeTitle);
            client.NegotiateAsyncOps();

            var request = new DicomCEchoRequest();

            request.OnResponseReceived += (DicomCEchoRequest req, DicomCEchoResponse response) =>
            {
                // Console.WriteLine("C-Echo Status: " + response.Status);
                TheServerConfig.LogInfo += ("ECHO: " + response.Status + Environment.NewLine);
            };

            await client.AddRequestAsync(request);

            await client.SendAsync();
        }

        private void QueryServer_ButtonClick(object sender, RoutedEventArgs e)
        {
            PacRealated.QueryServer_Do(TheServerConfig, TheQueryConfig);
        }
    }
}
