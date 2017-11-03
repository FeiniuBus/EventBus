using Dapper;
using EventBus.Core;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Subscribe
{
    public class ReceivedEventPersistenter : IReceivedEventPersistenter
    {
        private const string TableName = "eventbus.receivedmessages";

        private readonly EventBusMySQLOptions _eventBusMySQLOptions;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ReceivedEventPersistenter(EventBusMySQLOptions eventBusMySQLOptions,
            ILogger<ReceivedEventPersistenter> logger,
            IServiceProvider serviceProvider)
        {
            _eventBusMySQLOptions = eventBusMySQLOptions;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task ChangeStateAsync(long messageId, long transactId, string group, MessageState messageState)
        {
            var sql = BuildChangeStateSQL(false);
            var parameters = new { State = (short)messageState, MessageId = messageId, TransactId = transactId, Group = group };
            ChangeStateMessage result;
            using (var connection = await OpenDbConnectionAsync())
            {
                result = await connection.QueryFirstOrDefaultAsync<ChangeStateMessage>(sql, parameters);
            }
            if (result == null) throw new AffectedRowsCountUnExpectedException(1, 0);

            //var args = new StateChangedArgs(result.State, messageState);
            //var metaDataObj = result.GetMetaData();
            //var stateChangeHandlers = _serviceProvider.GetServices<IStateChangeHandler>().Where(handler => handler.CanHandle(MessageType.Subscribed, result.Content, metaDataObj, args));
            //if (stateChangeHandlers.Any())
            //{
            //    foreach (var handler in stateChangeHandlers)
            //    {
            //        try
            //        {
            //            await handler.HandleAsync(MessageType.Subscribed, result.Content, metaDataObj, args);
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError($"An exception was thrown during {handler.GetType().FullName} executing.{ex.Message}");
            //        }
            //    }
            //}

        }

        public async Task ChangeStateAsync(long id, MessageState messageState)
        {
            var sql = BuildChangeStateSQL(true);
            var parameters = new { State = (short)messageState, Id = id};
            using (var connection = await OpenDbConnectionAsync())
            {
                var affectedRows = await connection.ExecuteAsync(sql, parameters);
                if (affectedRows != 1) throw new AffectedRowsCountUnExpectedException(1, affectedRows);
            }
        }

        public async Task EnsureCreatedAsync()
        {
            var sql = new StringBuilder();
            sql.AppendLine($@"CREATE TABLE IF NOT EXISTS `{TableName}` (");
            sql.AppendLine(@"  `Id` bigint(20) NOT NULL, ");
            sql.AppendLine(@"  `MessageId` bigint(20) DEFAULT NULL,");
            sql.AppendLine(@"  `TransactId` bigint(20) DEFAULT NULL, ");
            sql.AppendLine(@"  `Group` varchar(255) DEFAULT NULL,");
            sql.AppendLine(@" `RouteKey` varchar(255) DEFAULT NULL, ");
            sql.AppendLine(@" `MetaData` longtext,");
            sql.AppendLine(@" `Content` longtext, ");
            sql.AppendLine(@"  `ReceivedTime` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,");
            sql.AppendLine(@"  `State` tinyint(4) DEFAULT NULL, ");
            sql.AppendLine(@" PRIMARY KEY (`Id`),");
            sql.AppendLine(@"  UNIQUE KEY `Index_EventBus_ReceivedMessage_IDs` (`MessageId`,`TransactId`,`Group`) USING BTREE");
            sql.AppendLine(@") ENGINE=InnoDB DEFAULT CHARSET=utf8;");
            try
            {
                using (var connection = await OpenDbConnectionAsync())
                {
                    await connection.ExecuteAsync(sql.ToString());
                }
            }
            catch { }
        }

        public async Task InsertAsync(object message)
        {
            var sql = BuildInsertSQL();
            using (var connection = await OpenDbConnectionAsync())
            {
                var affectedRows = await connection.ExecuteAsync(sql, message);
                if (affectedRows == 0) throw new AffectedRowsCountUnExpectedException(1, affectedRows);
            }
        }

        private async Task<IDbConnection> OpenDbConnectionAsync()
        {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(_eventBusMySQLOptions.ConnectionString);
            await conn.OpenAsync();
            return conn;
        }

        public async Task<IDbTransaction> BeginTransaction(IDbConnection connection)
        {
            return await Task.Run(() =>
            {
                var transaction = connection.BeginTransaction();
                return transaction;
            });
        }

        private string BuildInsertSQL()
        {
            var sql = new StringBuilder();
            sql.AppendLine($@"INSERT INTO `{TableName}` (");
            sql.AppendLine("	`Id`, ");
            sql.AppendLine("	`MessageId`,");
            sql.AppendLine("	`TransactId`,");
            sql.AppendLine("	`Group`,");
            sql.AppendLine("	`RouteKey`,");
            sql.AppendLine("	`MetaData`,");
            sql.AppendLine("	`Content`,");
            sql.AppendLine("	`ReceivedTime`,");
            sql.AppendLine("	`State`");
            sql.AppendLine(")");
            sql.AppendLine("VALUES");
            sql.AppendLine("   (");
            sql.AppendLine("	@Id, ");
            sql.AppendLine("@MessageId,");
            sql.AppendLine("@TransactId,");
            sql.AppendLine("@Group,");
            sql.AppendLine("@RouteKey,");
            sql.AppendLine("@MetaData,");
            sql.AppendLine("@Content,");
            sql.AppendLine("@ReceivedTime,");
            sql.AppendLine("@State");
            sql.AppendLine("	);");
            return sql.ToString();
        }

        private string BuildChangeStateSQL(bool useLocalId)
        {
            string whereClause = "";
            if (useLocalId)
                whereClause = @"(`Id` = @Id);";
            else
                whereClause = @"(`MessageId` = @MessageId) AND (`TransactId` = @TransactId) AND (`Group` = @Group);";


            var sql = new StringBuilder();

            sql.AppendLine(@"SELECT");
            sql.AppendLine(@"`State`,");
            sql.AppendLine(@"`Group`");
            sql.AppendLine(@"`MetaData`,");
            sql.AppendLine(@"`Content`");
            sql.AppendLine(@"FROM");
            sql.AppendLine($@"`{TableName}`");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(whereClause);

            sql.AppendLine($@"UPDATE `{TableName}` SET");
            sql.AppendLine(@" `State` = @State");
            sql.AppendLine(@"WHERE");
            sql.AppendLine(whereClause);

            return sql.ToString();
        }
    }

    internal class ChangeStateMessage
    {
        public MessageState State { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string Group { get; set; }

        public IMetaData GetMetaData()
        {
            if (string.IsNullOrEmpty(MetaData)) return null;
            return FeiniuBus.Util.FeiniuBusJsonConvert.DeserializeObject<MetaData>(MetaData);
        }
    }
}
