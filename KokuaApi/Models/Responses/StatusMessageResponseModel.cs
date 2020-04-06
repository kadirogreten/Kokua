using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KokuaApi.Models.Responses
{
    public class StatusMessageResponseModel
    {
        public List<string> Messages { get; set; }
        public bool IsError { get; set; }
    }
}
