using System;

namespace EventBus.Core.Infrastructure
{
    public static class MessageInfoCacheExtenssion
    {
        public static string GetMessageName<MessageT>(this MessageInfoCache @this) where MessageT: class
        {
            return @this.GetMessageName(typeof(MessageT));
        }

        public static string GetMessageName(this MessageInfoCache @this, Type type)
        {
            return @this.GetMessageInfo(type)?.Name;
        }

        public static string GetRequiredMessageName<MessageT>(this MessageInfoCache @this) where MessageT : class
        {
            return @this.GetRequiredMessageName(typeof(MessageT));
        }

        public static string GetRequiredMessageName(this MessageInfoCache @this, Type type)
        {
            return @this.GetMessageInfo(type)?.Name ?? throw new InvalidOperationException($"Not exist message type {type}");
        }

        public static string GetMessageKey<MessageT>(this MessageInfoCache @this) where MessageT : class
        {
            return @this.GetMessageKey(typeof(MessageT));
        }

        public static string GetMessageKey(this MessageInfoCache @this, Type type)
        {
            return @this.GetMessageInfo(type)?.Key;
        }

        public static string GetRequiredMessageKey<MessageT>(this MessageInfoCache @this) where MessageT : class
        {
            return @this.GetRequiredMessageKey(typeof(MessageT));
        }

        public static string GetRequiredMessageKey(this MessageInfoCache @this, Type type)
        {
            return @this.GetMessageInfo(type)?.Key ?? throw new InvalidOperationException($"Not exist message type {type}");
        }
    }
}
