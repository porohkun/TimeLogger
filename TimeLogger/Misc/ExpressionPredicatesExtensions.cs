namespace System.Linq.Expressions
{
    public static class ExpressionPredicatesExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> firstPredicate, Expression<Func<T, bool>> secondPredicate)
        {
            var parameter = Expression.Parameter(typeof(T), "e");

            var firstBody = Expression.Invoke(firstPredicate, parameter);
            var secondBody = Expression.Invoke(secondPredicate, parameter);

            var body = Expression.OrElse(firstBody, secondBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> firstPredicate, Expression<Func<T, bool>> secondPredicate)
        {
            var parameter = Expression.Parameter(typeof(T), "e");

            var firstBody = Expression.Invoke(firstPredicate, parameter);
            var secondBody = Expression.Invoke(secondPredicate, parameter);

            var body = Expression.AndAlso(firstBody, secondBody);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
