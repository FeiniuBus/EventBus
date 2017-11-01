using Newtonsoft.Json;
using System;
using System.Text;

namespace EventBus.Core.Infrastructure
{
    public class DefaultMessageSerializer : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message)
        {
            var jsonStr = Encoding.UTF8.GetString(message);
            return FeiniuBus.AspNetCore.Json.FeiniuBusJsonConvert.DeserializeObject<T>(jsonStr);
        }

        public byte[] Serialize(object message)
        {
            string jsonStr = string.Empty;
            if (message is string)
            {
                jsonStr = (string)message;
            }
            else
            {
                jsonStr = FeiniuBus.AspNetCore.Json.FeiniuBusJsonConvert.SerializeObject(message);
            }
            return Encoding.UTF8.GetBytes(jsonStr);
        }

        private bool IsSimpleType(Type t)
        {
            return t == typeof(string)
                || t == typeof(Int16)
                || t == typeof(Int32)
                || t == typeof(Int64)
                || t == typeof(IntPtr)
                || t == typeof(UInt16)
                || t == typeof(UInt32)
                || t == typeof(UInt64)
                || t == typeof(UIntPtr)
                || t == typeof(Single)
                || t == typeof(Double)
                || t == typeof(Decimal)
                || t == typeof(Byte)
                || t == typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(TimeSpan);
        }

        private SimpleTypes GetSimpleType(Type t)
        {
            if (t == typeof(Int16)) return SimpleTypes.Int16;
            else if (t == typeof(Int32)) return SimpleTypes.Int32;
            else if (t == typeof(Int64)) return SimpleTypes.Int64;
            else if (t == typeof(IntPtr)) return SimpleTypes.IntPtr;
            else if (t == typeof(UInt16)) return SimpleTypes.UInt16;
            else if (t == typeof(UInt32)) return SimpleTypes.UInt32;
            else if (t == typeof(UInt64)) return SimpleTypes.UInt64;
            else if (t == typeof(UIntPtr)) return SimpleTypes.UIntPtr;
            else if (t == typeof(Single)) return SimpleTypes.Single;
            else if (t == typeof(Double)) return SimpleTypes.Double;
            else if (t == typeof(Decimal)) return SimpleTypes.Decimal;
            else if (t == typeof(Byte)) return SimpleTypes.Byte;
            else if (t == typeof(DateTime)) return SimpleTypes.DateTime;
            else if (t == typeof(DateTimeOffset)) return SimpleTypes.DateTimeOffset;
            else if (t == typeof(TimeSpan)) return SimpleTypes.TimeSpan;
            else return SimpleTypes.String;
        }

    }
}
