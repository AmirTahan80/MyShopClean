using System.Collections;
using System.Collections.Generic;

namespace Application.ViewModels.Admin
{
    public class AdminIndexViewModel
    {
        public int TodayUserCount { get; set; }
        public int UserCount { get; set; }
        public int TodayProductCount { get; set; }
        public int ProductCount { get; set; }
        public int TodayCommentsCount { get; set; }
        public int TodayQuestionsCount { get; set; }
        public int TodayFactorsCount { get; set; }

        //Navigation
        public ICollection<FactorViewModel> Factors { get; set; }
    }
}
 