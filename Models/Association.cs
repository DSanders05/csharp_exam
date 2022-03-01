using System;
using System.ComponentModel.DataAnnotations;

namespace csharp_exam.Models
{
    public class Association
    {
        [Key]
        public int AssociationId {get;set;}
        public int UserId {get;set;}
        public int ActId {get;set;}

        public User Participant {get;set;}
        public Act Act {get;set;}
    }
}