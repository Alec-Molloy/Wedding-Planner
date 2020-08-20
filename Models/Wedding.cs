using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeddingPlanner.Validations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId{get;set;}

        [Required(ErrorMessage="Please Enter Name")]
        public string Wedder1{get;set;}

        [Required(ErrorMessage="Please Enter Name")]
        public string Wedder2{get;set;}

        [Required(ErrorMessage="Please Enter Your Wedding Day")]
        [DataType(DataType.Date)]
        [Future]
        public DateTime WeddingDay{get;set;}

        [Required(ErrorMessage="Where is your wedding?")]
        public string WeddingAddress{get;set;}

        public int UserId{get;set;}
        public User myWedding{get;set;}

        public List<Association> guests{get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

    }
}