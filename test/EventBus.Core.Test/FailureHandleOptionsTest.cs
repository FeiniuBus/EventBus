using EventBus.Core.Infrastructure;
using Xunit;
using System.Linq;

namespace EventBus.Core.Test
{
    public class FailureHandleOptionsTest
    {
        [Fact]
        public void RouterInfos()
        {
            var options = new FailureHandleOptions();

            options.RegisterFailureCallback("topic1", typeof(string));
            options.RegisterFailureCallback("topic2", "exchange", typeof(string));

            options.BuildWithDefaultSelfExchangeName("exchange", "deadletterExchange");

            Assert.Equal(2, options.DeadLetterInfos.Count);
            Assert.Equal(1, options.DeadLetterInfos.Count(x => x.Topic == "topic1"));
            Assert.Equal(1, options.DeadLetterInfos.Count(x => x.Topic == "topic2"));
            Assert.Equal(2, options.DeadLetterInfos.Count(x => x.Exchange == "deadletterExchange"));
        }
    }
}
