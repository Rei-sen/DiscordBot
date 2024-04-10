using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Model.DB
{
    public class PFSubscriptionsContext : DbContext
    {
        public DbSet<PFSubscription> Subscriptions { get; set; }

        public PFSubscriptionsContext(DbContextOptions<PFSubscriptionsContext> options) : base(options)
        {
        }
    }
}
