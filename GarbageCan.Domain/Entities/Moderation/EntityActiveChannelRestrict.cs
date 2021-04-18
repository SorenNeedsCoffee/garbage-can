using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarbageCan.Domain.Entities.Moderation
{
    public class EntityActiveChannelRestrict
    {
        [Key] public int id { get; set; }
        public ulong uId { get; init; }
        public ulong channelId { get; init; }
        [Column(TypeName = "datetime")] public DateTime expirationDate { get; init; }
    }
}