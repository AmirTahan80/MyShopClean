namespace Application.ViewModels.User
{
    public class UserCommentsViewModel
    {
        public int CommentId { get; set; }
        public int ProductId { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public string CommentText { get; set; }
        public bool IsShow { get; set; }
    }
}
