using Dapper;
using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using EventBus.MySQL.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.MySQL
{
    public class MessagePersistenter<TContent> : IMessagePersistenter<TContent>
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBusEFOptions _eventBusEFOptions;
        private readonly DbContext _dbContext;
        private readonly ILogger _logger;

        internal MessagePersistenter(long transactionID, IIdentityGenerator identityGenerator, EventBusEFOptions efOptions, ILogger<MessagePersistenter<TContent>> _logger, IServiceProvider serviceProvider)
        {
            TransactionID = transactionID;
            _eventBusEFOptions = efOptions;
            _serviceProvider = serviceProvider;
            _identityGenerator = identityGenerator;
            _dbContext = _serviceProvider.GetRequiredService(_eventBusEFOptions.DbContextType) as DbContext;
        }
        
        public long TransactionID { get; }


        public async Task ChangeStateAsync(long messageId, long transactId, MessageState messageState)
        {
            var connection = _dbContext.Database.GetDbConnection();
            var transaction = GetDbTransaction();
            await ChangeStateAsync(messageId, transactId, messageState, connection, transaction);
        }

        public async Task ChangeStateAsync(long messageId, long transactId, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var result = await dbConnection.QueryFirstOrDefaultAsync<ChangeStateMessage<TContent>>(BuildChangeStateSql(),
                new
                {
                    MessageId = messageId,
                    TransactId = transactId
                }, dbTransaction);

            var args = new StateChangedArgs(result.State, messageState);
            var contentObj = result.GetContent();
            var metaDataObj = result.GetMetaData();
            var stateChangeHandlers = _serviceProvider.GetServices<IStateChangeHandler>().Where(handler => handler.CanHandle(typeof(TContent), contentObj, metaDataObj, args));
            if (stateChangeHandlers.Any())
            {
                foreach(var handler in stateChangeHandlers)
                {
                    try
                    {
                        await handler.HandleAsync(typeof(TContent), contentObj, metaDataObj, args);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"An exception was thrown during {handler.GetType().FullName} executing.{ex.Message}");
                    }
                }
            }
        }

        public async Task<IMessage<TContent>> InsertAsync(TContent content, MessageType messageType, IMetaData metaData = null)
        {
            var connection = _dbContext.Database.GetDbConnection();
            var transaction = GetDbTransaction();
            return await InsertAsync(content, messageType, connection, transaction, metaData);            
        }

        public async Task<IMessage<TContent>> InsertAsync(TContent content, MessageType messageType, IDbConnection dbConnection, IDbTransaction dbTransaction, IMetaData metaData = null)
        {
            var message = new DefaultMessage<TContent>(content);
            if (metaData != null)
            {
                message.MetaData.Unoin(metaData);
            }
            var metaJson = JsonConvert.SerializeObject(message.MetaData);
            var contentJson = JsonConvert.SerializeObject(message.Content);

            var affectedRows = await dbConnection.ExecuteAsync(BuildInsertSql(),
                new Internal.Model.Message
                {
                    Id = _identityGenerator.NextIdentity(),
                    MessageId = _identityGenerator.NextIdentity(),
                    TransactId = TransactionID,
                    MetaData = metaJson,
                    Content = contentJson,
                    Type = (short)messageType,
                    State = (short)message.State,
                    CreationDate = message.CreateTime
                }, dbTransaction);

            if (affectedRows == 0) throw new AffectedRowsCountUnExpectedException(1, affectedRows);
            return message;
        }

        private IDbTransaction GetDbTransaction()
        {
            var transaction = _dbContext?.Database?.CurrentTransaction;
            if (transaction == null)
            {
                _logger.MessagePersitenterNotUsingTransaction(TransactionID);
                return null;
            }
            return transaction.GetDbTransaction();
        }

        private string BuildInsertSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"INSERT INTO `eventbus.messages` (");
            sql.AppendLine(@"`Id`,");
            sql.AppendLine(@"`MessageId`,");
            sql.AppendLine(@"`TransactId`,");
            sql.AppendLine(@"`MetaData`,");
            sql.AppendLine(@"`Content`,");
            sql.AppendLine(@"`Type`,");
            sql.AppendLine(@"`State`,");
            sql.AppendLine(@"`CreationDate`");
            sql.AppendLine(@")");
            sql.AppendLine(@"VALUES");
            sql.AppendLine(@"(");
            sql.AppendLine(@"`@Id`,");
            sql.AppendLine(@"`@MessageId`,");
            sql.AppendLine(@"`@TransactId`,");
            sql.AppendLine(@"`@MetaData`,");
            sql.AppendLine(@"`@Content`,");
            sql.AppendLine(@"`@Type`,");
            sql.AppendLine(@"`@State`,");
            sql.AppendLine(@"`@CreationDate`");
            sql.AppendLine(@")");
            return sql.ToString();
        }

        private string BuildChangeStateSql()
        {
            var sql = new StringBuilder();
            sql.AppendLine(@"SELECT");
            sql.AppendLine(@"`State`,");
            sql.AppendLine(@"`MetaData`,");
            sql.AppendLine(@"`Content`");
            sql.AppendLine(@"FROM");
            sql.AppendLine(@"`eventbus.messages`");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(@"	(`MessageId` = @MessageId) AND (`TransactId` = @TransactId);");

            sql.AppendLine(@"UPDATE `CAP_Consumer`.`Messages`");
            sql.AppendLine(@"SET ");
            sql.AppendLine(@"`State` = NULL,");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(@"	(`MessageId` = @MessageId) AND (`TransactId` = @TransactId);");
            return sql.ToString(); 
        }
        
    }

    internal class ChangeStateMessage<TContent>
    {
        public MessageState State { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }

        public TContent GetContent() => JsonConvert.DeserializeObject<TContent>(Content);
        public IMetaData GetMetaData()
        {
            if (string.IsNullOrEmpty(MetaData)) return null;
            return JsonConvert.DeserializeObject<MetaData>(MetaData);
        }
    }

}
