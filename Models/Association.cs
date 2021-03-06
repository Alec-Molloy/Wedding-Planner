using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Association
    {
        [Key]
        public int AssociationId{get;set;}
        public int WeddingId{get;set;}
        public int UserId{get;set;}
        public User guest{get;set;}
        public Wedding wedding{get;set;}
    }
}