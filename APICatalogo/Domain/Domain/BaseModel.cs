using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Domain
{
    public class BaseModel
    {
        public DateTime AddDate { get; set; } = DateTime.Now;
        public DateTime? AlterDate { get; set; } = null;
    }
}
