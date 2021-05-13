﻿using GarbageCan.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GarbageCan.Application.Roles.Commands.PrintLevelRoles
{
    public class PrintLevelRolesCommand : IRequest
    {
    }

    public class PrintLevelRolesCommandHandler : IRequestHandler<PrintLevelRolesCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDiscordGuildService _guildService;
        private readonly IDiscordResponseService _responseService;

        public PrintLevelRolesCommandHandler(IDiscordResponseService responseService,
            IApplicationDbContext context,
            IDiscordGuildService guildService)
        {
            _responseService = responseService;
            _context = context;
            _guildService = guildService;
        }

        public async Task<Unit> Handle(PrintLevelRolesCommand request, CancellationToken cancellationToken)
        {
            var levelRoles = await _context.LevelRoles.ToListAsync(cancellationToken);
            if (!levelRoles.Any())
            {
                await _responseService.RespondAsync("No level roles found!", formatAsBlock: true);
                return Unit.Value;
            }

            var roleDictionary = await _guildService.GetRoleNamesById(levelRoles.Select(x => x.roleId));

            var lines = levelRoles
                .Select(x => $"{x.id} :: level {x.lvl} | {GetRoleName(roleDictionary, x.roleId)}")
                .ToList();
            await _responseService.RespondAsync(string.Join(Environment.NewLine, lines), formatAsBlock: true);

            return Unit.Value;
        }

        private static string GetRoleName(Dictionary<ulong, string> roleDictionary, in ulong roleId)
        {
            return roleDictionary.ContainsKey(roleId)
                    ? roleDictionary[roleId]
                    : string.Empty;
        }
    }
}