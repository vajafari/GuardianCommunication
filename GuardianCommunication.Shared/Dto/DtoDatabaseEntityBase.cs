using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuardianCommunication.Shared.Dto
{
    public abstract class DtoDatabaseEntityBase
    {
        public Guid Id { get; set; }
        public DateTime InsertedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
