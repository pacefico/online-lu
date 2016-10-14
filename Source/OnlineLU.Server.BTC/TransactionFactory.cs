using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace OnlineLU.Server.BTC
{
    /// <summary>
    /// Classe utilizada para facilitar a criação e manipulação de transações.
    /// </summary>
    public class TransactionFactory
    {
        #region Methods

        #region Implementation

        /// <summary>
        /// Método criado para automatizar o processo de criação do TransactionScope.
        /// </summary>
        /// <param name="transactionScopeOption"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(TransactionScopeOption transactionScopeOption, System.Transactions.IsolationLevel isolationLevel)
        {
            TransactionScope _returnValue = null;

            //if (Transaction.Current.IsNotNull())
            if (Transaction.Current != null)
            {
                isolationLevel = Transaction.Current.IsolationLevel;
            }

            _returnValue = new TransactionScope(
                transactionScopeOption,
                new TransactionOptions()
                {
                    IsolationLevel = isolationLevel
                }
            );


            return _returnValue;
        }

        #endregion Implementation

        #endregion Methods
    }
}
