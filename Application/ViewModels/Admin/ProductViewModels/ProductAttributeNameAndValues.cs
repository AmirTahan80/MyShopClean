using System.Collections.Generic;

namespace Application.ViewModels.Admin
{
    public class ProductAttributeNameAndValuesViewModel
    {
        public string Name { get; set; }
        public List<ProductAttributeValuesViewModel> Values { get; set; }
    }
}
