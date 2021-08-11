namespace Application.ViewModels
{
    public class ResultDto
    {
        public bool Status { get; set; }
        public bool ShowNotFound { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }
    }
}
