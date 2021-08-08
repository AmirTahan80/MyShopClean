namespace Domain.Models
{
    public class CartDetail
    {
        public int CartDetailId { get; set; }
        public int ProductCount { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int TotalPrice { get; set; }
        public int ProductPrice { get; set; }

        public string AttributeValues { get; set; }

        //NavigationBar
        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public AttributeTemplate Templates { get; set; }

    }
}