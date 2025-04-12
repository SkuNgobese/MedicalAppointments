using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointments.Shared.Models
{
    public class SysAdmin : ApplicationUser
    {
        public Hospital? Hospital { get; set; }
    }
}
