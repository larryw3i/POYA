using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace POYA.Unities.Attributes
{
    /// <summary>
    /// Determine the value be passed is 
    /// </summary>
    public class DOBirthAttribute : ValidationAttribute
    {
        /// <summary>
        /// The DOStartString be passed should be able to convert to DateTime type
        /// </summary>
        public string DOStartString { get; set; }
        /// <summary>
        /// The DOEndString be passed should be able to convert to DateTime type
        /// </summary>
        public string DOEndString { get; set; }
        public bool IsValueNullable { get; set; } = false;
        public override bool IsValid(object value)
        {
            if (value == null && IsValueNullable)
                return true;
            var _value = Convert.ToDateTime(value.ToString());
            var DOStart = string.IsNullOrWhiteSpace(DOStartString) ? DateTime.Now.AddYears(-120) : Convert.ToDateTime(DOStartString);
            var DOEnd = string.IsNullOrWhiteSpace(DOEndString) ? DateTime.Now : Convert.ToDateTime(DOEndString);
            if (_value >= DOStart && _value <= DOEnd)
                return true;
            //  return base.IsValid(value);
            return false;
        }
    }

}
