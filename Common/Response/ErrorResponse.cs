using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Response
{
    public class ErrorResponse
    {
        public bool IsError { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
}
