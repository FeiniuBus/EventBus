using System.Threading.Tasks;

namespace EventBus.Core
{
    public interface IPubMessageValidator
    {
        void Validate(PubMessageValidateContext context);
    }
}
