using System;

using Data.Contracts.EntityOrm;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EF.Core
{
    public abstract class EntityOrmConfigurationBase<TOrm, TEntityIdType> : IEntityTypeConfiguration<TOrm>
        where TOrm : class, IEntityOrm<TEntityIdType>
        where TEntityIdType : IComparable<TEntityIdType>, IEquatable<TEntityIdType>
    {
        protected abstract string Table { get; }

        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<TOrm> builder)
        {
            builder.ToTable(this.Table);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd().HasColumnType("INTEGER");
            builder.Property(x => x.CreatedDateTimeUtc).IsRequired().HasColumnType("DATETIME");
            builder.Property(x => x.ModifiedDateTimeUtc).HasColumnType("DATETIME");
            builder.Property(x => x.RowVersion).IsRowVersion();

            ConfigureCore(builder);
        }

        protected abstract void ConfigureCore(EntityTypeBuilder<TOrm> builder);
    }
}
