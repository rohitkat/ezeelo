using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.CustomAnnotation
{
    public class FutureDateValidation : ValidationAttribute
    {


        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (value != null)
            {
                if ((DateTime)value > DateTime.UtcNow.AddHours(5.5))
                {
                    return new ValidationResult("Date can't be in future.");

                }
                else if ((DateTime)value < DateTime.UtcNow.AddHours(5.5).AddYears(-100))
                {
                    return new ValidationResult("Date of birth can't be too past.");
                }
                
            }
            return ValidationResult.Success;
            //else
            //{
            //    return new ValidationResult("Date Can't be null");
            
            //}
                  
        }
    }
}
