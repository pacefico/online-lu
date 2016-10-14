
 
 


declare module HistoryModels {
interface HistoryModelVM {
  ID: number;
  ExecutionDate: Date;
  Range: number;
  TimeDonwloadMs: number;
  CudaTimeMs: number;
  TimeUploadMs: number;
  TotalTimeMs: number;
  Success: boolean;
  Details: HistoryModels.HistoryDetailVM;
}
interface HistoryDetailVM {
  BytesUpload: string;
  RateKbsUpload: string;
  BytesDownload: string;
  RateKbsDownload: string;
}
}
declare module ProjectModels {
interface ProjectModelParamVM {
  DateFrom?: Date;
  DateTo?: Date;
  Status: boolean;
  Range: number;
}
interface ProjectSimpleModelVM {
  ID: number;
  ExecutionDate: Date;
  Range: number;
  Status?: boolean;
  History: HistoryModels.HistoryModelVM;
}
interface UploadFileModelParamVM {
  ID: number;
  Line: string;
}
}
