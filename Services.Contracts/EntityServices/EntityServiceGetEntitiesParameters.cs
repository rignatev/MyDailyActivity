using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Infrastructure.Shared.Entities;

namespace Services.Contracts.EntityServices
{
    public class EntityServiceGetEntitiesParameters<TEntity, TEntityIdType>
        where TEntity : class, IEntity<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        public IEnumerable<TEntityIdType> Ids { get; set; }

        public bool? OrderByDescending { get; set; }

        public Expression<Func<TEntity, object>> OrderByProperty { get; set; }

        public int? Count { get; set; }

        public bool IncludeRelated { get; set; }
    }
}
