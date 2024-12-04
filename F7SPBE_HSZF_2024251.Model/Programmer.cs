using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Model
{
    public class Programmer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        public string Role { get; set; }
        private short dateOfJoining;
        public short DateOfJoining { 
            get 
            { 
                return dateOfJoining; 
            }
            set 
            {
                if (value < 1970 || value > 2099)
                {
                    throw new Exception("The year of joining should be between 1970 and 2099.");
                }
                    dateOfJoining = value;
            } 
        }

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        public Programmer(string name, string role, short dateOfJoining)
        {
            this.Name = name;
            this.Role = role;
            this.DateOfJoining = dateOfJoining;
        }

        public Programmer(string name, string role, short dateOfJoining, List<Task> tasks)
        {
            this.Name = name;
            this.Role = role;
            this.DateOfJoining = dateOfJoining;
            this.Tasks = tasks;
        }

        public Programmer() { }
    }
}
