using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class CartViewModel
    {
        public int Id { get; set; }
        public int TotalPrice { get; set; }
        public DateTime CreateTime { get; set; }
        public int CountOfProduct { get; set; }

        public string CodeName { get; set; }

        public ICollection<CartDetailViewModel> CartDetails { get; set; }
    }
}
