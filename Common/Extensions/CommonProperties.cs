using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
    public class CommonProperties
    {
        public virtual bool? IsActive { get; set; } = true;
        public virtual bool? IsDeleted { get; set; } = false;
        public virtual DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual DateTime UpdatedAt { get; set; } = DateTime.Now;
        public virtual string OperationBy { get; set; } = AppConstants.AppSystem;
    }
}
