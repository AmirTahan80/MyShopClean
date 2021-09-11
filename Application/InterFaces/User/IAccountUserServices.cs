using Application.ViewModels;
using Application.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IAccountUserServices
    {
        /// <summary>
        /// ثبت نام کاربر با ایمیل و رمز عبور
        /// </summary>
        /// <param name="register"></param>
        /// <returns>bool</returns>
        Task<bool> RegisterUserWithGmail(RegisetUserForLoginViewModel register);
        /// <summary>
        /// تاییدیه ایمیل و دادن مقام خریدار به شخص تایید کننده ایمیل
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="token"></param>
        /// <returns>bool</returns>
        Task<bool> ConfirmEmailAsync(string userEmail, string token);
        /// <summary>
        /// لاگین کردن کاربر با ایمیل و یا نام کاربری
        /// </summary>
        /// <param name="login"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> LoginAsync(LoginViewModel login);
        /// <summary>
        /// نمایش پروفایل
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ProfileViewModel</returns>
        Task<ProfileViewModel> GetDescriptionAsync(string userId);
        /// <summary>
        /// گرفتن اطلاعات کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ProfileViewModel</returns>
        Task<ProfileViewModel> GetPersonalInformationAsync(string userId);
        /// <summary>
        /// ویرایش اطلاعات کاربری
        /// </summary>
        /// <param name="editPersonalInfo"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> EditPersonalInfo(ProfileViewModel editPersonalInfo, string userId);
        /// <summary>
        /// تغییر رمز عبور
        /// </summary>
        /// <param name="changePassWord"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> ChangePassWord(NewPassWordViewModel changePassWord, string userId);
        /// <summary>
        /// فراموشی رمز عبور از طریق ایمیل
        /// </summary>
        /// <param name="forgotPassword"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> ForgotPassWordAsync(ForgotPassWordViewModel forgotPassword);
        /// <summary>
        /// باز یابی رمز عبور
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> ResetPassWordAsync(ResetPasswordViewModel resetPassword);
        /// <summary>
        /// خروج کاربر جاری از سایت
        /// </summary>
        /// <returns></returns>
        Task SignOutAsync();

        /// <summary>
        /// افزودن محصول به سبد خرید و ساخت سبد خرید در صورت نداشتن سبد .
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="templateId"></param>
        /// <param name="count"></param>
        /// <param name="userId"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AddToCartAsync(int productId, int templateId, int count, string userId);
        /// <summary>
        /// گزفتن سبد خرید کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>CartViewModel</returns>
        Task<CartViewModel> GetCartAsync(string userId);
        /// <summary>
        /// حذف کامل محصول
        /// </summary>
        /// <param name="cartDetailId"></param>
        /// <returns>bool</returns>
        Task<bool> RemoveCartDetail(int cartDetailId);
        /// <summary>
        /// کاهش تعداد محصول از سبد خرید
        /// </summary>
        /// <param name="cartDetailId"></param>
        /// <returns>bool</returns>
        Task<bool> LowOffProduct(int cartDetailId);
        /// <summary>
        /// افزایش تعداد محصول
        /// </summary>
        /// <param name="cartDetailId"></param>
        /// <returns>bool</returns>
        Task<bool> IncreaseProduct(int cartDetailId);

        /// <summary>
        /// افزودن محصول به علاقه مندی ها
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AddOrRemoveToFavotieAsync(int productId, string userId);

        /// <summary>
        /// گرفتن علاقه مندی های کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<FavoriteViewModel> GetFavoriteAsync(string userId);
        /// <summary>
        /// پاک کردن محصول از علاقه مندی ها
        /// </summary>
        /// <param name="favoriteDetailId"></param>
        /// <returns></returns>
        Task<bool> RemoveFavoriteDetailAsync(int favoriteDetailId);
        /// <summary>
        /// پرسش و پاسخ
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="question"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AskQuestionAsync(QuestionViewModel question);
        /// <summary>
        /// گرفتن سوالات و نمایش آنها
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ProfileViewModel> GetQuestionsAsync(string userId);
        /// <summary>
        /// اعال تخفیف بر روی سبد خرید
        /// </summary>
        /// <param name="discount"></param>
        /// <returns></returns>
        Task<ResultDto> DiscountCartAsync(CartViewModel discount);
        /// <summary>
        /// گرفتن فاکتور ها برای کاربر
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>FacotrsViewModel</returns>
        Task<ProfileViewModel> GetFactorsAsync(string userId);
        /// <summary>
        /// گزفتن جزئیات فاکتور
        /// </summary>
        /// <param name="id"></param>
        /// <returns>ProfileViewModel</returns>
        Task<ProfileViewModel> GetFactorAsync(int id);
        /// <summary>
        /// ساخت ارتباط با ما
        /// </summary>
        /// <param name="addContactUs"></param>
        /// <returns>ResultDto</returns>
        Task<ResultDto> AddContactUsAsync(ContactUsViewModel addContactUs, string userId);
        /// <summary>
        /// اپلود عکس داخل ادیتور
        /// </summary>
        /// <param name="file"></param>
        /// <returns>JsonResult</returns>
        JsonResult UploadFileEditor(IFormFile file);
        /// <summary>
        /// عضویت در خبرنامه
        /// </summary>
        /// <returns>ResultDto</returns>
        Task<ResultDto> JoinToSendEmailAsync(string email);
    }
}
