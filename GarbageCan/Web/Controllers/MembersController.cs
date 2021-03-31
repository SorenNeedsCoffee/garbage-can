﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GarbageCan.Data;
using GarbageCan.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarbageCan.Web.Controllers
{
    [ApiController]
    [Route("members")]
    public class MembersController : Controller
    {
        [HttpGet]
        public async Task<List<Member>> Get()
        {
            var result = new List<Member>();
            await using var context = new Context();
            var members = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].GetAllMembersAsync();

            foreach (var member in members)
            {
                var xpMember = await context.xpUsers.FindAsync(member.Id);
                result.Add(new Member
                {
                    Id = member.Id,
                    Name = member.DisplayName,
                    Xp = xpMember.xp,
                    Level = xpMember.lvl
                });
            }

            return result;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> Get(ulong id)
        {
            await using var context = new Context();
            var member = await GarbageCan.Client.Guilds[GarbageCan.OperatingGuildId].GetMemberAsync(id);
            if (member == null) return NotFound();
            var memberEntity = await context.xpUsers.FindAsync(id);

            return Ok(new Member
            {
                Id = id,
                Name = member.DisplayName,
                Xp = memberEntity.xp,
                Level = memberEntity.lvl
            });
        }
    }
}