﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EA.Audit.Common.Application.Features.Audits.Commands;
using EA.Audit.Common.Infrastructure;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EA.Audit.Subscriber
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly ILogger<RedisSubscriberService> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMediator _mediator;

        public RedisSubscriberService(ILogger<RedisSubscriberService> logger, IConnectionMultiplexer connectionMultiplexer, IMediator mediator)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(Constants.Redis.AuditChannel, async (channel, value) =>
            {
                _logger.LogInformation($"{DateTime.Now:yyyyMMdd HH:mm:ss}<{value.ToString()}>.");

                var command = JsonConvert.DeserializeObject<CreateAuditCommand>(value);

                await _mediator.Send(command, stoppingToken);
            });
        }
    }
}
