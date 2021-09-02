using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Text { get; set; }
        public string Topic { get; set; }
        [Required(ErrorMessage ="فیلد پاسخ اجباریست !")]
        public string Awnser { get; set; }
    }
}
