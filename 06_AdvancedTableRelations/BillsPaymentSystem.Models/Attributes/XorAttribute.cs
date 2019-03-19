﻿namespace BillsPaymentSystem.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private string xorTargetProperty;

        public XorAttribute(string xorTarget)
        {
            this.xorTargetProperty = xorTarget;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetPropertyValue = validationContext.ObjectType
                .GetProperty(xorTargetProperty)
                .GetValue(validationContext.ObjectInstance);

            if ((targetPropertyValue) == null ^ (value == null))
            {
                return ValidationResult.Success;
            }

            string errorMsg = "The two properties must have opposite values!";

            return new ValidationResult(errorMsg);
        }
    }
}
