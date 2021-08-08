using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class CartDetailViewModel
    {
        public int Id { get; set; }

        public int TotalPrice { get; set; }

        public int ProductCount { get; set; }
        public int ProductPrice { get; set; }
        public string ProductName { get; set; }
        public string ImgSrc { get; set; }

        public List<AttributeNamesViewModel> AttributeNames { get; set; }
        public List<string> AttributeValues { get; set; }

        public IEnumerable<PropertyViewModel> Properties { get; set; }


    }
}
