using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Data.Shared;

namespace Data.Contracts.EntityDataServices
{
    public class EntityDataServiceGetEntitiesParameters<TEntityOrm, TEntityOrmIdType>
        where TEntityOrm : class, IEntityOrm<TEntityOrmIdType>, new()
        where TEntityOrmIdType : IComparable<TEntityOrmIdType>, IEquatable<TEntityOrmIdType>
    {
        public IEnumerable<TEntityOrmIdType> Ids { get; set; }

        public bool? OrderByDescending { get; set; }

        public Expression<Func<TEntityOrm, object>> OrderByProperty { get; set; }

        public int? Count { get; set; }

        public bool IncludeRelated { get; set; }
    }
}
