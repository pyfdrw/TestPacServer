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
            

            // Find a list of Studies

            // var request = OneQueryConfig.DicomCFindRequestFunct();
            var request = OneQueryConfig.DicomCFindRequestFunct2();

            var studyUids = new List<string>();

            var serieUids = new List<string>();

            request.OnResponseReceived += (req, response) =>
            {
                DebugStudyResponse(response, OneServerConfig);
                studyUids.Add(response.Dataset?.GetSingleValue<string>(DicomTag.StudyInstanceUID));

                //DebugSerieResponse(response, OneServerConfig);
                //serieUids.Add(response.Dataset?.GetSingleValue<string>(DicomTag.SeriesInstanceUID));
            };
            await client.AddRequestAsync(request);
            await client.SendAsync();
        }

        #region test code
        //public static DicomCFindRequest CreateStudyRequestByPatientName(string patientName)
        //{
        //    // there is a built in function to create a Study-level CFind request very easily: 
        //    // return DicomCFindRequest.CreateStudyQuery(patientName: patientName);

        //    // but consider to create your own request that contains exactly those DicomTags that
        //    // you realy need pro process your data and not to cause unneccessary traffic and IO load:

        //    var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

        //    // always add the encoding
        //    // request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");
        //    request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "utf-8");

        //    // add the dicom tags with empty values that should be included in the result of the QR Server
        //    request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
        //    request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyDate, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyDescription, "");

        //    // add the dicom tags that contain the filter criterias
        //    request.Dataset.AddOrUpdate(DicomTag.PatientName, patientName);

        //    return request;
        //}

        //public static DicomCFindRequest CreateStudyRequestByPatientId(string patientName)
        //{
        //    // there is a built in function to create a Study-level CFind request very easily: 
        //    // return DicomCFindRequest.CreateStudyQuery(patientName: patientName);

        //    // but consider to create your own request that contains exactly those DicomTags that
        //    // you realy need pro process your data and not to cause unneccessary traffic and IO load:

        //    var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

        //    // always add the encoding
        //    // request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");
        //    request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "utf-8");

        //    // add the dicom tags with empty values that should be included in the result of the QR Server
        //    request.Dataset.AddOrUpdate(DicomTag.PatientName, "");
        //    request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyDate, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.StudyDescription, "");

        //    // add the dicom tags that contain the filter criterias
        //    request.Dataset.AddOrUpdate(DicomTag.PatientName, patientName);

        //    return request;
        //}

        //public static DicomCFindRequest CreateSeriesRequestByStudyUID(string studyInstanceUID)
        //{
        //    // there is a built in function to create a Study-level CFind request very easily: 
        //    // return DicomCFindRequest.CreateSeriesQuery(studyInstanceUID);

        //    // but consider to create your own request that contains exactly those DicomTags that
        //    // you realy need pro process your data and not to cause unneccessary traffic and IO load:
        //    var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Series);

        //    //request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");
        //    request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "utf-8");

        //    // add the dicom tags with empty values that should be included in the result
        //    request.Dataset.AddOrUpdate(DicomTag.SeriesInstanceUID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.SeriesDescription, "");
        //    request.Dataset.AddOrUpdate(DicomTag.Modality, "");
        //    request.Dataset.AddOrUpdate(DicomTag.NumberOfSeriesRelatedInstances, "");

        //    // add the dicom tags that contain the filter criterias
        //    request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, studyInstanceUID);

        //    return request;
        //}

        //// "20200808-20200809" OR "20200805"
        //public static DicomCFindRequest CreateSeriesRequestByStudyDate(string studyDate)
        //{
        //    // there is a built in function to create a Study-level CFind request very easily: 
        //    // return DicomCFindRequest.CreateSeriesQuery(studyInstanceUID);

        //    // but consider to create your own request that contains exactly those DicomTags that
        //    // you realy need pro process your data and not to cause unneccessary traffic and IO load:
        //    var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Series);

        //    //request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");
        //    request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "utf-8");

        //    // add the dicom tags with empty values that should be included in the result
        //    request.Dataset.AddOrUpdate(DicomTag.SeriesInstanceUID, "");
        //    request.Dataset.AddOrUpdate(DicomTag.SeriesDescription, "");
        //    request.Dataset.AddOrUpdate(DicomTag.Modality, "");
        //    request.Dataset.AddOrUpdate(DicomTag.NumberOfSeriesRelatedInstances, "");

        //    // add the dicom tags that contain the filter criterias
        //    request.Dataset.AddOrUpdate(DicomTag.StudyDate, studyDate);

        //    return request;
        //}


        //public static DicomCGetRequest CreateCGetBySeriesUID(string studyUID, string seriesUID)
        //{
        //    var request = new DicomCGetRequest(studyUID, seriesUID);
        //    // no more dicomtags have to be set
        //    return request;
        //}


        //public static DicomCMoveRequest CreateCMoveBySeriesUID(string destination, string studyUID, string seriesUID)
        //{
        //    var request = new DicomCMoveRequest(destination, studyUID, seriesUID);
        //    // no more dicomtags have to be set
        //    return request;
        //}


        //public static DicomCMoveRequest CreateCMoveByStudyUID(string destination, string studyUID)
        //{
        //    var request = new DicomCMoveRequest(destination, studyUID);
        //    // no more dicomtags have to be set
        //    return request;
        //}
        #endregion

        public static void DebugStudyResponse(DicomCFindResponse response, ServerConfig OneServerConfig)
        {
            if (response.Status == DicomStatus.Pending)
            {
                // print the results
                // Console.WriteLine($"Patient {response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty)}, {(response.Dataset.TryGetString(DicomTag.ModalitiesInStudy, out var dummy) ? dummy : string.Empty)}-Study from {response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, new DateTime())} with UID {response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty)} ");
                OneServerConfig.LogInfo += $"Patient " +
                    $"{response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty)}, " +
                    $"{(response.Dataset.TryGetString(DicomTag.ModalitiesInStudy, out var dummy) ? dummy : string.Empty)}" +
                    $"-Study from " +
                    $"{response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, new DateTime())} " +
                    $"with UID " +
                    $"{response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty)} " + 
                    Environment.NewLine;

            }
            if (response.Status == DicomStatus.Success)
            {
                // Console.WriteLine(response.Status.ToString());
                OneServerConfig.LogInfo += response.Status.ToString() + Environment.NewLine;
            }
        }


        public static void DebugSerieResponse(DicomCFindResponse response, ServerConfig OneServerConfig)
        {
            try
            {
                if (response.Status == DicomStatus.Pending)
                {
                    // print the results
                    //Console.WriteLine($"Serie {response.Dataset.GetSingleValue<string>(DicomTag.SeriesDescription)}, {response.Dataset.GetSingleValue<string>(DicomTag.Modality)}, {response.Dataset.GetSingleValue<int>(DicomTag.NumberOfSeriesRelatedInstances)} instances");
                    OneServerConfig.LogInfo += $"Serie " +
                        $"{response.Dataset.GetSingleValue<string>(DicomTag.SeriesDescription)}, " +
                        $"{response.Dataset.GetSingleValue<string>(DicomTag.Modality)}, " +
                        $"{response.Dataset.GetSingleValue<int>(DicomTag.NumberOfSeriesRelatedInstances)} " +
                        $"instances" +
                        Environment.NewLine;
                }
                if (response.Status == DicomStatus.Success)
                {
                    //Console.WriteLine(response.Status.ToString());
                    OneServerConfig.LogInfo += response.Status.ToString() +
                        Environment.NewLine;
                }
            }
            catch (Exception)
            {
                // ignore errors
            }
        }


        public static void SaveImage(DicomDataset dataset, string storagePath)
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
}
