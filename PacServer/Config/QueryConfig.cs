using Dicom;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DicomClient = Dicom.Network.Client.DicomClient;

namespace PacServer.Config
{
    public class QueryConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        // Patient Name
        private string _toQueryPatientName = "";

        public string ToQueryPatientName
        {
            get { return _toQueryPatientName; }
            set
            {
                _toQueryPatientName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ToQueryPatientName"));
            }
        }

        // Patient ID
        private string _toQueryPatientId = "";

        public string ToQueryPatientId
        {
            get { return _toQueryPatientId; }
            set
            {
                _toQueryPatientId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ToQueryPatientId"));
            }
        }

        // Study UID
        private string _toQueryStudyInstanceUid = "";

        public string ToQueryStudyInstanceUid
        {
            get { return _toQueryStudyInstanceUid; }
            set
            {
                _toQueryStudyInstanceUid = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ToQueryStudyInstanceUid"));
            }
        }

        // Date Range
        private DicomDateRange _toQueryStudyDateRange = new DicomDateRange(DateTime.Today, DateTime.Today);

        public DicomDateRange ToQueryStudyDateRange
        {
            get { return _toQueryStudyDateRange; }
            set
            {
                _toQueryStudyDateRange = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ToQueryStudyDateRange"));
            }
        }

        // min Date
        private DateTime _toQueryMinStudyDateRange = DateTime.Today.AddMonths(-12);

        public DateTime ToQueryMinStudyDateRange
        {
            get { return _toQueryMinStudyDateRange; }
            set
            {
                if (_toQueryMinStudyDateRange <= _toQueryMaxStudyDateRange)
                {
                    _toQueryMinStudyDateRange = value;
                    ToQueryStudyDateRange.Minimum = _toQueryMinStudyDateRange;
                    OnPropertyChanged(new PropertyChangedEventArgs("ToQueryMinStudyDateRange"));
                }

            }
        }

        // max date
        private DateTime _toQueryMaxStudyDateRange = DateTime.Today;

        public DateTime ToQueryMaxStudyDateRange
        {
            get { return _toQueryMaxStudyDateRange; }
            set
            {
                if (_toQueryMinStudyDateRange <= _toQueryMaxStudyDateRange)
                {
                    _toQueryMaxStudyDateRange = value;
                    ToQueryStudyDateRange.Maximum = _toQueryMaxStudyDateRange;
                    OnPropertyChanged(new PropertyChangedEventArgs("ToQueryMaxStudyDateRange"));
                }

            }
        }

        // 建立连接并查询
        async public void DicomCFindRequestFunct(ServerConfig OneServerConfig, List<ValuableResult> Results)
        {
            // 首先清空原来的查询结果
            Results.Clear();

            // 建立连接，进行病人层次的查询
            var client = new DicomClient(OneServerConfig.RemoteIp, OneServerConfig.RemotePort,
                false, OneServerConfig.LocalAeTitle, OneServerConfig.RemoteAeTitle);
            client.NegotiateAsyncOps();

            var patientRequest = DicomCFindRequest.CreatePatientQuery(patientId: ToQueryPatientId, patientName: ToQueryPatientName);
            // List<string> patientIdsSearched = new List<string>();

            patientRequest.OnResponseReceived += (req, response) =>
            {
                if (response.Status == DicomStatus.Pending)
                {
                    lock (Results)
                    {
                        Results.Add(new ValuableResult()
                        {
                            PatientId = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty)
                        });
                    }
                }
                if (response.Status == DicomStatus.Success)
                {
                    // Console.WriteLine(response.Status.ToString());
                    //OneServerConfig.LogInfo += response.Status.ToString() + Environment.NewLine;
                }
            };

            await client.AddRequestAsync(patientRequest);
            await client.SendAsync();
            // 病人层次的查询结束

            // 进行Study层次的查询
            List<ValuableResult> resultTmp = new List<ValuableResult>();
            foreach (var patientId in Results)
            {
                var studyRequest = DicomCFindRequest.CreateStudyQuery(
                    patientId: patientId.PatientId, 
                    studyInstanceUid: ToQueryStudyInstanceUid, 
                    studyDateTime: ToQueryStudyDateRange);

                studyRequest.OnResponseReceived += (req, response) =>
                {
                    if (response.Status == DicomStatus.Pending)
                    {
                        lock (resultTmp)
                        {
                            resultTmp.Add(new ValuableResult()
                            {
                                PatientId = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                                StudyInstanceUid = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                            });
                        }
                    }
                    if (response.Status == DicomStatus.Success)
                    {
                        // Console.WriteLine(response.Status.ToString());
                        //OneServerConfig.LogInfo += response.Status.ToString() + Environment.NewLine;
                    }
                };
            }
            // Study层次查询成功
            
            // 


            return DicomCFindRequest.CreateStudyQuery(
                patientName: this.ToQueryPatientName,
                patientId: this.ToQueryPatientId,
                studyDateTime: this.ToQueryStudyDateRange,
                studyInstanceUid: this.ToQueryStudyInstanceUid);
        }
    }

    // 存储有效的查找记录
    public class ValuableResult
    {
        public string PatientId;
        public string StudyInstanceUid;
        public string SeriesUid;
    }
}
