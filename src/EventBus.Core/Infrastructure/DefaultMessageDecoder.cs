using System.Text;
using Newtonsoft.Json.Linq;
using EventBus.Core;
using EventBus.Core.Internal.Model;
using FeiniuBus;
using System;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultMessageDecoder : IMessageDecoder
    {
        public ReceivedMessage Decode(MessageContext context)
        {
            var json = Encoding.UTF8.GetString(context.Content);
            var jobject = JObject.Parse(json);

            try
            {
                var returnValue = new ReceivedMessage();
                returnValue.Id = Puid.NewPuid();
                returnValue.MessageId = jobject["MetaData"]["MessageID"].Value<long>();
                returnValue.TransactId = jobject["MetaData"]["TransactID"].Value<long>();
                returnValue.Group = context.Queue;
                returnValue.RouteKey = context.Topic;
                returnValue.MetaData = jobject["MetaData"].ToString();
                returnValue.Content = jobject["Content"].ToString();
                returnValue.Retries = 0;
                returnValue.ReceivedTime = DateTime.Now;
                returnValue.ExpiredTime = DateTime.Now.AddDays(1);
                returnValue.State = Core.State.MessageState.Processing;
                return returnValue;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
