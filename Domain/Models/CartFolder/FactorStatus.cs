using System.ComponentModel;

namespace Domain.Models
{
    public enum FactorStatus
    {
        [Description("در حال انجام")]
        Progssess=0,
        [Description("لغو شده")]
        Cansel =1,
        [Description("اتمام یافته")]
        Success =2
    }
}
