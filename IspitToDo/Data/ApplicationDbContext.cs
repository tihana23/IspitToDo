using IspitToDo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IspitToDo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
         
    }
        public DbSet<Todolist> Todolists { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
    }
}
