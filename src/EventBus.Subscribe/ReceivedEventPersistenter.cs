using EventBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using EventBus.Core.State;
using System.Data;
using System.Threading.Tasks;
using EventBus.Core.Infrastructure;

namespace EventBus.Subscribe
{
    public class ReceivedEventPersistenter : IReceivedEventPersistenter
    {
        private readonly EventBusMySQLOptions _eventBusMySQLOptions;

        public ReceivedEventPersistenter(EventBusMySQLOptions eventBusMySQLOptions)
        {
            _eventBusMySQLOptions = eventBusMySQLOptions;
        }

        public Task ChangeStateAsync(long messageId, long transactId, string group, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            throw new NotImplementedException();
        }

        public Task ChangeStateAsync(long id, MessageState messageState, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            throw new NotImplementedException();
        }

        public Task EnsureCreated()
        {
            var sql = new StringBuilder();
            sql.AppendLine(@"CREATE TABLE IF NOT EXISTS `eventbus.receivedmessages` (");
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
            return Task.CompletedTask;
        }

        public Task InsertAsync(object message, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            throw new NotImplementedException();
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

    }
}
