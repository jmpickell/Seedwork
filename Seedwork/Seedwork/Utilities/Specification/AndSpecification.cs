using Seedwork.Utilities.Specification.Interfaces;

namespace Seedwork.Utilities.Specification
{
    public class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _leftSpecification;
        private readonly Specification<T> _rightSpecification;
        public AndSpecification(Specification<T> leftSpecification, Specification<T> rightSpecification)
        {
            _leftSpecification = leftSpecification;
            _rightSpecification = rightSpecification;
        }
        public override bool IsSatisfied(T item)
        {
            return _leftSpecification.IsSatisfied(item) && _rightSpecification.IsSatisfied(item);
        }
    }
}
