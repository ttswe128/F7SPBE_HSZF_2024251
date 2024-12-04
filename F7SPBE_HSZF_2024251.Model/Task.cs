using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace F7SPBE_HSZF_2024251.Model
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public virtual Programmer Responsible { get; set; }
        [StringLength(255)]
        public string Size { get; set; }
        public EStatus Status { get; set; }


        public Task(string name, string description, Programmer responsible, string size, EStatus status)
        {
            Name = name;
            Description = description;
            Responsible = responsible;
            Size = size;
            Status = status;
        }

        public Task()
        {

            this.Status = EStatus.STARTED;
        }
    }
}
