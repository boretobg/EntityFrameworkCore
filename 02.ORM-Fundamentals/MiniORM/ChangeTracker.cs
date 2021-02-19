using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MiniORM
{
	// TODO: Create your ChangeTracker class here.
	internal class ChangeTracker<T>
		where T: class, new()
	{
		private readonly List<T> allEnities;
		private readonly List<T> added;
		private readonly List<T> removed;

        public ChangeTracker(IEnumerable<T> entites)
        {
			this.added = new List<T>();
			this.removed = new List<T>();

			this.allEnities = CloneEntities(entites);
        }

		public IReadOnlyCollection<T> AllEntites => this.allEnities.AsReadOnly();
		public IReadOnlyCollection<T> Added => this.added.AsReadOnly();
		public IReadOnlyCollection<T> Removed => this.removed.AsReadOnly();

		public void Add(T item)
        {
			this.added.Add(item);
        }

		public void Remove(T item)
		{
			this.removed.Add(item);
		}

		private static List<T> CloneEntities(IEnumerable<T> entities)
        {
			var clonedEntities = new List<T>();

			var propertiesToClone = typeof(T).GetProperties().Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType)).ToArray();

            foreach (var entity in entities)
            {
				var clonedEntity = Activator.CreateInstance<T>();

                foreach (var property in propertiesToClone)
                {
					var value = property.GetValue(entity);
					property.SetValue(clonedEntity, value);
                }

				clonedEntities.Add(clonedEntity);
            }

			return clonedEntities;
        }

		public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
			var modifiedEntites = new List<T>();

			var primaryKeys = typeof(T).GetProperties().Where(pi => pi.HasAttribute<KeyAttribute>()).ToArray();

            foreach (var proxyEntity in this.AllEntites)
            {
				var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

				var entity = dbSet.Entities.Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

				var isModified = IsModified(proxyEntity, entity);
                if (isModified)
                {
					modifiedEntites.Add(entity);
                }
            }

			return modifiedEntites;
        }

		private static bool IsModified(T entity, T proxyEntity)
        {
			var monitoredProperties = typeof(T).GetProperties().Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType));

			var modifiedProperties = monitoredProperties.Where(pi => !Equals(pi.GetValue(entity), pi.GetValue(proxyEntity)).ToArray();

			var isModified = modifiedProperties.Any();

			return isModified;
        }

		private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
			return primaryKeys.Select(pk => pk.GetValue(entity));
        }
	}
}