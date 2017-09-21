using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Core.Extensions
{
    public static class ObjectExtenssion
    {
        public static string ToJson(this object obj)
        {
            return FeiniuBus.Util.FeiniuBusJsonConvert.SerializeObject(obj);
        }
    }
}
