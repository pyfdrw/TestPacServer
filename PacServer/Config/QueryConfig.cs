using Dicom;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        //// Study UID
        //private string _toQueryStudyInstanceUid = "";

        //public string ToQueryStudyInstanceUid
        //{
        //    get { return _toQueryStudyInstanceUid; }
        //    set
        //    {
        //        _toQueryStudyInstanceUid = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ToQueryStudyInstanceUid"));
        //    }
        //}

        // Date Range
        public DicomDateRange ToQueryStudyDateRange
        {
            get { return new DicomDateRange(ToQueryMinStudyDateRange, ToQueryMaxStudyDateRange); }
            private set { }
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
                    // ToQueryStudyDateRange = new DicomDateRange(ToQueryMinStudyDateRange, ToQueryMaxStudyDateRange);
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
                    // ToQueryStudyDateRange = new DicomDateRange(ToQueryMinStudyDateRange, ToQueryMaxStudyDateRange);
                    OnPropertyChanged(new PropertyChangedEventArgs("ToQueryMaxStudyDateRange"));
                }

            }
        }

        // CT 
        private bool _isCtSave = true;

        public bool IsCtSave
        {
            get { return _isCtSave; }
            set
            {
                _isCtSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsCtSave"));
            }
        }

        // MR
        private bool _isMrSave = true;

        public bool IsMrSave
        {
            get { return _isMrSave; }
            set
            {
                _isMrSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsMrSave"));
            }
        }

        // Pet
        private bool _isPetSave = false;

        public bool IsPetSave
        {
            get { return _isPetSave; }
            set
            {
                _isPetSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsPetSave"));
            }
        }

        // RtS
        private bool _isRtsSave = true;

        public bool IsRtsSave
        {
            get { return _isRtsSave; }
            set
            {
                _isRtsSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsRtsSave"));
            }
        }

        // RtP
        private bool _isRtpSave = true;

        public bool IsRtpSave
        {
            get { return _isRtpSave; }
            set
            {
                _isRtpSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsRtpSave"));
            }
        }

        // RtDose
        private bool _isRtDoseSave = true;

        public bool IsRtDoseSave
        {
            get { return _isRtDoseSave; }
            set
            {
                _isRtDoseSave = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsRtDoseSave"));
            }
        }

        // Not used
        // Query string for modality
        public string ModalityInStudy
        {
            get
            {
                string queryStr = "";
                queryStr += IsCtSave ? "CT\\" : "";
                queryStr += IsMrSave ? "MR\\" : "";
                queryStr += IsPetSave ? "PT\\" : "";
                queryStr += IsRtpSave ? "RTPLAN\\" : "";
                queryStr += IsRtsSave ? "RTSTRUCT\\" : "";
                queryStr += IsRtDoseSave ? "RTDOSE\\" : "";

                return queryStr;
            }
        }

        // RetrieveSaveFolder 为Retrieve的保存路径
        private string _retrieveSaveFolder = "./";

        public string RetrieveSaveFolder
        {
            get { return _retrieveSaveFolder; }
            set
            {
                if (Directory.Exists(value))
                {
                    _retrieveSaveFolder = value;
                }
                OnPropertyChanged(new PropertyChangedEventArgs("RetrieveSaveFolder"));
            }
        }

        // 存储查询到的结果
        public List<ValuableResult> Results = new List<ValuableResult>();

        // 建立连接并查询
        async public void DicomCFindRequestFunct(ServerConfig OneServerConfig)
        {
            // 首先清空原来的查询结果
            Results.Clear();

            // 建立连接，查找病人下面的所有Study
            var client = new DicomClient(OneServerConfig.RemoteIp, OneServerConfig.RemotePort,
                false, OneServerConfig.LocalAeTitle, OneServerConfig.RemoteAeTitle);
            client.NegotiateAsyncOps();

            var patientRequest = DicomCFindRequest.CreateStudyQuery(
                patientId: ToQueryPatientId, patientName: ToQueryPatientName, studyDateTime: ToQueryStudyDateRange);
            // ATTENTION
            // 需要把DicomTag.StudyTime去除才能得到正确的查询结果，可能是fo-dicom程序的Bug
            // 注意这个StudyTime与查询里面的studyDateTime不是一个意思
            patientRequest.Dataset.Remove(DicomTag.StudyTime);

            var patientRequestResults = new List<ValuableResult>();
            patientRequest.OnResponseReceived += (req, response) =>
            {
                if (response.Status == DicomStatus.Pending)
                {
                    lock (patientRequestResults)
                    {
                        var patientId = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty);
                        var studyInstanceUid = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

                        if (!string.IsNullOrEmpty(patientId) && !string.IsNullOrEmpty(studyInstanceUid))
                        {
                            patientRequestResults.Add(new ValuableResult()
                            {
                                PatientId = patientId,
                                StudyInstanceUid = studyInstanceUid
                            });

                            // 更新主界面显示
                            // OneServerConfig.AddLogStr($"Found {response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty)} " +
                            //     $"{patientId} and the study instance id is {studyInstanceUid} which was created in " +
                            //     $"{response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty)}");
                            // OneServerConfig.AddLogStr($"Found: {response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty)} ");
                        }
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
            // 得到需要的病人ID和Study的ID

            // 查询对应的序列
            var seriesRequestResults = new List<ValuableResult>();
            foreach (var patientRequestResult in patientRequestResults)
            {
                var seriesRequest = DicomCFindRequest.CreateSeriesQuery(patientRequestResult.StudyInstanceUid);
                seriesRequest.OnResponseReceived += (req, response) =>
                {
                    if (response.Status == DicomStatus.Pending)
                    {
                        var seriesUid = response.Dataset?.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                        if (!string.IsNullOrEmpty(seriesUid))
                        {
                            string modality = response.Dataset?.GetSingleValue<string>(DicomTag.Modality);
                            if ((IsCtSave && modality.ToUpper().Trim() == "CT") ||
                                (IsMrSave && modality.ToUpper().Trim() == "MR") ||
                                (IsPetSave && modality.ToUpper().Trim() == "PT") ||
                                (IsRtsSave && modality.ToUpper().Trim() == "RTSTRUCT") ||
                                (IsRtpSave && modality.ToUpper().Trim() == "PTPLAN") ||
                                (IsRtDoseSave && modality.ToUpper().Trim() == "RTDOSE"))
                            {
                                seriesRequestResults.Add(new ValuableResult()
                                {
                                    PatientId = patientRequestResult.PatientId,
                                    StudyInstanceUid = patientRequestResult.StudyInstanceUid,
                                    SeriesInstanceUid = seriesUid
                                });

                                OneServerConfig.AddLogStr($"Found: {patientRequestResult.PatientId} {modality}");
                            }
                        }
                    }
                    if (response.Status == DicomStatus.Success)
                    {
                        // Console.WriteLine(response.Status.ToString());
                        //OneServerConfig.LogInfo += response.Status.ToString() + Environment.NewLine;
                    }

                    // serieUids.Add(response.Dataset?.GetSingleValue<string>(DicomTag.SeriesInstanceUID));
                };
                await client.AddRequestAsync(seriesRequest);
                await client.SendAsync();
            }

            Results.AddRange(seriesRequestResults);
        }

        // CStore Server
        public IDicomServer CStoreServer = DicomServer.Create<CStoreSCP>(1112);

        //建立连接并Retrieve
        async public void DicomCRetrieveRequestFunct(ServerConfig OneServerConfig)
        {
            // 建立连接
            var client = new DicomClient(OneServerConfig.RemoteIp, OneServerConfig.RemotePort,
                false, OneServerConfig.LocalAeTitle, OneServerConfig.RemoteAeTitle);
            client.NegotiateAsyncOps();

            // 使用CMOVE抓取信息，发送到本机STORE服务
            // using ()
            {
                OneServerConfig.AddLogStr($"Run C-Store SCP server on port 1112");
                foreach (var oneResult in Results)
                {
                    var cMoveRequest = new DicomCMoveRequest("SegAE", oneResult.StudyInstanceUid, oneResult.SeriesInstanceUid);
                    bool? moveSuccessfully = null;


                    cMoveRequest.OnResponseReceived += (DicomCMoveRequest requ, DicomCMoveResponse response) =>
                    {
                        if (response.Status.State == DicomState.Pending)
                        {
                            // Console.WriteLine("Sending is in progress. please wait: " + response.Remaining.ToString());
                        }
                        else if (response.Status.State == DicomState.Success)
                        {
                            OneServerConfig.AddLogStr($"{oneResult.PatientId} " +
                                $"{oneResult.SeriesInstanceUid}");
                            moveSuccessfully = true;
                        }
                        else if (response.Status.State == DicomState.Failure)
                        {

                            OneServerConfig.AddLogStr($"{response.Status.Description}");
                            moveSuccessfully = false;
                        }
                    };
                    await client.AddRequestAsync(cMoveRequest);
                    await client.SendAsync();
                }

            }

        }
        public void SaveImage(DicomDataset dataset, string storagePath)
        {
            var studyUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            var instUid = dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);

            var path = Path.GetFullPath(storagePath);
            path = Path.Combine(path, studyUid);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, instUid) + ".dcm";

            new DicomFile(dataset).Save(path);
        }
    }

    // 存储有效的查找记录
    public class ValuableResult
    {
        public string PatientId;
        public string StudyInstanceUid;
        public string SeriesInstanceUid;
    }
}
