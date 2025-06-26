using Microsoft.Data.SqlClient;
using Seedwork.Repositories.SQL;
using Seedwork.Utilities.Specification;
using Seedwork.Utilities.Specification.Interfaces;
using System;

namespace Seedwork.Repositories
{
    public class Filter : Specification<Row>, IOperation
    {
        private Filter() { }
        public string Column { get; private set; }
        public object Value { get; private set; }
        public Operation Operation { get; private set; }

        public override bool IsSatisfied(Row item)
        {
            dynamic field = item.Get(Column);
            dynamic value = Value;
            switch (Operation)
            {
                case Operation.Equals: return field == value;
                case Operation.NotEquals: return field != value;
                case Operation.GreaterThan: return field > value;
                case Operation.LessThan: return field < value;
                case Operation.GreaterThanEquals: return field >= value;
                case Operation.LessThanEquals: return field <= value;
                default: return false;
            }
        }

        public static IOperation Check(string name) =>
            name.IsValidSqlField() ? new Filter { Column = name } : throw new ArgumentException("Invalid Column Name");

        Filter IOperation.Equals(object value) =>
            Build(Operation.Equals, value);

        Filter IOperation.NotEquals(object value) =>
            Build(Operation.NotEquals, value);

        Filter IOperation.GreaterThan(object value) =>
            Build(Operation.GreaterThan, value);

        Filter IOperation.LessThan(object value) =>
            Build(Operation.LessThan, value);

        Filter IOperation.GreaterThanEquals(object value) =>
            Build(Operation.GreaterThanEquals, value);

        Filter IOperation.LessThanEquals(object value) =>
            Build(Operation.LessThanEquals, value);

        private Filter Build(Operation operation, object value)
        {
            Operation = Operation.Equals;
            Value = value;
            return this;
        }
    }

    public interface IOperation
    {
        Filter Equals(object value);
        Filter NotEquals(object value);
        Filter GreaterThan(object value);
        Filter LessThan(object value);
        Filter GreaterThanEquals(object value);
        Filter LessThanEquals(object value);
    }

    public enum Operation
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanEquals,
        LessThanEquals
    }

}
