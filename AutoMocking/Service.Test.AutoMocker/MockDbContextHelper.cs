using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Service.Test.UseAutoMocker
{
    public static class MockDbContextHelper
    {
        public static DbSet<TEntity> ConfigureDbSet<TEntity>(this List<TEntity> set) where TEntity : class
        {
            var queryable = set.AsQueryable();
            var mockDbSet = new Mock<DbSet<TEntity>>();
            mockDbSet.As<IQueryable<TEntity>>();

            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(() => queryable.Provider);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(() => queryable.Expression);
            mockDbSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(() => queryable.ElementType);

            mockDbSet.Setup(m => m.Add(It.IsAny<TEntity>()))
                .Callback((TEntity entity) => set.Add(entity))
                .Returns((TEntity entity) => entity);

            mockDbSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback((IEnumerable<TEntity> entities) => set.AddRange(entities))
                .Returns((IEnumerable<TEntity> entities) => entities);

            mockDbSet.Setup(m => m.Remove(It.IsAny<TEntity>()))
                .Callback((TEntity entity) => set.Remove(entity))
                .Returns((TEntity entity) => entity);

            mockDbSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback((IEnumerable<TEntity> entities) =>
                {
                    var matches = set.Join(entities,
                        JsonConvert.SerializeObject,
                        JsonConvert.SerializeObject,
                        (item, entity) => item)
                    .ToArray();
                    foreach(var match in matches)
					{
                        set.Remove(match);
					}
                })
                .Returns((IEnumerable<TEntity> entities) => entities);

            return mockDbSet.Object;
        }
    }
}
