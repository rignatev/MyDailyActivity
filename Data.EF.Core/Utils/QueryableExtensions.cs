using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data.EF.Core.Utils
{
    static public class QueryableExtensions
    {
        static public IQueryable<TEntity> ConditionalInclude<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            bool condition,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            if (condition)
            {
                return source.Include(navigationPropertyPath);
            }

            return source;
        }

        static public IQueryable<TEntity> IncludeAll<TEntity>(this DbSet<TEntity> dbSet, int maxDepth = int.MaxValue)
            where TEntity : class
        {
            IQueryable<TEntity> query = dbSet.AsQueryable();

            DbContext dbContext = dbSet.GetService<ICurrentDbContext>().Context;
            IEnumerable<string> includePaths = GetIncludePaths<TEntity>(dbContext, maxDepth);

            return includePaths.Aggregate(query, (current, includePath) => current.Include(includePath));
        }

        static public IQueryable<TEntity> IncludeAll<TEntity>(
            this IQueryable<TEntity> source,
            DbContext dbContext,
            int maxDepth = int.MaxValue)
            where TEntity : class
        {
            IEnumerable<string> includePaths = GetIncludePaths<TEntity>(dbContext, maxDepth);

            return includePaths.Aggregate(source, (current, includePath) => current.Include(includePath));
        }

        static private IEnumerable<string> GetIncludePaths<TEntity>(this DbContext context, int maxDepth = int.MaxValue)
        {
            if (maxDepth < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDepth));
            }

            IEntityType entityType = context.Model.FindEntityType(typeof(TEntity));
            var includedNavigations = new HashSet<INavigation>();
            var stack = new Stack<IEnumerator<INavigation>>();

            while (true)
            {
                var entityNavigations = new List<INavigation>();
                if (stack.Count <= maxDepth)
                {
                    foreach (INavigation navigation in entityType.GetNavigations())
                    {
                        if (includedNavigations.Add(navigation))
                        {
                            entityNavigations.Add(navigation);
                        }
                    }
                }

                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                    {
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                    }
                }
                else
                {
                    foreach (INavigation navigation in entityNavigations)
                    {
                        INavigation inverseNavigation = navigation.Inverse;
                        if (inverseNavigation != null)
                        {
                            includedNavigations.Add(inverseNavigation);
                        }
                    }

                    stack.Push(entityNavigations.GetEnumerator());
                }

                while (stack.Count > 0 && !stack.Peek().MoveNext())
                {
                    stack.Pop();
                }

                if (stack.Count == 0)
                {
                    break;
                }

                entityType = stack.Peek().Current.GetTargetType();
            }
        }
    }
}
