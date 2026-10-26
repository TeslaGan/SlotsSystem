using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class SlotContainerTests
    {
        private sealed class TestEntity
        {
        }

        private sealed class TestDefinition : ISlotDefinition<TestEntity>
        {
            public bool Match(TestEntity entity)
            {
                return true;
            }
        }

        [Test]
        public void Add_WithNullSlot_ThrowsArgumentNullException()
        {
            var container = new SlotContainer<TestEntity>();

            Assert.Throws<ArgumentNullException>(() => container.Add(null));
        }

        [Test]
        public void Add_WithSameSlotTwice_ThrowsArgumentException()
        {
            var container = new SlotContainer<TestEntity>();
            var slot = new Slot<TestEntity>(new TestDefinition());

            container.Add(slot);

            Assert.Throws<ArgumentException>(() => container.Add(slot));
        }

        [Test]
        public void Add_ExposesSlotThroughMutableAndReadOnlyViews()
        {
            var container = new SlotContainer<TestEntity>();
            var slot = new Slot<TestEntity>(new TestDefinition());

            container.Add(slot);

            IReadOnlySlotContainer<TestEntity> readOnlyContainer = container;

            Assert.That(container.Slots.Count, Is.EqualTo(1));
            Assert.That(container.Slots[0], Is.SameAs(slot));
            Assert.That(readOnlyContainer.Slots.Count, Is.EqualTo(1));
            Assert.That(readOnlyContainer.Slots[0], Is.SameAs(slot));
        }

        [Test]
        public void Remove_WithContainedSlot_RemovesFromMutableAndReadOnlyViews()
        {
            var container = new SlotContainer<TestEntity>();
            var slot = new Slot<TestEntity>(new TestDefinition());

            container.Add(slot);

            bool result = container.Remove(slot);

            IReadOnlySlotContainer<TestEntity> readOnlyContainer = container;

            Assert.That(result, Is.True);
            Assert.That(container.Slots, Is.Empty);
            Assert.That(readOnlyContainer.Slots, Is.Empty);
        }

        [Test]
        public void Remove_WithMissingSlot_ReturnsFalse()
        {
            var container = new SlotContainer<TestEntity>();
            var slot = new Slot<TestEntity>(new TestDefinition());

            bool result = container.Remove(slot);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AccessContainer_Add_ExposesAccessThroughAccessAndReadOnlyViews()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(access);

            IReadOnlySlotContainer<TestEntity> readOnlyContainer = container;

            Assert.That(container.Slots.Count, Is.EqualTo(1));
            Assert.That(container.Slots[0], Is.SameAs(access));
            Assert.That(readOnlyContainer.Slots.Count, Is.EqualTo(1));
            Assert.That(readOnlyContainer.Slots[0], Is.SameAs(access));
        }

        [Test]
        public void AccessContainer_Add_WithSameAccessTwice_ThrowsArgumentException()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(access);

            Assert.Throws<ArgumentException>(() => container.Add(access));
        }

        [Test]
        public void AccessContainer_Remove_WithContainedAccess_RemovesFromBothViews()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(access);

            bool result = container.Remove(access);

            IReadOnlySlotContainer<TestEntity> readOnlyContainer = container;

            Assert.That(result, Is.True);
            Assert.That(container.Slots, Is.Empty);
            Assert.That(readOnlyContainer.Slots, Is.Empty);
        }
    }
}
