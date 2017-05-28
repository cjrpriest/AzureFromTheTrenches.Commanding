﻿using System;
using AccidentalFish.Commanding.Implementation;
using Moq;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
{
    public class AsyncLocalCommandScopeManagerTests
    {
        [Fact]
        public void EnterCreatesInitialContext()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object);

            // Act
            ICommandContext context = manager.Enter();

            // Assert
            Assert.Equal("someid", context.CorrelationId);
            Assert.Equal(0, context.Depth);
        }

        [Fact]
        public void ExitOfInitialContextDecrementsToMinusOne()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object);

            // Act
            ICommandContext context = manager.Enter();
            manager.Exit();

            // Assert
            Assert.Equal(-1, context.Depth);
        }

        [Fact]
        public void RepeatedEntryIncreasesDepth()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object);

            // Act
            ICommandContext context = manager.Enter();
            manager.Enter();
            manager.Enter();

            // Assert
            Assert.Equal(2, context.Depth);
        }

        [Fact]
        public void RepeatedEntryAndMatchingxitDecrementsToMinusOne()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object);

            // Act
            ICommandContext context = manager.Enter();
            manager.Enter();
            manager.Enter();
            manager.Exit();
            manager.Exit();
            manager.Exit();

            // Assert
            Assert.Equal(-1, context.Depth);
        }

        [Fact]
        public void EntryAfterFinalExitCreatesNewContext()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            correlationIdProvider.Setup(x => x.Create()).Returns(() => Guid.NewGuid().ToString());
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object);
            
            // Act
            ICommandContext initialContext = manager.Enter();
            manager.Exit();
            ICommandContext secondContext = manager.Enter();

            // Assert
            Assert.NotEqual(initialContext, secondContext);
            Assert.NotEqual(initialContext.CorrelationId, secondContext.CorrelationId);
        }
    }
}
