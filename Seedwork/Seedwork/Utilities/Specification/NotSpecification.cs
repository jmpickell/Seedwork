using Seedwork.Utilities.Specification.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Seedwork.Utilities.Specification
{

    public class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;
        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }
        public override bool IsSatisfied(T item)
        {
            return !_specification.IsSatisfied(item);
        }
    }
}
