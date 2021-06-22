using Application.ViewModels.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface IAccountServices
    {
        Task<bool> CreateUserByEmailAndPass(CreateAccountViewModel user, string userPassword);
        Task<IList<UsersListViewModel>> GetAllUsersListAsync();
        Task<UserDetailViewModel> FinUserById(string userId);
        Task<bool> ConfirmEmailAsync(string userName, string token);
    }
}
