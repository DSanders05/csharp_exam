using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace csharp_exam.Models
{
    public class Act
    {
        [Key]
        public int ActId {get;set;}
        [Required]
        public string Title {get;set;}
        [Required]
        public DateTime Date {get;set;}
        [Required]
        public int Duration {get;set;}
        public string DurAmount {get;set;}
        public DateTime Time {get;set;}
        [Required]
        public string Description {get;set;}
        public int UserId {get;set;}
        public User Creator {get;set;}

        public List<Association> Participants {get;set;}
        public DateTime createdAt {get;set;} = DateTime.Now;
        public DateTime updatedAt {get;set;} = DateTime.Now;
    }
}