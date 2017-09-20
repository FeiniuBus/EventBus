using System.Threading.Tasks;
using EventBus.Core;
using Microsoft.Extensions.Options;
using System;
using FeiniuBusSDK.Notification;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EventBus.Alert
{
    public class DefaultSubAlertHandler : ISubFailureHandler
    {
        private readonly SMSAlertOptions _options;
        private readonly ILastAlertMemento _memento;
        private readonly IFeiniuBusNotification _feiniuBusNotification;
        private readonly ILogger<DefaultSubAlertHandler> _logger;

        public DefaultSubAlertHandler(IOptions<SMSAlertOptions> optionsAccessor
            , ILastAlertMemento memento
            , IFeiniuBusNotification feiniuBusNotification
            , ILogger<DefaultSubAlertHandler> logger)
        {
            _options = optionsAccessor.Value;
            _memento = memento;
            _feiniuBusNotification = feiniuBusNotification;
            _logger = logger;
        }

        public async Task HandleAsync(MessageContext context)
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

            if ((now - _memento.LastPubAlert).TotalSeconds < _options.AlertIntervalSecs)
            {
                return;
            }

            _memento.LastPubAlert = now;

            var request = new FeiniuBusSDK.Notification.Model.CreateSmsRequest
            {
                Numbers = _options.Contacts,
                Message = CreateMessage(context)
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

        private static string CreateMessage(MessageContext context)
        {
            return $"你以{context.Queue}订阅的主题{context.Topic}处理失败，请即时处理。";
        }
    }
}
