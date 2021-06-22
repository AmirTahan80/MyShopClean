namespace Domain.Models
{
    public class CartDetail
    {
        public long CartDetailId { get; set; }
        public int ProductCount { get; set; }
        public long CartId { get; set; }
        public string ImgSrc { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        //NavigationBar
        public Cart Cart{get; set;}
        public Product Product{get; set; }
        public Factor Factor { get; set; }

    }
}