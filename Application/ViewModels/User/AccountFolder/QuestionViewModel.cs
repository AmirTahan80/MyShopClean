using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "موضوع")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "سوال")]
        public string Text { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        public int ProductId { get; set; } = 0;
        public int ReplayId { get; set; } = 0;

        public int Counter { get; set; }
    }
}
