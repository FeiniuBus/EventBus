using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using EventBus.Publish.Internal.Model;
using FeiniuBus;
using Xunit;

namespace EventBus.Core.Test
{
    public partial class PublishTest
    {
        [Fact]
        public async Task MessagePersistenerTest()
        {
            var persistener = Provider.GetRequiredService<IPublishedEventPersistenter>();
            var dbContext = Provider.GetRequiredService<TestDbContext>();
            var connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            await persistener.EnsureCreatedAsync();
            await persistener.InsertAsync(new Message
            {
                Id= Puid.NewPuid(),
                Content =  "tesing",
                CreationDate = DateTime.Now,
                Exchange = "tesing.exchange",
                MessageId = Puid.NewPuid(),
                MetaData = "{}",
                RouteKey = "tesing.key",
                State = MessageState.Processing,
                TransactId =  Puid.NewPuid(),
                Type =  MessageType.Published
            }, connection, transaction);
            transaction.Commit();
            connection.Close();
            connection.Dispose();
        }
    }
}
