using EventBus.Core.Infrastructure;
using Newtonsoft.Json;

namespace EventBus.Core.Extensions
{
    public static class IMetaDataExtensions
    {
        public static string ToJson(this IMetaData metaData)
        {
            return JsonConvert.SerializeObject(metaData.GetDictionary());
        }

        public static IMetaData Union(this IMetaData metaData, IMetaData source)
        {
            metaData.Contact(source);
            return metaData;
        }
    }
}
