﻿using System;

namespace EventBus.Subscribe
{
    public interface ISubscribeClient: IDisposable
    {
        Action<SubscribeContext> OnReceive { get; set; }

        void Start();
        void Ack(SubscribeContext context);
        void Reject(SubscribeContext context);
    }
}
