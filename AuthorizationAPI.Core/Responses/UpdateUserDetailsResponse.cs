using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationAPI.Core.Responses
{
    public class UpdateUserDetailsResponse
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public List<string>? Permissions { get; set; }   
    }
}
