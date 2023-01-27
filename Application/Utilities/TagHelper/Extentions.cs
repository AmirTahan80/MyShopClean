using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities.TagHelper
{
    public static class Extentions
    {
        public static string GetWebFullDomain(this HttpRequest httpRequest)
        {
            var request = httpRequest.Scheme + "://" + httpRequest.Host + httpRequest.Path + httpRequest.QueryString;
            return request;
        }
    }
}
