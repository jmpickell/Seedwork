using System;
using System.Collections.Generic;
using System.Text;

namespace Seedwork.IOC
{
    public class IocParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public static implicit operator IocParameter((string n, object v) value) => new IocParameter { Name = value.n, Value = value.v };
    }
}
