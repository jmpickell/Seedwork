using Seedwork.Utilities.Specification.Interfaces;

namespace Seedwork.Utilities.Specification
{
    public class OrSpecification<T> : Specification<T>, ICompositeSpecification<T>
    {
        public ISpecification<T> Left { get; }
        public ISpecification<T> Right { get; }
        public OrSpecification(ISpecification<T> leftSpecification, ISpecification<T> rightSpecification)
        {
            Left = leftSpecification;
            Right = rightSpecification;
        }
        public override bool IsSatisfied(T item)
        {
            return Left.IsSatisfied(item) || Right.IsSatisfied(item);
        }
    }
}
