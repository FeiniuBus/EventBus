using EventBus.Core;
using FeiniuBus;

namespace EventBus.Core
{
    public class IdentityGenerator : IIdentityGenerator
    {
        public long NextIdentity()
        {
            return Puid.NewPuid();
        }
    }
}
