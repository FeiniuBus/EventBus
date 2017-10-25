using System.Threading.Tasks;
using EventBus.Core;
using Microsoft.Extensions.Options;
using System;
using FeiniuBusSDK.Notification;
using System.Linq;
using EventBus.Core.Internal.Model;
using Microsoft.Extensions.Logging;

namespace EventBus.Alert
{
    public class DefaultSubAlertHandler : ISubFailureHandler
    {
        private readonly SMSAlertOptions _options;
        private readonly ILastAlertMemento _memento;
        private readonly IFeiniuBusNotification _feiniuBusNotification;
        private readonly IMessageDecoder _decoder;
        private readonly ILogger<DefaultSubAlertHandler> _logger;

        public DefaultSubAlertHandler(IOptions<SMSAlertOptions> optionsAccessor
            , ILastAlertMemento memento
            , IFeiniuBusNotification feiniuBusNotification
            , IMessageDecoder decoder
            , ILogger<DefaultSubAlertHandler> logger)
        {
            _options = optionsAccessor.Value;
            _memento = memento;
            _decoder = decoder;
            _feiniuBusNotification = feiniuBusNotification;
            _logger = logger;
        }

        public async Task HandleAsync(ReceivedMessage message)
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
                Message = CreateMessage(message)
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

        private string CreateMessage(ReceivedMessage message)
        {
            return $"你以{message.Group}订阅的主题{message.RouteKey}消息Id:{message.Id}处理失败，请即时处理。";
        }
    }
}
