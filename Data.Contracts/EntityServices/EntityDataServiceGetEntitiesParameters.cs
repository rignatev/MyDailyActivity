using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Data.Shared;

namespace Data.Contracts.EntityServices
{
    public class EntityDataServiceGetEntitiesParameters<TEntity, TEntityIdType>
        where TEntity : class, IEntityOrm<TEntityIdType>, new()
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        public IEnumerable<TEntityIdType> Ids { get; set; }

        public bool? OrderByDescending { get; set; }

        public Expression<Func<TEntity, object>> OrderByProperty { get; set; }

        public int? Count { get; set; }

        public bool IncludeRelated { get; set; }
    }
}
