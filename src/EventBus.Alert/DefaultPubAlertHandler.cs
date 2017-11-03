using EventBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Polaris.FNS;
using Polaris.FNS.Model;

namespace EventBus.Alert
{
    public class DefaultPubAlertHandler : IPubFailureHandler
    {
        private readonly SMSAlertOptions _options;
        private readonly ILastAlertMemento _memento;
        private readonly ILogger<DefaultPubAlertHandler> _logger;
        private readonly IPolarisFns _fns;

        public DefaultPubAlertHandler(IOptions<SMSAlertOptions> optionsAccessor
            , ILastAlertMemento memento
            , IPolarisFns fns
            , ILogger<DefaultPubAlertHandler> logger)
        {
            _options = optionsAccessor.Value;
            _memento = memento;
            _logger = logger;
            _fns = fns;
        }

        public async Task HandleAsync(string exchange, string topic, byte[] content)
        {
            if (!_options.Enable)
            {
                return;
            }

            if (!_options.Contacts.Any())
            {
                return;
            }

            var now = DateTime.Now;

            if ((now - _memento.LastSubAlert).TotalSeconds < _options.AlertIntervalSecs)
            {
                return;
            }

            _memento.LastSubAlert = now;

            try
            {
                await _fns.SendSmsAsync(new SendSmsRequest
                {
                    Numbers = _options.Contacts,
                    Message = CreateMessage(exchange, topic, content),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(110, ex, $"订阅消息失败通知发送失败");
            }
        }

        private static string CreateMessage(string exchange, string topic, byte[] content)
        {
            return $"你的消息exchange={exchange} topic={topic} content={content}发送失败";
        }
    }
}
