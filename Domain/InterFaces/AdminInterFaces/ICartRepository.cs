using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.InterFaces.AdminInterFaces
{
    public interface ICartRepository:ISaveInterFaces
    {
        Task<IEnumerable<Cart>> GetCartsAsync();
        Task<Cart> GetCartAsync(string userId);

        Task<CartDetail> GetCartDetailAsync(int cartDetailId);

        Task AddCart(Cart t);
        Task AddCartDetail(CartDetail t);

        void RemoveCartDetail(CartDetail t);

        Task AddFavorite(UserFavorite t);
        Task AddFavoriteDetail(UserFavoritesDetail t);

        Task<UserFavorite> GetFavoriteAsync(string userId);
        Task<UserFavoritesDetail> GetFavoriteDetailAsync(int favoriteDetailId);

        void RemoveFavoriteDetail(UserFavoritesDetail t);

    }
}
