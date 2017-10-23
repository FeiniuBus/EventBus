using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.Core.Test
{
    public partial class PublishTest
    {
		[Fact]
        public async Task MessageQueueTransaction()
		{
		    var transaction = Provider.GetRequiredService<IMessageQueueTransaction>();
		    await transaction.PublishAsync("testing.exchange", "testing.key", new[] {(byte)1, (byte)1, (byte)1 });
		    await transaction.CommitAsync();
		}
    }
}
