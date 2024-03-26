namespace IspitToDo.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public bool Status { get; set; }
        public int TodolistId { get; set; }

        public virtual Todolist? Todolist { get; set; }
    }
}
