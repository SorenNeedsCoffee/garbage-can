﻿using GarbageCan.Application.Common.Models;
using GarbageCan.Application.Roles.Commands.AlterRole;
using GarbageCan.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.EventHandlers
{
    public class DiscordMessageReactionAddedEventHandler : INotificationHandler<DomainEventNotification<DiscordMessageReactionAddedEvent>>
    {
        private readonly IMediator _mediator;

        public DiscordMessageReactionAddedEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(DomainEventNotification<DiscordMessageReactionAddedEvent> notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(new AlterRoleCommand
            {
                ChannelId = notification.DomainEvent.ChannelId,
                Emoji = notification.DomainEvent.Emoji,
                GuildId = notification.DomainEvent.GuildId,
                MessageId = notification.DomainEvent.MessageId,
                UserId = notification.DomainEvent.UserId,
                Add = true
            }, cancellationToken);
        }
    }
}