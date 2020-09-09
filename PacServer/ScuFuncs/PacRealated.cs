using PacServer.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dicom.Log;
using PacServer.Config;
using Dicom.Network.Client;
using Dicom.Network;
using Dicom;

using System.IO;

namespace PacServer.ScuFuncs
{
    static public class PacRealated
    {
        // 按照设置的条件查询
        // 返回ID
        static public async void QueryServer_Do(ServerConfig OneServerConfig, QueryConfig OneQueryConfig)
        {
            // List<ValuableResult> results = new List<ValuableResult>();
            OneQueryConfig.DicomCFindRequestFunct(OneServerConfig);
        }

        // 按照查询的结构保存图像
        static public async void RetrieveServer_Do(ServerConfig OneServerConfig, QueryConfig OneQueryConfig)
        {
            OneQueryConfig.DicomCRetrieveRequestFunct(OneServerConfig);
        }

        
    }
}
