using Seedwork.Utilities.Specification.Interfaces;

namespace Seedwork.Utilities.Specification
{
    public class AndSpecification<T> : Specification<T>, ICompositeSpecification<T>
    {
        public ISpecification<T> Left { get; }
        public ISpecification<T> Right { get; }
        public AndSpecification(Specification<T> leftSpecification, Specification<T> rightSpecification)
        {
            Left = leftSpecification;
            Right = rightSpecification;
        }
        public override bool IsSatisfied(T item)
        {
            return Left.IsSatisfied(item) && Right.IsSatisfied(item);
        }
    }
}
