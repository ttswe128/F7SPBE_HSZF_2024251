using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Model
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string Name { get; set; }
        [StringLength(255)]
        [Required]
        public string Description { get; set; }
        [Required]
        public EStatus Status { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public virtual ICollection<Programmer> Participants { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        [Required]
        public List<DateTime> Deadlines { get; set; }



        public Project()
        {
            Participants = new HashSet<Programmer>();
            Tasks = new HashSet<Task>();
            Deadlines = new List<DateTime>();
        }

        public Project(string name, string description, EStatus status, DateTime startDate, DateTime endDate, ICollection<Programmer> participants, ICollection<Task> tasks, HashSet<DateTime> deadlines)
        {
            Name = name;
            Description = description;
            Status = status;
            StartDate = startDate;
            EndDate = endDate;
            Participants = new HashSet<Programmer>(participants);
            Tasks = new HashSet<Task>(tasks);
            Deadlines = new List<DateTime>(deadlines);
        }
    }
}
