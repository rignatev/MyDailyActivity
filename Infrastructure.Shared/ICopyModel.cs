using System;

namespace Infrastructure.Shared
{
    public interface ICopyModel<out TEntity>
        where TEntity : class, new()
    {
        TEntity CopyModelForCreate(DateTime? createdDateTimeUtc = null);

        TEntity CopyModelForEdit(DateTime? modifiedDateTimeUtc = null);
    }
}
