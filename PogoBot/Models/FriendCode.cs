using EasyDatabase.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace PogoBot.Models
{
    public class FriendCode
        : IEntity
    {
        public Guid Id { get; set; }

        public ulong UserId { get; set; }

        public IEnumerable<string> Codes { get; set; }
    }
}
