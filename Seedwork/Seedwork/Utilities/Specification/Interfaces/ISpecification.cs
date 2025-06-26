namespace Seedwork.Utilities.Specification.Interfaces
{
    public interface ICompositeSpecification<T> : ISpecification<T>
    {
        ISpecification<T> Left { get; }
        ISpecification<T> Right { get; }
    }

    public interface IUnitarySpecification<T> : ISpecification<T>
    {
        ISpecification<T> Base { get; }
    }

    public interface ISpecification<T>
    {
        bool IsSatisfied(T item);
    }

    public abstract class Specification<T> : ISpecification<T>
    {
        public abstract bool IsSatisfied(T item);

        public static Specification<T> operator &(Specification<T> left, Specification<T> right) =>
            new AndSpecification<T>(left, right);

        public static Specification<T> operator |(Specification<T> left, Specification<T> right) =>
            new OrSpecification<T>(left, right);

        public static Specification<T> operator !(Specification<T> specification) =>
            new NotSpecification<T>(specification);
    }
}
