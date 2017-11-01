using Dapper;
using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
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
        private const string TableName = "eventbus.publishedmessages";
        private readonly EventBusMySQLOptions _eventBusMySQLOptions;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public PublishedEventPersistenter(IIdentityGenerator identityGenerator, EventBusMySQLOptions eventBusMySQLOptions, IServiceProvider serviceProvider, ILogger<PublishedEventPersistenter> logger)
        {
            _identityGenerator = identityGenerator;
            _eventBusMySQLOptions = eventBusMySQLOptions;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ChangeStateAsync(long messageId, long transactId, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var result = await dbConnection.QueryFirstOrDefaultAsync<ChangeStateMessage>(BuildChangeStateSql(),
                new
                {
                    State = (short)messageState,
                    MessageId = messageId,
                    TransactId = transactId
                }, dbTransaction);

            if (result == null) throw new AffectedRowsCountUnExpectedException(1, 0);

            var args = new StateChangedArgs(result.State, messageState);
            var metaDataObj = result.GetMetaData();
            var stateChangeHandlers = _serviceProvider.GetServices<IStateChangeHandler>().Where(handler => handler.CanHandle(MessageType.Published, result.Content, metaDataObj, args));
            if (stateChangeHandlers.Any())
            {
                foreach(var handler in stateChangeHandlers)
                { 
                    try
                    {
                        await handler.HandleAsync(MessageType.Published, result.Content, metaDataObj, args);
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
            sql.AppendLine($@"INSERT INTO `{TableName}` (");
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
            sql.AppendLine($@"`{TableName}`");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(@"	(`MessageId` = @MessageId) AND (`TransactId` = @TransactId);");

            sql.AppendLine($@"UPDATE `{TableName}`");
            sql.AppendLine(@"SET ");
            sql.AppendLine(@"`State` = @State,");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(@"	(`MessageId` = @MessageId) AND (`TransactId` = @TransactId);");
            return sql.ToString(); 
        }

        public async Task EnsureCreatedAsync()
        {
            var sql = new StringBuilder();
            sql.AppendLine($@"CREATE TABLE IF NOT EXISTS `{TableName}` (");
            sql.AppendLine(@"  `Id` bigint(20) NOT NULL, ");
            sql.AppendLine(@"  `MessageId` bigint(20) NOT NULL,");
            sql.AppendLine(@"  `TransactId` bigint(20) NOT NULL, ");
            sql.AppendLine(@"  `MetaData` longtext NOT NULL,");
            sql.AppendLine(@"  `Content` longtext NOT NULL, ");
            sql.AppendLine(@"  `Exchange` varchar(255) DEFAULT NULL,");
            sql.AppendLine(@"  `RouteKey` varchar(255) DEFAULT NULL, ");
            sql.AppendLine(@"  `Type` tinyint(4) NOT NULL,");
            sql.AppendLine(@"  `State` tinyint(4) NOT NULL, ");
            sql.AppendLine(@"  `CreationDate` datetime NOT NULL,");
            sql.AppendLine(@"  PRIMARY KEY(`Id`)");
            sql.AppendLine(@") ENGINE=InnoDB DEFAULT CHARSET=utf8;");

            try
            {
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(_eventBusMySQLOptions.ConnectionString))
                {
                    await connection.ExecuteAsync(sql.ToString());
                }
            }
            catch { }
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
            return FeiniuBus.AspNetCore.Json.FeiniuBusJsonConvert.DeserializeObject<MetaData>(MetaData);
        }

        public Type GetContentType()
        {
            return Type.GetType(ContentType);
        }
    }

}
