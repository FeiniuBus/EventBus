using Microsoft.AspNetCore.Hosting;
using System;

namespace EventBus.Core.Infrastructure
{
    public class DefaultEnviromentalNameUpdator : IEnviromentNameConcator
    {
        private readonly IHostingEnvironment _env;

        public DefaultEnviromentalNameUpdator(IHostingEnvironment env)
        {
            _env = env;
        }

        public string Concat(string str)
        {
            if (!str.EndsWith(_env.EnvironmentName))
            {
                return string.Concat(str, ".", _env.EnvironmentName);
            }
            return str;
        }
    }
}
