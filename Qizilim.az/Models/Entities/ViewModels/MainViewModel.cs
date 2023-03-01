using Qizilim.az.Models.Entities.Membreship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qizilim.az.Models.Entities.ViewModels
{
    public class MainViewModel
    {
        public List<Products> OxsarProducts { get; set; }
        public List<Products> Products { get; set; }
        public List<QizilimUser> QizilimUser { get; set; }
        public List<Advertisement> Advertisement { get; set; }
    }
}
