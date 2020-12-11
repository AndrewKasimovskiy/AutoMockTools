using FakeItEasy;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Service.Test
{
    public static class FakeDbContextHelper
    {
        public static DbSet<TEntity> ConfigureDbSet<TEntity>(this List<TEntity> set) where TEntity : class
        {
            var queryable = set.AsQueryable();
            var fakeDbSet = A.Fake<DbSet<TEntity>>(o => o.Implements(typeof(IQueryable<TEntity>)));

            A.CallTo(() => ((IQueryable<TEntity>)fakeDbSet).GetEnumerator()).ReturnsLazily(() => queryable.GetEnumerator());
            A.CallTo(() => ((IQueryable<TEntity>)fakeDbSet).Provider).ReturnsLazily(() => queryable.Provider);
            A.CallTo(() => ((IQueryable<TEntity>)fakeDbSet).Expression).ReturnsLazily(() => queryable.Expression);
            A.CallTo(() => ((IQueryable<TEntity>)fakeDbSet).ElementType).ReturnsLazily(() => queryable.ElementType);
            
            A.CallTo(() => fakeDbSet.Add(A<TEntity>.Ignored))
                .Invokes((TEntity entity) => set.Add(entity))
                .ReturnsLazily((TEntity entity) => entity);

            A.CallTo(() => fakeDbSet.AddRange(A<IEnumerable<TEntity>>.Ignored))
                .Invokes((IEnumerable<TEntity> entities) => set.AddRange(entities))
                .ReturnsLazily((IEnumerable<TEntity> entities) => entities);

            A.CallTo(() => fakeDbSet.Remove(A<TEntity>.Ignored))
                .Invokes((TEntity entity) => set.Remove(entity))
                .ReturnsLazily((TEntity entity) => entity);

            A.CallTo(() => fakeDbSet.RemoveRange(A<IEnumerable<TEntity>>.Ignored))
                .Invokes((IEnumerable<TEntity> entities) =>
                {
                    var matches = set.Join(entities,
                        JsonConvert.SerializeObject,
                        JsonConvert.SerializeObject,
                        (item, entity) => item)
                    .ToArray();
                    foreach (var match in matches)
                    {
                        set.Remove(match);
                    }
                })
                .ReturnsLazily((IEnumerable<TEntity> entities) => entities);
            return fakeDbSet;
        }
    }
}
