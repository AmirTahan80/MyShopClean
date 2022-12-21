namespace Application.ViewModels
{
    public class ResultDto
    {
        public bool Status { get; set; }
        public bool ShowNotFound { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }
        public string ReturnRedirect { get; set; }
        public object Data { get; set; }
    }
    public class ResultDto<T>
    {
        public bool Status { get; set; }
        public bool ShowNotFound { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }
        public string ReturnRedirect { get; set; }
        public T Data { get; set; }
    }
}
