using Microsoft.AspNetCore.Identity;

namespace IspitToDo.Models
{
    public class Todolist
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? UserId { get; set; }

        public virtual IdentityUser? User { get; set; }

        public virtual ICollection<Tasks>? Tasks { get; set; }
    }
}
