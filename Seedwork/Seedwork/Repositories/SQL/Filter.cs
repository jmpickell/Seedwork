using Seedwork.Utilities.Specification.Interfaces;
using System;

namespace Seedwork.Repositories.SQL
{
    public abstract class Filter : Specification<object>
    {
        public Filter(string name, object value, Operation operation)
        {
            Name = name;
            Value = value;
            Operation = operation;
        }
        public string Name { get; }
        public object Value { get; }
        public Operation Operation { get; }
    }
}
