using Seedwork.Utilities.Specification.Interfaces;

namespace Seedwork.Utilities.Specification
{
    public class OrSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _leftSpecification;
        private readonly ISpecification<T> _rightSpecification;
        public OrSpecification(ISpecification<T> leftSpecification, ISpecification<T> rightSpecification)
        {
            _leftSpecification = leftSpecification;
            _rightSpecification = rightSpecification;
        }
        public override bool IsSatisfied(T item)
        {
            return _leftSpecification.IsSatisfied(item) || _rightSpecification.IsSatisfied(item);
        }
    }
}
