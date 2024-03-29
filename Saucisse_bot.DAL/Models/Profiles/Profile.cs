﻿using Saucisse_bot.DAL.Models.Items;
using System;
using System.Collections.Generic;

namespace Saucisse_bot.DAL.Models.Profiles
{
    public class Profile : Entity
    {
        public ulong DiscordId { get; set; }
        public ulong GuildId { get; set; }
        public int Gold { get; set; }
        public int Xp { get; set; }
        public int Level => (int)Math.Sqrt(Xp/100);

        public List<ProfileItem> Inventory { get; set; } = new List<ProfileItem>();
    }
}
