namespace Domain.Models
{
    public class FactorDetail
    {
        public int FactorDetailId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPriceOfProduct { get; set; }

        //Navigation
        public int FactorId { get; set; }
        public Factor Factor { get; set; }
    }
}
