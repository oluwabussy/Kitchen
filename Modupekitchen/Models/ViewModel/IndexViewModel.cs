using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modupekitchen.Models.ViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<MenuItem> MenuItem { get; set; }
        public IEnumerable<Coupon_t> Coupon { get; set; }
        public IEnumerable<Category> Category { get; set; }
    }
}
