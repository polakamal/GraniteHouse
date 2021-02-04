using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModel
{
    public class AppointmentsDetailsViewModel
    {
        public Appointments appointment { get; set; }
        public List<ApplicationUser> SalesPerson { get; set; }
        public List<Products> products { get; set; }
    }
}
