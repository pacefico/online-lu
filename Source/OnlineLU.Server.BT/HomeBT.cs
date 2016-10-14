using OnlineLU.HelperUtils.MessageResponse;
using OnlineLU.Server.POLibrary;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace OnlineLU.Server.BT
{
    public class HomeBT : IHomeBT
    {
        #region Members

        private OnlineLUEntities m_EntityContext = null;

        #endregion Members

        #region constructors

        public HomeBT()
        {
        }

        public HomeBT(OnlineLUEntities entityContext)
        {
            this.m_EntityContext = entityContext;
        }

        #endregion constructors

        #region IHomeBTMethods

        public MessageResponse<GetHistoryRespTO> GetHistory(GetHistoryParamTO paramTO)
        {
            var _response = new MessageResponse<GetHistoryRespTO>()
            {
                Result = new GetHistoryRespTO()
                {
                    Histories = new List<HistoryTO>()
                }
            };

            var _histories = (from history in m_EntityContext.tbHistories
                              join project in m_EntityContext.tbProjects on history.tbProjectID equals project.ID
                              where history.Success == true
                              select new HistoryTO()
                              {
                                  ExecutionDate = history.ExecutionDate,
                                  Range = project.Range,
                                  ID = history.ID,
                                  ProjectID = history.tbProjectID,
                                  Success = history.Success,
                                  TimeInitialUpalod = history.TimeInitialUploadMs,
                                  TimeCudaMs = history.TimeCudaMs,
                                  TimeDownload = history.TimeDownloadMs,
                                  TimeUpload = history.TimeUploadMs,
                                  TotalTime = history.TotalTimeMs,
                                  Detail = new HistoryDetailTO()
                                  {
                                    ByteInitialUpload = history.tbHistoryDetail.BytesInitialUpload,
                                    ByteDownload = history.tbHistoryDetail.BytesDownload,
                                    ByteUpload = history.tbHistoryDetail.BytesUpload,
                                    RateDownload = history.tbHistoryDetail.RateDownload,
                                    RateUpload = history.tbHistoryDetail.RateUpload,
                                    RateInitialUpload = history.tbHistoryDetail.RateInitialUpload
                                  }

                              });

            _response.Result.Histories = _histories.ToList();

            //if (paramTO.Take > 0)
            //{
            //    _response.Result.SimpleSite = _response.Result.SimpleSite.Skip(paramTO.Skip).Take(paramTO.Take).ToList();
            //}

            return _response;
        }

        public MessageResponse<SetHistoryRespTO> SetHistory(SetHistoryParamTO paramTO)
        {
            var _response = new MessageResponse<SetHistoryRespTO>()
            {
                Result = new SetHistoryRespTO(){ Success = false }
            };

            var _projectPO = m_EntityContext.tbProjects.Where(a => a.ID == paramTO.History.ProjectID).FirstOrDefault();

            tbHistory _historyPO = null;
            tbHistoryDetail _detailPO = null;

            if (paramTO.IsInitial)
            {
                if (paramTO.History != null)
                {
                    _historyPO = new tbHistory();
                    _historyPO.Created = DateTime.Now;
                    _historyPO.Modified = DateTime.Now;
                    _historyPO.ExecutionDate = DateTime.Now;
                    _historyPO.TimeInitialUploadMs = paramTO.History.TimeInitialUpalod;
                    _historyPO.tbProjectID = _projectPO.ID;
                    //_historyPO.Range = paramTO.History.Range;

                    if (paramTO.History.Detail != null)
                    {
                        _detailPO = new tbHistoryDetail();
                        _detailPO.BytesInitialUpload = paramTO.History.Detail.ByteInitialUpload;
                        _detailPO.RateInitialUpload = paramTO.History.Detail.RateInitialUpload;

                        _historyPO.tbHistoryDetail = _detailPO;
                    }
                    else
                    {
                        //erro em details
                    }
                }
                else
                {
                    //erro em history
                }
                
                if (_detailPO != null)
                    _historyPO.tbHistoryDetail = _detailPO;

                m_EntityContext.tbHistories.AddObject(_historyPO);

            } else{

                if (_projectPO != null)
                {
                    _projectPO.Status = true;
                    _historyPO = m_EntityContext.tbHistories.Where(a => a.tbProjectID == _projectPO.ID).FirstOrDefault();
                }

                if (_historyPO != null)
                {
                    long _totalTime = (long)paramTO.History.TotalTime;
                    if (_historyPO.TimeInitialUploadMs.HasValue)
                    {
                        _totalTime += _historyPO.TimeInitialUploadMs.Value;  
                    }

                    _historyPO.ExecutionDate = paramTO.History.ExecutionDate;
                    _historyPO.Modified = DateTime.Now;
                    _historyPO.Success = paramTO.History.Success;
                    _historyPO.TimeCudaMs = paramTO.History.TimeCudaMs;
                    _historyPO.TimeDownloadMs = paramTO.History.TimeDownload;
                    _historyPO.TimeUploadMs = paramTO.History.TimeUpload;
                    _historyPO.TotalTimeMs = (long)_totalTime;
                    _historyPO.tbProjectID = (long)paramTO.History.ProjectID;

                    _detailPO = (from history in m_EntityContext.tbHistories
                                 join detail in m_EntityContext.tbHistoryDetails on history.tbHistoryDetail_ID equals detail.ID
                                 where history.ID == _historyPO.ID
                                 select detail).FirstOrDefault();

                    if (paramTO.History.Detail != null)
                    {
                        if (_detailPO != null)
                        {
                            //_detailPO = new tbHistoryDetail();
                            _detailPO.BytesDownload = paramTO.History.Detail.ByteDownload;
                            _detailPO.BytesUpload = paramTO.History.Detail.ByteUpload;
                            _detailPO.RateDownload = paramTO.History.Detail.RateDownload;
                            _detailPO.RateUpload = paramTO.History.Detail.RateUpload;
                        }
                    }

                    tbHardware _hardwarePO = null;
                    if (paramTO.History.Hardware != null)
                    {
                        var _hardware = m_EntityContext.tbHardwares.Where(a => a.HardwareKey == paramTO.History.Hardware.HardwareKey).FirstOrDefault();

                        if (_hardware == null)
                        {
                            _hardwarePO = new tbHardware();
                            _hardwarePO.HardwareKey = paramTO.History.Hardware.HardwareKey;
                            _hardwarePO.MemoryAmount = paramTO.History.Hardware.MemoryAmount;
                            _hardwarePO.SystemName = paramTO.History.Hardware.SystemName;
                            _hardwarePO.ProcessorName = paramTO.History.Hardware.ProcessorName;
                            _hardwarePO.CoreNumber = paramTO.History.Hardware.CoreNumber;
                            _hardwarePO.CudaCapable = paramTO.History.Hardware.CudaCapable;
                        }
                        else
                        {
                            _hardwarePO = _hardware;
                        }
                    }

                    if (_hardwarePO != null)
                        _historyPO.tbHardware = _hardwarePO;
                }
            }



            _response.Result.Success = true;

            return _response;
        }

        public MessageResponse<long> SaveExecution(ProjectTO paramTO)
        {
            var _response = new MessageResponse<long>();

            var _userPO = m_EntityContext.tbUsers.Where(a => a.Username == "admin").FirstOrDefault();

            
            var _projectPO = new tbProject()
            {
                Created = DateTime.Now,
                ContainerName = paramTO.ContainerName,
                ContainerNameResult = paramTO.ContainerNameResult,
                QueueName = paramTO.QueueName,
                QueueNameResult = paramTO.QueueNameResult,
                Range = paramTO.Range,
                Status = paramTO.Status
            };

            if (_userPO != null)
            {
                _projectPO.tbUser = _userPO;
            }

            m_EntityContext.tbProjects.AddObject(_projectPO);

            m_EntityContext.SaveChanges();

            _response.Result = _projectPO.ID;

            return _response;
        }

        public MessageResponse<GetExecutionsRespTO> GetExecutions(GetExecutionsParamTO paramTO)
        {
            var _response = new MessageResponse<GetExecutionsRespTO>()
            {
                Result = new GetExecutionsRespTO()
                {
                    Projects = new List<ProjectTO>()
                }
            };

            var _query = m_EntityContext.tbProjects.ToList();

            if (paramTO.DateFrom.HasValue && paramTO.DateTo.HasValue)
            {
                _query = _query.Where(a => a.Created >= paramTO.DateFrom.Value && a.Created <= paramTO.DateTo.Value).ToList();
            }

            if (paramTO.Range > 0)
            {
                _query = _query.Where(a => a.Range == paramTO.Range).ToList();
            }

            if (paramTO.Status.HasValue)
            {
                _query = _query.Where(a => a.Status == paramTO.Status.Value).ToList();
            }

            var _result = (from project in _query
                                select new ProjectTO()
                                {
                                    ID = project.ID,
                                    Created = project.Created,
                                    Range = project.Range,
                                    Status = (bool)project.Status,
                                }).ToList();
            
            _response.Result.Projects = _result;
            _response.Result.Total = _result.Count();

            return _response;
        }
        


        #endregion IHomeBTMethods

    }

}
