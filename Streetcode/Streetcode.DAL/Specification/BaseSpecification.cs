using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.DAL.Specification
{
    public abstract class BaseSpecification<T> : IBaseSpecification<T>
    {
        public BaseSpecification()
        {
        }

        protected BaseSpecification(Expression<Func<T, bool>>? predicate)
        {
            Predicate = predicate;
        }

        public Expression<Func<T, bool>>? Predicate { get; }

        public List<Expression<Func<T, object>>>? Includes { get; } = new List<Expression<Func<T, object>>>();

        public Expression<Func<T, object>>? OrderBy { get; private set; }

        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public Expression<Func<T, object>>? GroupBy { get; private set; }

        public int? Take { get; private set; }

        public int? Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; } = false;

        protected virtual void AddInclude(Expression<Func<T, object>>? includeExpresion)
        {
            Includes.Add(includeExpresion);
        }

        protected virtual void ApplyPaging(int? skip, int? take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>>? expression)
        {
            OrderBy = expression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>>? expression)
        {
            OrderByDescending = expression;
        }

        protected virtual void ApplyGroupBy(Expression<Func<T, object>>? expression)
        {
            GroupBy = expression;
        }
    }
}
