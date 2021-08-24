using Application.Utilities.Attributes;
using Domain.Models;
using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class FactorViewModel
    {
        public int Id { get; set; }
        public FactorStatus FactorStatus { get; set; }
        public int TotalPrice { get; set; }
        public int RefId { get; set; }


        //Navigation

        public IEnumerable<DisCountViewModel> Discounts { get; set; }
        public IEnumerable<FactorDetailViewModel> FactorDetails { get; set; }
    }
}
