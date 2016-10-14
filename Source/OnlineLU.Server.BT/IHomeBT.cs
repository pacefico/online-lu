using OnlineLU.HelperUtils.MessageResponse;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLU.Server.BT
{
    public interface IHomeBT
    {
        MessageResponse<GetHistoryRespTO> GetHistory(GetHistoryParamTO paramTO);

        MessageResponse<SetHistoryRespTO> SetHistory(SetHistoryParamTO paramTO);

        MessageResponse<long> SaveExecution(ProjectTO paramTO);

        MessageResponse<GetExecutionsRespTO> GetExecutions(GetExecutionsParamTO paramTO);
    }
}
