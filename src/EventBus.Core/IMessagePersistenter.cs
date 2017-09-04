using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IMessagePersistenter<TContent>
    {
        long TransactionID { get; }

        #region Using EF
        /// <summary>
        /// [For EntityFramework] Insert a message into database asynchrony.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<IMessage<TContent>> InsertAsync(TContent content, IMetaData metaData = null);
        /// <summary>
        /// [For EntityFramework] Change state of a specialized message asynchrony.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageState">new message state</param>
        /// <returns></returns>
        Task ChangeStateAsync(long messageId, long transactId, MessageState messageState);
        #endregion

        #region Using Ado.Net
        /// <summary>
        /// [For Ado.NET] Insert a message into database asynchrony.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<IMessage<TContent>> InsertAsync(TContent content, IDbConnection dbConnection, IDbTransaction dbTransaction, IMetaData metaData = null);
        /// <summary>
        /// [For Ado.NET] Change state of a specialized message asynchrony.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageState">new message state</param>
        /// <returns></returns>
        Task ChangeStateAsync(long messageId, long transactId, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction);
        #endregion  

    }
}
