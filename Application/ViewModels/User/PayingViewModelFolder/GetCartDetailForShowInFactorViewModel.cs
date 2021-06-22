namespace Application.ViewModels.Admin
{
    public class GetCartDetailForShowInFactorViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPriceOfProduct { get; set; }
    }
}
