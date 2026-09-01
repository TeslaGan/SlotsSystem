using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class EnumSlotDefinitionTests
    {
        [Flags]
        private enum TestFlags
        {
            None = 0,
            First = 1 << 0,
            Second = 1 << 1,
            Third = 1 << 2
        }

        private sealed class TestEntity : IFlagged<TestFlags>
        {
            public TestEntity(TestFlags flags)
            {
                Flags = flags;
            }

            public TestFlags Flags { get; }
        }

        [Test]
        public void Match_Any_WithMatchingFlag_ReturnsTrue()
        {
            var definition = new EnumSlotDefinition<TestEntity, TestFlags>(TestFlags.First | TestFlags.Second, FlagMatchMode.Any);
            var entity = new TestEntity(TestFlags.Second);

            Assert.That(definition.Match(entity), Is.True);
        }

        [Test]
        public void Match_Any_WithoutMatchingFlag_ReturnsFalse()
        {
            var definition = new EnumSlotDefinition<TestEntity, TestFlags>(TestFlags.First | TestFlags.Second, FlagMatchMode.Any);
            var entity = new TestEntity(TestFlags.Third);

            Assert.That(definition.Match(entity), Is.False);
        }

        [Test]
        public void Match_All_WithAllRequiredFlags_ReturnsTrue()
        {
            var definition = new EnumSlotDefinition<TestEntity, TestFlags>(TestFlags.First | TestFlags.Second, FlagMatchMode.All);
            var entity = new TestEntity(TestFlags.First | TestFlags.Second | TestFlags.Third);

            Assert.That(definition.Match(entity), Is.True);
        }

        [Test]
        public void Match_All_WithMissingRequiredFlag_ReturnsFalse()
        {
            var definition = new EnumSlotDefinition<TestEntity, TestFlags>(TestFlags.First | TestFlags.Second, FlagMatchMode.All);
            var entity = new TestEntity(TestFlags.First);

            Assert.That(definition.Match(entity), Is.False);
        }

        [Test]
        public void Match_WithInvalidMode_ThrowsArgumentOutOfRangeException()
        {
            var definition = new EnumSlotDefinition<TestEntity, TestFlags>(TestFlags.First, (FlagMatchMode)int.MaxValue);
            var entity = new TestEntity(TestFlags.First);

            Assert.Throws<ArgumentOutOfRangeException>(() => definition.Match(entity));
        }
    }
}
