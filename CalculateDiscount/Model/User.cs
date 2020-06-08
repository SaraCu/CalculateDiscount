using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateDiscount.Model
{
    public class User
    {
        public string UserName { get; set; }

        public UserCategory Category { get; set; }
    }
}
