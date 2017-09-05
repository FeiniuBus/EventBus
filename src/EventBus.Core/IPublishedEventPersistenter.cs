using EventBus.Core.State;
using System.Data;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IPublishedEventPersistenter
    {
        #region Using Ado.Net
        /// <summary>
        /// [For Ado.NET] Insert a message into database asynchrony.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task InsertAsync(object message, IDbConnection dbConnection, IDbTransaction dbTransaction);
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
