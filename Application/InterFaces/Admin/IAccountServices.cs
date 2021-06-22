using Application.ViewModels.Admin;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
