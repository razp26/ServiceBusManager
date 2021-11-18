using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBShared.Models
{
    public class SBModel <T> where T: class
    {
        public string Publisher { get; set; }
        [Required]
        public T Message { get; set; }
    }
}
