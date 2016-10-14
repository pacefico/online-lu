using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace OnlineLU.Server.BTC
{
    public static class ConnectionUtils
    {
        public static EntityConnection GetTargetEntityConnection<TEntityContext>(params object[] parameters)
    where TEntityContext : ObjectContext
        {
            EntityConnection _returnValue = null;

            string _configKey = typeof(TEntityContext).Name;

            System.Configuration.ConnectionStringSettings _connString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[_configKey];
            if (_connString == null)
            {
                _connString = System.Configuration.ConfigurationManager.ConnectionStrings[_configKey];
            }

            if (_connString != null)
            {
                System.Data.EntityClient.EntityConnectionStringBuilder entityBuilder =
                    new System.Data.EntityClient.EntityConnectionStringBuilder(_connString.ConnectionString);

                string _targetConnectionString = string.Empty;

                //if (parameters.Count() <= 0)
                //{
                //    _targetConnectionString = TCConfigurationAppServices.GetTargetDBConnectionString();
                //}
                //else
                //{
                //    BaseTenantControlBTParam _btParam = (from a in parameters.OfType<BaseTenantControlBTParam>()
                //                                         select a).FirstOrDefault();

                //    if (_btParam != null)
                //    {
                //        _targetConnectionString = TCConfigurationAppServices.GetTargetDBConnectionString(_btParam);
                //    }
                //}

                if (!String.IsNullOrWhiteSpace(_targetConnectionString))
                {
                    entityBuilder.ProviderConnectionString = _targetConnectionString;
                }

                _returnValue = new System.Data.EntityClient.EntityConnection(entityBuilder.ToString());
            }

            return _returnValue;
        }


    }
}
