using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class FooterSubjectModel
    {
        public int FooterSubjectId { get; set; }
        public int FooterId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string MediaName { get; set; }

        ///Navigation
        public FooterModel Foooter { get; set; }
    }
}
