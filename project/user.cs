using System;
using Microsoft.EntityFrameworkCore;

namespace UserApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
