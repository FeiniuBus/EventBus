using EventBus.Core.Infrastructure;
using EventBus.Core.Internal;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Core
{
    public static class IEventPublisherExtensions
    {
        public static async Task PrepareAsync(this IEventPublisher eventPublisher, string routeKey, object content, object metaData, string exchange = "default.exchange@feiniubus", object args = null)
        {
            var message = new DefaultMessage(content);

            var anonyObj = new AnonymousObject(metaData);
            var anonyMembers = anonyObj.GetProperties().Where(member => member.MemberType == System.Reflection.MemberTypes.Property)
                .Select(member=> new { MemberName = member.MemberName, Value = anonyObj.GetValue(member.DeclaringType, member.MemberName) });

            if (anonyMembers.Any())
            {
                var md = new MetaData();
                foreach (var member in anonyMembers)
                {
                    md.Set(member.MemberName, member.Value.ToString());
                }
                message.MetaData.Contact(md);
            }

            await eventPublisher.PrepareAsync(new EventDescriptor(routeKey, message, exchange, args));
        }
    }
}
