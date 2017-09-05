using EventBus.Core;
using FeiniuBus;

namespace EventBus.MySQL
{
    public class IdentityGenerator : IIdentityGenerator
    {
        public long NextIdentity()
        {
            return Puid.NewPuid();
        }
    }
}
