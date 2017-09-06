using EventBus.Core;
using FeiniuBus;

namespace EventBus.Publish
{
    public class IdentityGenerator : IIdentityGenerator
    {
        public long NextIdentity()
        {
            return Puid.NewPuid();
        }
    }
}
