using Dapper;
using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using EventBus.Publish.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public class PublishedEventPersistenter : IPublishedEventPersistenter
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public PublishedEventPersistenter(IIdentityGenerator identityGenerator,IServiceProvider serviceProvider, ILogger<PublishedEventPersistenter> logger)
        {
            _identityGenerator = identityGenerator;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ChangeStateAsync(long messageId, long transactId, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var result = await dbConnection.QueryFirstOrDefaultAsync<ChangeStateMessage>(BuildChangeStateSql(),
                new
                {
                    MessageId = messageId,
                    TransactId = transactId
                }, dbTransaction);

            var args = new StateChangedArgs(result.State, messageState);
            var contentType = result.GetContentType();
            var metaDataObj = result.GetMetaData();
            var stateChangeHandlers = _serviceProvider.GetServices<IStateChangeHandler>().Where(handler => handler.CanHandle(contentType, result.Content, metaDataObj, args));
            if (stateChangeHandlers.Any())
            {
                foreach(var handler in stateChangeHandlers)
                {
                    try
                    {
                        await handler.HandleAsync(contentType, result.Content, metaDataObj, args);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"An exception was thrown during {handler.GetType().FullName} executing.{ex.Message}");
                    }
                }
            }
        }

        public async Task InsertAsync(object message, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var affectedRows = await dbConnection.ExecuteAsync(BuildInsertSql(), message, dbTransaction);

            if (affectedRows == 0) throw new AffectedRowsCountUnExpectedException(1, affectedRows);
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
            sql.AppendLine(@"`Exchange`,");
            sql.AppendLine(@"`RouteKey`,");
            sql.AppendLine(@"`Type`,");
            sql.AppendLine(@"`State`,");
            sql.AppendLine(@"`CreationDate`");
            sql.AppendLine(@")");
            sql.AppendLine(@"VALUES");
            sql.AppendLine(@"(");
            sql.AppendLine(@"@Id,");
            sql.AppendLine(@"@MessageId,");
            sql.AppendLine(@"@TransactId,");
            sql.AppendLine(@"@MetaData,");
            sql.AppendLine(@"@Content,");
            sql.AppendLine(@"@Exchange,");
            sql.AppendLine(@"@RouteKey,");
            sql.AppendLine(@"@Type,");
            sql.AppendLine(@"@State,");
            sql.AppendLine(@"@CreationDate");
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

    internal class ChangeStateMessage
    {
        public MessageState State { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        
        public IMetaData GetMetaData()
        {
            if (string.IsNullOrEmpty(MetaData)) return null;
            return JsonConvert.DeserializeObject<MetaData>(MetaData);
        }

        public Type GetContentType()
        {
            return Type.GetType(ContentType);
        }
    }

}
