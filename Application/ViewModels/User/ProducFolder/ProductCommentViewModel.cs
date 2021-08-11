using System;
using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class ProductCommentViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public bool CustomerSuggestToBuy { get; set; }
        public DateTime InsertTime { get; set; }
        public IEnumerable<string> GoodNess { get; set; }
        public IEnumerable<string> Bads { get; set; }

    }
}
