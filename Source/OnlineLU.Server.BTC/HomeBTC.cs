using OnlineLU.HelperUtils;
using OnlineLU.HelperUtils.MessageResponse;
using OnlineLU.Server.BT;
using OnlineLU.Server.POLibrary;
using OnlineLU.TOLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineLU.Server.BTC
{
    public class HomeBTC : IHomeBT
    {
        #region IHomeBTMethods

        public MessageResponse<GetHistoryRespTO> GetHistory(GetHistoryParamTO paramTO)
        {
            return this.TransactionBTC<GetHistoryRespTO>(TransactionScopeOption.Suppress, IsolationLevel.ReadUncommitted, "GetHistory", paramTO);
        }

        public MessageResponse<SetHistoryRespTO> SetHistory(SetHistoryParamTO paramTO)
        {
            return this.TransactionBTC<SetHistoryRespTO>(TransactionScopeOption.RequiresNew, IsolationLevel.ReadCommitted, "SetHistory", paramTO);
        }

        public MessageResponse<long> SaveExecution(ProjectTO paramTO)
        {
            return this.TransactionBTC<long>(TransactionScopeOption.RequiresNew, IsolationLevel.ReadCommitted, "SaveExecution", paramTO);
        }

        public MessageResponse<GetExecutionsRespTO> GetExecutions(GetExecutionsParamTO paramTO)
        {
            return this.TransactionBTC<GetExecutionsRespTO>(TransactionScopeOption.Suppress, IsolationLevel.ReadUncommitted, "GetExecutions", paramTO);
        }

        #endregion IHomeBTMethods

        #region transaction
        private MessageResponse<T> TransactionBTC<T>(TransactionScopeOption transactionScope, IsolationLevel isolationLevel, string methodName, params object[] methodArgs)
        {
            MessageResponse<T> _response = new MessageResponse<T>();

            try
            {

                System.Data.EntityClient.EntityConnection _entityConnection = ConnectionUtils.GetTargetEntityConnection <OnlineLUEntities>();
                    //= null; // TenantControl.UtilityLibrary.TCConfigurationAppServices.GetTargetEntityConnection<PesquisaRapidaEntities>(paramTO);

                    using (TransactionScope _scope = TransactionFactory
                        .CreateTransactionScope(transactionScope, isolationLevel))
                    {

                        using (OnlineLUEntities _entityContext = new OnlineLUEntities(_entityConnection))
                        {

                            _entityContext.ApplyIsolationLevel(IsolationLevel.ReadUncommitted);
                            Type _type = typeof(HomeBT);
                            MethodInfo _method = _type.GetMethod(methodName);
                            HomeBT _bt = new HomeBT(_entityContext);

                            _response = (MessageResponse<T>) _method.Invoke(_bt, methodArgs);

#if DEBUG

                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                System.Diagnostics.Debug.Assert(!_response.HasErrors(), "Foram encontrados erros no MessageResponse.");

                                if (_response.HasErrors())
                                    System.Diagnostics.Debugger.Break();
                            }
#endif

                            if (_response.HasErrors())
                            {
                                return _response;
                            }

                            _entityContext.SaveChanges();
                            _scope.Complete();

                            return _response;
                        }
                    }
                
            }
            catch (Exception ex)
            {
#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
#endif

                _response = new MessageResponse<T>();

                _response.Exceptions.Add(
                    new MessageResponseException(ex.Message, ResponseKind.Error));

                return _response;
            }
            //return _response;
        }
        #endregion transaction
    }
}
