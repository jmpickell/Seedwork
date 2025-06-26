using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Seedwork.Utilities.Specification
{

    public class NotSpecification<T> : Specification<T>, IUnitarySpecification<T>
    {
        public ISpecification<T> Base { get; }
        public NotSpecification(Specification<T> specification)
        {
            Base = specification;
        }
        public override bool IsSatisfied(T item)
        {
            return !Base.IsSatisfied(item);
        }
    }
}
