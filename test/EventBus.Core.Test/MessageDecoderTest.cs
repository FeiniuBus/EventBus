using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using EventBus.Subscribe.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Text;
using Xunit;

namespace EventBus.Core.Test
{
    public class MessageDecoderTest
    {
        [Fact]
        public void MessageDecode()
        {
            var decoder = new DefaultMessageDecoder();
            var message = new EventBus.Publish.Internal.Model.Message
            {
                Id = 1,
                MessageId = 1,
                TransactId = 1,
                Content = "HelloWorld",
                CreationDate = DateTime.Now,
                State = MessageState.Processing,
                Type = MessageType.Published,
                Exchange = "testExchange",
                RouteKey = "testRoutingKey"
            };

            var metaData = new MetaData();
            metaData.Set("ContentType", "");
            metaData.Set("TransactID", message.TransactId.ToString());
            metaData.Set("MessageID", message.MessageId.ToString());

            var jsonTransafer = JsonConvert.SerializeObject(new { MetaData = metaData.GetDictionary(), Content = "HelloWorld" });
            var bytes = Encoding.UTF8.GetBytes(jsonTransafer);

            var context = new MessageContext
            {
                Content = bytes
            };

            var received = decoder.Decode(context);

            Assert.Equal("HelloWorld", received.Content);
        }
    }
}
