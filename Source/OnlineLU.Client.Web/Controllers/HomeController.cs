using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.JsonHelper;
using OnlineLU.Client.Web.Models;
using OnlineLU.Server.BTC;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OnlineLU.Client.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Paulo Figueiredo";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contato";

            return View();
        }

        #region History

        public ActionResult History()
        {
            ViewBag.Message = "Histórico";

            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult RetrieveHistory()
        {
            var _responseResult = new List<HistoryModelVM>();

            var _btcResp = new HomeBTC().GetHistory(new GetHistoryParamTO());

            if (!_btcResp.HasErrors())
            {
                foreach (var history in _btcResp.Result.Histories)
                {
                    var _historyVM = new HistoryModelVM()
                    {
                        CudaTimeMs = history.TimeCudaMs != null ? (long)history.TimeCudaMs : 0,
                        Range = history.Range,
                        ExecutionDate = history.ExecutionDate,
                        TimeInitialUploadMs = history.TimeInitialUpalod != null ? (long)history.TimeInitialUpalod : 0,
                        TimeDonwloadMs = history.TimeDownload != null ? (long)history.TimeDownload : 0,
                        TimeUploadMs = history.TimeUpload != null ? (long)history.TimeUpload : 0,
                        TotalTimeMs = history.TotalTime != null ? (long)history.TotalTime : 0,
                        Success = history.Success,
                        Details = new HistoryDetailVM()
                        {
                            BytesInitialUpload = history.Detail.ByteInitialUpload,
                            RateKbsInitialUpload = history.Detail.RateInitialUpload,
                            BytesDownload = history.Detail.ByteDownload,
                            BytesUpload = history.Detail.ByteUpload,
                            RateKbsDownload = history.Detail.RateDownload,
                            RateKbsUpload = history.Detail.RateUpload
                        }
                    };
                    _responseResult.Add(_historyVM);
                }
            }

            var _response = new
            {
                Result = _responseResult
                //Total = _responseResult.total
            };

            return Json(_response, JsonRequestBehavior.AllowGet);
        }





        #endregion History

        #region NewProject

        public ActionResult New()
        {
            ViewBag.Message = "Nova Execução";

            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GenerateProjectExecution(ProjectSimpleModelVM paramVM)
        {
            var _responseResult = new List<HistoryModelVM>();
            bool _success = false;

            string _rangeStr = paramVM.Range.ToString();

            var _paramTO = new ProjectTO()
            {
                Status = false,
                ContainerName = OnlineLUConstants.ContainerName,
                ContainerNameResult = OnlineLUConstants.ContainerNameResult,
                QueueName = OnlineLUConstants.QueueName,
                QueueNameResult = OnlineLUConstants.QueueNameResult,
                Range = paramVM.Range,
            };
            
            var _btcResp = new HomeBTC().SaveExecution(_paramTO);

            if (!_btcResp.HasErrors() && _btcResp.Result > 0)
            {
                var _strSerialized = JsonSerialize.SerializeQueueMessage(new Library.Models.QueueMessage{
                    projectid = _btcResp.Result,
                    containerSource = _paramTO.ContainerName,
                    containerResult = _paramTO.ContainerNameResult,
                    queueName = _paramTO.QueueName,
                    range = _paramTO.Range
                });

                var _queueHelper = new QueueHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);

                if (_queueHelper.PutMessage(_paramTO.QueueName, _strSerialized))
                {
                    _success = true;
                }
            }

            var _response = new
            {
                Result = _success
                //Total = _responseResult.total
            };

            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public JsonResult GetExecutions(ProjectModelParamVM paramVM)
        {
            var _responseList = new List<ProjectSimpleModelVM>();

            var _paramTO = new GetExecutionsParamTO()
            {
                Range = paramVM.Range,
                Status = paramVM.Status,
            };

            if (paramVM.DateFrom.HasValue)
            {
                if (paramVM.DateFrom.Value.Year > 2000)
                {
                    _paramTO.DateFrom = paramVM.DateFrom;
                }
            }
            if (paramVM.DateTo.HasValue)
            {
                if (paramVM.DateTo.Value.Year > 2000)
                {
                    _paramTO.DateTo = paramVM.DateTo;
                }
            }

            var _btcResponse = new HomeBTC().GetExecutions(_paramTO);

            if (!_btcResponse.HasErrors())
            {
                foreach (var project in _btcResponse.Result.Projects)
                {
                    var _projectVM = new ProjectSimpleModelVM()
                    {
                         ExecutionDate = project.Created,
                         ID = project.ID,
                         Range = project.Range,
                         Status = project.Status
                    };
                    _responseList.Add(_projectVM);
                }
            }

            var _response = new
            {
                Result = _responseList
                //Total = _responseResult.total
            };
            return Json(_response, JsonRequestBehavior.AllowGet);
        }

        #region Upload

        [HttpPost]
        public ActionResult SetMetadata(int blocksCount, string fileName, long fileSize)
        {
            var _paramTO = new ProjectTO()
            {
                Status = false,
                ContainerName = OnlineLUConstants.ContainerName,
                ContainerNameResult = OnlineLUConstants.ContainerNameResult,
                QueueName = OnlineLUConstants.QueueName,
                QueueNameResult = OnlineLUConstants.QueueNameResult,
                Range = blocksCount,
            };
            
            var _btcResp = new HomeBTC().SaveExecution(_paramTO);

            if (!_btcResp.HasErrors() && _btcResp.Result > 0)
            {
                var fileToUpload = new CloudFile(blocksCount)
                {
                    BlockCount = blocksCount,
                    FileName = fileName,
                    Size = fileSize,
                    StartTime = DateTime.Now,
                    IsUploadCompleted = false,
                    UploadStatusMessage = string.Empty,
                    ProjectData = _paramTO
                };
                fileToUpload.ProjectData.ID = _btcResp.Result;

                Session.Add("CurrentFile", fileToUpload);
            }
            return Json(true);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadChunk(int id)
        {
            HttpPostedFileBase request = Request.Files["Slice"];
            byte[] chunk = new byte[request.ContentLength];
            request.InputStream.Read(chunk, 0, Convert.ToInt32(request.ContentLength));
            JsonResult returnData = null;
            string fileSession = "CurrentFile";
            
            if (Session[fileSession] != null)
            {
                CloudFile model = (CloudFile)Session[fileSession];
                
                returnData = UploadCurrentChunk(model, chunk, id);
                
                if (returnData != null)
                {
                    return returnData;
                }
                if (id == model.BlockCount-1)
                {
                    model.DisposeQueue();

                    var _initialUploadTime = DateTime.Now - model.StartTime;
                    var _bytesInitial = model.CompressedBytesSent;
                    var _initialRateUpload = model.CompressedBytesSent / _initialUploadTime.Seconds;

                    var _btcResp = new HomeBTC().SetHistory(new SetHistoryParamTO()
                    {
                        IsInitial = true,
                        History = new HistoryTO()
                        {
                            ProjectID = model.ProjectData.ID,
                            TimeInitialUpalod = (long)_initialUploadTime.TotalMilliseconds,
                            Range = model.ProjectData.Range,
                            Detail = new HistoryDetailTO()
                            {
                                ByteInitialUpload = model.CompressedBytesSent.ToString(),
                                RateInitialUpload = _initialRateUpload.ToString()
                            }
                        }
                    });

                    if (!_btcResp.HasErrors())
                    {
                        var _strSerialized = JsonSerialize.SerializeQueueMessage(new Library.Models.QueueMessage
                        {
                            projectid = model.ProjectData.ID,
                            containerSource = model.ProjectData.ContainerName,
                            containerResult = model.ProjectData.ContainerNameResult,
                            queueName = model.ProjectData.QueueName,
                            range = model.ProjectData.Range
                        });

                        var _queueHelper = new QueueHelper(AzureStorageConstants.Account, AzureStorageConstants.StorageAccountKey);

                        if (_queueHelper.PutMessage(model.ProjectData.QueueName, _strSerialized))
                        {
                            return CommitAllChunks(model);
                        }
                    }
                }
            }
            else
            {
                returnData = Json(new
                {
                    error = true,
                    isLastBlock = false,
                    message = string.Format(CultureInfo.CurrentCulture,
                        "Failed to Upload file.", "Session Timed out")
                });
                return returnData;
            }
            return Json(new { error = false, isLastBlock = false, message = string.Empty });
        }

        private JsonResult UploadCurrentChunk(CloudFile model, byte[] chunk, int id)
        {
            try
            {
                model.SendFile(id, chunk);

                //model.BlockBlob.PutBlock(
                //    blockId,
                //    chunkStream, null, null,
                //    new BlobRequestOptions()
                //    {
                //        RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3)
                //    },
                //    null);
                return null;
            }
            catch (Exception e)
            {
                Session.Remove("CurrentFile");
                model.IsUploadCompleted = true;
                model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                return Json(new { error = true, isLastBlock = false, message = model.UploadStatusMessage });
            }
        }

        private ActionResult CommitAllChunks(CloudFile model)
        {
            model.IsUploadCompleted = true;
            bool errorInOperation = false;
            
            try
            {
                var duration = DateTime.Now - model.StartTime;
                
                float fileSizeInKb = model.Size / 1024;
                
                string fileSizeMessage = fileSizeInKb > 1024 ?
                    string.Concat((fileSizeInKb / 1024).ToString(CultureInfo.CurrentCulture), " MB") :
                    string.Concat(fileSizeInKb.ToString(CultureInfo.CurrentCulture), " KB");
                
                model.UploadStatusMessage = string.Format(CultureInfo.CurrentCulture,
                    "File uploaded successfully. {0} took {1} seconds to upload",
                    fileSizeMessage, Math.Round(duration.TotalSeconds, 2));
            }
            catch (Exception e)
            {
                model.UploadStatusMessage = "Failed to Upload file. Exception - " + e.Message;
                errorInOperation = true;
            }
            finally
            {
                Session.Remove("CurrentFile");
            }
           
            return Json(new
            {
                error = errorInOperation,
                isLastBlock = model.IsUploadCompleted,
                message = model.UploadStatusMessage
            });
        }

        #endregion Upload

        #endregion NewProject

        #region Graphics

        #endregion Graphics


    }
}
