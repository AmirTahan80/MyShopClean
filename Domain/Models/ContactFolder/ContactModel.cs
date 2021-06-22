using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPhoneNumber { get; set; }
        public string Address { get; set; }
        public string InstagramUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string TelegramUrl { get; set; }
        public string SoroushUrl { get; set; }
        public string LinkDinUrl { get; set; }
        public string TwiterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string WhatsAppUrl { get; set; }
        public string FaceBookUrl { get; set; }

    }
}
