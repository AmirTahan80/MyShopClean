namespace Application.ViewModels.Admin
{
    public class UsersListViewModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
        public string UserId { get; set; }
    }
}
