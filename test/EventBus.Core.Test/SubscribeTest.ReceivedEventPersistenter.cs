using EventBus.Core.State;
using FeiniuBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.Core.Test
{
    public partial class SubscribeTest
    {
        [Fact]
        public async Task ReceivedEventPersistenterTest()
        {
            var persistenter = Provider.GetRequiredService<IReceivedEventPersistenter>();
            await persistenter.EnsureCreatedAsync();
            await persistenter.InsertAsync(new
            {
                Id = Puid.NewPuid(),
                MessageId = Puid.NewPuid(),
                TransactId = Puid.NewPuid(),
                Group = "testGroup",
                RouteKey = "test.key",
                MetaData = "{}",
                Content = @"{""title"":""testing""}",
                ReceivedTime = DateTime.Now,
                State = MessageState.Succeeded
            });
        }


    }
}
