using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MiniORM
{
    // TODO: Create your DbSet class here.
    public class DbSet<TEntity> : ICollection<TEntity>
        where TEntity : class, new()
    {

        internal DbSet(IEnumerable<TEntity> entities)
        {
            this.Entities = entities.ToList();
            this.ChangeTracker = new ChangeTracker<TEntity>(entities);
        }

        internal ChangeTracker<TEntity> ChangeTracker { get; set; }
        internal IList<TEntity> Entities { get; set; }


        public int Count => this.Entities.Count;

        public bool IsReadOnly => this.Entities.IsReadOnly;

        public void Add(TEntity item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null!");
            }

            this.Entities.Add(item);
            this.ChangeTracker.Add(item);
        }

        public void Clear()
        {
            while (this.Entities.Any())
            {
                var entity = this.Entities.First();
                this.Remove(entity);
            }
        }

        public bool Contains(TEntity item)
            => this.Entities.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex)
            => this.Entities.CopyTo(array, arrayIndex);

        public IEnumerator<TEntity> GetEnumerator()
            => this.Entities.GetEnumerator();

        public bool Remove(TEntity item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null!");
            }

            var isRemoved = this.Entities.Remove(item);

            if (isRemoved)
            {
                this.ChangeTracker.Remove(item);
            }

            return isRemoved;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        public void RemoveRange(IEnumerable<TEntity> entites)
        {
            foreach (var entity in entites.ToArray())
            {
                this.Remove(entity);
            }
        }
    }
}