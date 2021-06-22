using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class FooterModel
    {
        public int FooterId { get; set; }
        public string Title { get; set; }
        ///Navigation
        public ICollection<FooterSubjectModel> Subjects { get; set; }
    }
}
