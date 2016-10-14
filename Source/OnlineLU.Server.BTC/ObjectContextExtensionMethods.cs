using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;

namespace OnlineLU.Server.BTC
{
    public static class ObjectContextExtensionMethods
    {
        public static void ApplyIsolationLevel(this ObjectContext objectContext, System.Transactions.IsolationLevel isolationLevel)
        {
            switch (isolationLevel)
            {
                case System.Transactions.IsolationLevel.Chaos:
                    break;
                case System.Transactions.IsolationLevel.ReadCommitted:
                    objectContext.ExecuteStoreCommand("set transaction isolation level read committed", null);
                    break;
                case System.Transactions.IsolationLevel.ReadUncommitted:
                    objectContext.ExecuteStoreCommand("set transaction isolation level read uncommitted", null);
                    break;
                case System.Transactions.IsolationLevel.RepeatableRead:
                    break;
                case System.Transactions.IsolationLevel.Serializable:
                    break;
                case System.Transactions.IsolationLevel.Snapshot:
                    break;
                case System.Transactions.IsolationLevel.Unspecified:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get a PO in memory or create a PO with propertyID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectContext"></param>
        /// <param name="keyPropertyName"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public static T GetObjectByKey<T>(this ObjectContext objectContext, object keyValue) where T : EntityObject
        {
            T _objectPO = null;

            var _propertyObjectSet = objectContext.GetType()
                .GetProperties()
                .Where(a => a.PropertyType == typeof(ObjectSet<T>))
                .FirstOrDefault();

            if (_propertyObjectSet != null)
            {
                ObjectSet<T> _entitySet = _propertyObjectSet.GetValue(objectContext, null) as ObjectSet<T>;
                var keyPropertyName = "";

                foreach (var _property in typeof(T).GetProperties())
	            {
                    var _customAttributes = _property.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), true);

                    if (_customAttributes != null && _customAttributes.Length > 0 && (_customAttributes.GetValue(0) as EdmScalarPropertyAttribute).EntityKeyProperty) 
                    {
                        keyPropertyName = _property.Name;
                        break;
                    }
	            }

                ObjectStateEntry _result = null;
                objectContext.ObjectStateManager.TryGetObjectStateEntry(
                        new EntityKey(string.Format("{0}.{1}", objectContext.DefaultContainerName, _entitySet.EntitySet.ToString()), keyPropertyName, keyValue), out _result);

                if (_result == null)
                {
                    var _propertyID = typeof(T).GetProperty(keyPropertyName);
                    _objectPO = Activator.CreateInstance<T>();
                    _propertyID.SetValue(_objectPO, keyValue, null);

                    _entitySet.Attach(_objectPO);
                }
                else
                {
                    _objectPO = (T)_result.Entity;
                }
            }

            return _objectPO;
        }
    }
}
