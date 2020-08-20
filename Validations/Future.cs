using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Validations
{
    public class Future : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if((DateTime) value < DateTime.Today)
            {
                return new ValidationResult("Wedding must be in the future");
            }
            return ValidationResult.Success;
        }
    }
}