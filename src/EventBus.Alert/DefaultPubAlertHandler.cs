using EventBus.Core;
using FeiniuBusSDK.Notification;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Alert
{
    public class DefaultPubAlertHandler : IPubFailureHandler
    {
        private readonly SMSAlertOptions _options;
        private readonly ILastAlertMemento _memento;
        private readonly IFeiniuBusNotification _feiniuBusNotification;
        private readonly ILogger<DefaultPubAlertHandler> _logger;

        public DefaultPubAlertHandler(IOptions<SMSAlertOptions> optionsAccessor
            , ILastAlertMemento memento
            , IFeiniuBusNotification feiniuBusNotification
            , ILogger<DefaultPubAlertHandler> logger)
        {
            _options = optionsAccessor.Value;
            _memento = memento;
            _feiniuBusNotification = feiniuBusNotification;
            _logger = logger;
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

            var request = new FeiniuBusSDK.Notification.Model.CreateSmsRequest
            {
                Numbers = _options.Contacts,
                Message = CreateMessage(exchange, topic, content)
            };

            try
            {
                await _feiniuBusNotification.CreateSmsAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(110, ex, $"订阅消息失败通知发送失败");
            }
        }

        private static string CreateMessage(string exchange, string topic, byte[] content)
        {
            return $"你的主题{topic}发送失败，请及时处理。";
        }
    }
}
