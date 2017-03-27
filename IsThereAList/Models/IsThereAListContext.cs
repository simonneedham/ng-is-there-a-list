using Microsoft.EntityFrameworkCore;

namespace IsThereAList.Models
{
    public class IsThereAListContext : DbContext
    {
        public IsThereAListContext(DbContextOptions<IsThereAListContext> options) : base(options)
        {
        }

        public DbSet<List> Lists { get; set; }
        public DbSet<ListItem> ListItems { get; set; }
        public DbSet<ListType> ListTypes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

