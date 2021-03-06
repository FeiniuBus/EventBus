﻿using System.Threading.Tasks;
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

        public async Task HandleAsync(FailContext context)
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

        private static string CreateMessage(FailContext context)
        {
            var msg = "";

            if (context.Raw != null)
            {
                msg = $"您订阅的消息{context.Raw}处理失败，失败原因{context.ExceptionMessage}";
            }

            if (context.State is ReceivedMessage receivedMessage)
            {

                msg = $"您订阅的消息处理失败，Id={receivedMessage.Id},TransactId={receivedMessage.TransactId},Group={receivedMessage.Group},Topic={receivedMessage.RouteKey},失败原因:{context.ExceptionMessage}";
            }

            return msg;
        }
    }
}
