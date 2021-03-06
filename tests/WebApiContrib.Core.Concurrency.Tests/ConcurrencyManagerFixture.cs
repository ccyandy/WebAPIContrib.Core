﻿using Moq;
using System;
using System.Threading.Tasks;
using WebApiContrib.Core.Concurrency.Storage;
using Xunit;
using WebApiContrib.Core.Concurrency;

namespace WebApiContrib.Core.Concurrency.Tests
{
    public class ConcurrencyManagerFixture
    {
        private Mock<IStorage> _storageStub;

        private IConcurrencyManager _concurrencyManager;

        private class Customer
        {
            public string FirstName { get; set; }
        }

        [Fact]
        public void When_Passing_Null_Parameters_Then_Exceptions_Are_Thrown()
        {
            // ARRANGE
            InitializeFakeObjects();

            // ACT & ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(() => _concurrencyManager.TryUpdateRepresentationAsync(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _concurrencyManager.IsRepresentationDifferentAsync(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _concurrencyManager.IsRepresentationDifferentAsync("name", null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _concurrencyManager.TryGetRepresentationAsync(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => _concurrencyManager.RemoveAsync(null));
        }

        [Fact]
        public async Task When_Resource_Is_Removed_Then_Operation_Is_Called()
        {
            // ARRANGE
            const string name = "name";
            InitializeFakeObjects();

            // ACT
            await _concurrencyManager.RemoveAsync(name);

            // ASSERT
            _storageStub.Verify(s => s.RemoveAsync(name));
        }

        [Fact]
        public async Task When_Value_Doesnt_Exist_Then_Null_Is_Returned()
        {
            // ARRANGE
            const string name = "name";
            InitializeFakeObjects();
            _storageStub.Setup(s => s.TryGetValueAsync(name))
                .Returns(Task.FromResult<ConcurrentObject>(null));

            // ACT
            var result = await _concurrencyManager.TryGetRepresentationAsync(name);

            // ASSERT
            Assert.Null(result);
        }

        private void InitializeFakeObjects()
        {
            _storageStub = new Mock<IStorage>();
            var options = new ConcurrencyOptions
            {
                Storage = _storageStub.Object
            };
            _concurrencyManager = new ConcurrencyManager(options);
        }
    }
}
