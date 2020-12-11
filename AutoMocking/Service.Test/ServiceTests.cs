using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Smocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Test
{
    [TestFixture]
	class ServiceTests
    {
        private static readonly DateTime FDatePrev = new DateTime(2000, 1, 1);
        private static readonly DateTime FDateNow = new DateTime(2020, 1, 1);

        State state;
		[SetUp]
		public void Setup()
		{
			MyMapper.Initialize();
			state = GetState();
		}

		[Test]
        public void AddTask_Failed()
        {
            var task = new DealTask
            {
                Cid = "00",
                DealSer = 13,
                ProductId = 15,
                TaskStatus = (byte)TaskStatus.Finished,
                TypeId = 1
            };
            var result = state.Service.AddTask(task);
            Assert.IsFalse(result);
            Assert.AreEqual(state.DealTasksInitialCount, state.EfContext.DealTasks.Count());
        }

        [Test]
        public void AddTask_Succeed()
        {
            var task = new DealTask
            {
                Cid = "21",
                DealSer = 13,
                ProductId = 15,
                TaskStatus = (byte)TaskStatus.Finished,
                TypeId = 1
            };
            state.AffectedEntities = 1;
            var result = state.Service.AddTask(task);
            Assert.IsTrue(result);
            Assert.AreEqual(state.DealTasksInitialCount + 1, state.EfContext.DealTasks.Count());
            var newItem = state.EfContext.DealTasks.Last();
            newItem.Should().BeEquivalentTo(new Item
            {
                Cid = "21",
                DealSer = 13,
                ProductId = 15,
                TimeSent = null,
                TimeRecv = null,
                TaskStatus = (byte)TaskStatus.Finished,
                TypeId = 1,
                Context = new byte[] { }
            });
        }

        [Test]
        public void ArchiveTask_Failed()
        {
            var service = state.Service;
            var result = service.ArchiveTask("21");
            Assert.IsFalse(result);
            Assert.AreEqual(state.DealTasksInitialCount, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksArchInitialCount, state.EfContext.DealTasksArch.Count());
        }

        [Test]
        public void ArchiveTask_Succeed()
        {
            var service = state.Service;
            var result = service.ArchiveTask("11");
            Assert.IsTrue(result);
            Assert.AreEqual(state.DealTasksInitialCount - 1, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksArchInitialCount + 1, state.EfContext.DealTasksArch.Count());
            var actual = state.EfContext.DealTasksArch.Last();
            actual.Should().BeEquivalentTo(new ItemArch
            {
                Id = 1,
                TypeId = 2,
                DealSer = 3,
                Cid = "11",
                TimeSent = FDatePrev,
                TimeRecv = FDatePrev.AddDays(1),
                TaskStatus = 1,
                ProductId = 5,
                Context = new byte[] { }
            });
        }

        [Test]
        public void ArchiveDeal_Failed()
        {
            var service = state.Service;
            var result = service.ArchiveDeal(0, 0);
            Assert.IsFalse(result);
            Assert.AreEqual(state.DealTasksInitialCount, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksArchInitialCount, state.EfContext.DealTasksArch.Count());
        }

        [Test]
        public void ArchiveDeal_Succeed()
        {
            var service = state.Service;
            var result = service.ArchiveDeal(3, 5);
            Assert.True(result);
            const int count = 2;

            Assert.AreEqual(state.DealTasksInitialCount - count, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksArchInitialCount + count, state.EfContext.DealTasksArch.Count());
            var actual = state.EfContext.DealTasksArch.Where(item => item.DealSer == 3 && item.ProductId == 5).ToArray();
            var expected = GetDealTasks()
                .Where(item => item.DealSer == 3 && item.ProductId == 5)
                .Select(item => new ItemArch
                {
                    Id = item.Id,
                    TypeId = 2,
                    DealSer = item.DealSer,
                    Cid = item.Cid,
                    TimeSent = item.TimeSent,
                    TimeRecv = item.TimeRecv,
                    ProductId = item.ProductId,
                    TaskStatus = item.TaskStatus,
                    Context = new byte[] { }
                });
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CancelTask_Failed()
        {
            var service = state.Service;
            var result = service.CanceledTask("21");
            Assert.IsFalse(result);

            Assert.AreEqual(state.DealTasksInitialCount, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksCanceledInitialCount, state.EfContext.DealTasksCanceled.Count());
        }

        [Test]
        public void CancelTask_Succeed()
        {
            var service = state.Service;
            var result = service.CanceledTask("11");
            Assert.IsTrue(result);

            Assert.AreEqual(state.DealTasksInitialCount - 1, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksCanceledInitialCount + 1, state.EfContext.DealTasksCanceled.Count());
            var actual = state.EfContext.DealTasksCanceled.Last();
            actual.Should().BeEquivalentTo(new ItemCanceled
            {
                Id = 1,
                TypeId = 2,
                DealSer = 3,
                Cid = "11",
                TimeSent = FDatePrev,
                TimeRecv = FDatePrev.AddDays(1),
                TaskStatus = (byte)TaskStatus.Canceled,
                ProductId = 5,
                Context = new byte[] { }
            });
        }

        [Test]
        public void CancelDeal_Failed()
        {
            var service = state.Service;
            var result = service.CanceledDeal(0, 0);
            Assert.IsFalse(result);
            Assert.AreEqual(state.DealTasksInitialCount, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksCanceledInitialCount, state.EfContext.DealTasksCanceled.Count());
        }

        [Test]
        public void CancelDeal_Succeed()
        {
            var service = state.Service;
            var result = service.CanceledDeal(3, 5);
            Assert.IsTrue(result);
            const int count = 2;

            Assert.AreEqual(state.DealTasksInitialCount - count, state.EfContext.DealTasks.Count());
            Assert.AreEqual(state.DealTasksCanceledInitialCount + count, state.EfContext.DealTasksCanceled.Count());

            var actual = state.EfContext.DealTasksCanceled.Where(item => item.DealSer == 3 && item.ProductId == 5).ToArray();
            var expected = GetDealTasks()
                .Where(item => item.DealSer == 3 && item.ProductId == 5)
                .Select(item => new ItemCanceled
                {
                    Id = item.Id,
                    TypeId = 2,
                    DealSer = item.DealSer,
                    Cid = item.Cid,
                    TimeSent = item.TimeSent,
                    TimeRecv = item.TimeRecv,
                    TaskStatus = item.TaskStatus,
                    ProductId = item.ProductId,
                    Context = new byte[] { }
                });

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetNextTaskId_ByCid_Failed()
        {
            var service = state.Service;
            var result = service.GetNextTaskId("10", out _);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("11", "15")]
        [TestCase("12", null)]
        [TestCase("15", "15")]
        public void GetNextTaskId_ByCid_Succeed(string cid, string nextCid)
        {
            var service = state.Service;
            var result = service.GetNextTaskId(cid, out var actualNextId);
            Assert.IsTrue(result);
            Assert.AreEqual(nextCid, actualNextId);
        }

        [Test]
        public void GetNextTaskId_ByDealSerAndProductId_Failed()
        {
            var service = state.Service;
            var result = service.GetNextTaskId(0, 0, out _);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(3, 5, "15")]
        [TestCase(4, 5, null)]
        public void GetNextTaskId_ByDealSerAndProductId_Succeed(int dealSer, int productId, string nextCid)
        {
            var service = state.Service;
            var result = service.GetNextTaskId(dealSer, productId, out var actualNextCid);
            Assert.IsTrue(result);
            Assert.AreEqual(nextCid, actualNextCid);
        }

        [Test]
        [TestCase(TaskStatus.New)]
        [TestCase(TaskStatus.Sent)]
        [TestCase(TaskStatus.Finished)]
        [TestCase(TaskStatus.Canceled)]
        [TestCase(TaskStatus.Timeout)]
        public void SetTaskStatus_Failed(TaskStatus taskStatus)
        {
            var service = state.Service;
            var result = service.SetTaskStatus("00", taskStatus);
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase("11", TaskStatus.Sent)]
        [TestCase("12", TaskStatus.Finished)]
        [TestCase("13", TaskStatus.Canceled)]
        [TestCase("14", TaskStatus.Timeout)]
        [TestCase("15", TaskStatus.New)]
        public void SetTaskStatus_Succeed(string cid, TaskStatus taskStatus)
        {
            var service = state.Service;
            var result = service.SetTaskStatus(cid, taskStatus);
            Assert.IsTrue(result);
            A.CallTo(() => state.EfContext.SaveChanges()).MustNotHaveHappened();
        }

        [Test]
        [TestCase("11", TaskStatus.New)]
        [TestCase("12", TaskStatus.Sent, true, false)]
        [TestCase("13", TaskStatus.Finished, false, true)]
        [TestCase("14", TaskStatus.Canceled)]
        [TestCase("15", TaskStatus.Timeout)]
        public void GetNextTaskId_NotSameStatus_succeed(string cid, TaskStatus status,
            bool sentDateChanged = false,
            bool recvDateChanged = false)
        {
            bool result = false;

            byte entityStatus = default;

            DateTime? fDateS = null;
            DateTime? fDateR = null;

            Smock.Run((context) =>
            {
                context.Setup(() => DateTime.Now).Returns(FDateNow);

                var stateS = GetState();
                var service = stateS.Service;
                result = service.SetTaskStatus(cid, status);

				A.CallTo(() => stateS.EfContext.SaveChanges()).MustHaveHappenedOnceExactly();
				var entity = stateS.EfContext.DealTasks.First(item => item.Cid == cid);

                entityStatus = entity.TaskStatus;

                fDateS = entity.TimeSent;
                fDateR = entity.TimeRecv;
			});

            Assert.IsTrue(result);

            Assert.AreEqual((byte)status, entityStatus);
            Assert.AreEqual(sentDateChanged ? FDateNow : FDatePrev, fDateS);
			Assert.AreEqual(recvDateChanged ? FDateNow : FDatePrev.AddDays(1), fDateR);
		}

        [Test]
        public void GetTask_Failed()
		{
            var service = state.Service;
            var result = service.GetTask("00");
            Assert.IsNull(result);
		}

        [Test]
        public void GetTask_Succeed()
        {
            var service = state.Service;
            var actual = service.GetTask("11");
            var expected = new DealTask
            {
                Id = 1,
                TypeId = 2,
                DealSer = 3,
                Cid = "11",
                TaskStatus = (byte)TaskStatus.Sent,
                ProductId = 5,
                TimeSent = FDatePrev,
                TimeRecv = FDatePrev.AddDays(1),
                Context = new byte[] { }
            };
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetAllTasksCids_ByType()
		{
            var service = state.Service;
            var actual = service.GetAllTaskCids(2);
            var expected = GetDealTasks().Where(item => item.TypeId == 2).Select(item => item.Cid).ToList();
            actual.Should().BeEquivalentTo(expected);
		}

        [Test]
        public void GetAllTasksCids_ByTypeDealProduct()
		{
            var service = state.Service;
            var actual = service.GetAllTaskCids(2, 3, 5);
            var expected = GetDealTasks()
                .Where(item => item.TypeId == 2
                                && item.DealSer == 3
                                && item.ProductId == 5)
                .Select(item => item.Cid)
                .ToList();
            actual.Should().BeEquivalentTo(expected);
		}

        [Test]
        public void GetAllTasksCids_NoFilter()
		{
            var service = state.Service;
            var actual = service.GetAllTaskCids();
            var expected = GetDealTasks().Select(i => i.Cid).ToList();
            actual.Should().BeEquivalentTo(expected);
		}

        private State GetState()
        {
            var result = new State();
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
            result.EfContext = fixture.Freeze<Fake<IEfContext>>().FakedObject;

            var dealTasks = GetDealTasks().ToList().ConfigureDbSet();
            result.DealTasksInitialCount = dealTasks.Count();

            A.CallTo(() => dealTasks.Add(A<Item>.That.Matches(item => item.Cid == "00")))
                .Throws(() => new Exception());

            var dealArchTasks = GetDealArchTasks().ToList().ConfigureDbSet();
            result.DealTasksArchInitialCount = dealArchTasks.Count();

            var dealCanceledTasks = GetDealCanceledTasks().ToList().ConfigureDbSet();
            result.DealTasksCanceledInitialCount = dealCanceledTasks.Count();

            A.CallTo(() => result.EfContext.DealTasks).Returns(dealTasks);

            A.CallTo(() => result.EfContext.DealTasks).Returns(dealTasks);
            A.CallTo(() => result.EfContext.DealTasksArch).Returns(dealArchTasks);
            A.CallTo(() => result.EfContext.DealTasksCanceled).Returns(dealCanceledTasks);
            A.CallTo(() => result.EfContext.SaveChanges()).ReturnsLazily(() => result.AffectedEntities);

            fixture.Inject(result.EfContext);
            result.Service = fixture.Create<Service>();

            return result;
        }

        private IEnumerable<Item> GetDealTasks()
        {
            return (
                from g in new[] { 1, 2 }
                from product in new[] { 5, 6 }
                from dealser in new[] { 3, 4 }
                select new { product, dealser }
                )
                .Select((item, index) => new Item
                {
                    Id = index + 1,
                    TypeId = 2,
                    DealSer = item.dealser,
                    Cid = "1" + (index + 1),
                    TimeSent = FDatePrev,
                    TimeRecv = FDatePrev.AddDays(1),
                    TaskStatus = (byte)((index + 1) % 5),
                    ProductId = item.product
                });
        }

        private IEnumerable<ItemArch> GetDealArchTasks()
        {
            return new[]
            {
                new ItemArch
                {
                    Id = 1,
                    TypeId = 2,
                    DealSer = 7,
                    Cid = "11",
                    TimeSent = FDatePrev,
                    TimeRecv = FDatePrev.AddDays(1),
                    TaskStatus = (int)TaskStatus.Finished,
                    ProductId = 8
                }
            };
        }

        private IEnumerable<ItemCanceled> GetDealCanceledTasks()
        {
            return new[]
            {
                new ItemCanceled
                {
                    Id = 1,
                    TypeId = 2,
                    DealSer = 7,
                    Cid = "11",
                    TimeSent = FDatePrev,
                    TimeRecv = FDatePrev.AddDays(1),
                    TaskStatus = (int)TaskStatus.Canceled,
                    ProductId = 8
                }
            };
        }

        private class State
        {
            public IService Service { get; set; }
            public IEfContext EfContext { get; set; }
            public int AffectedEntities { get; set; }
            public int DealTasksInitialCount { get; set; }
            public int DealTasksArchInitialCount { get; set; }
            public int DealTasksCanceledInitialCount { get; set; }
        }
    }
}
