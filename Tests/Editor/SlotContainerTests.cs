using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class SlotContainerTests
    {
        private sealed class TestEntity
        {
            public TestEntity(int value = 1)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private sealed class TestDefinition : ISlotDefinition<TestEntity>
        {
            public TestDefinition(bool accepts = true)
            {
                Accepts = accepts;
            }

            public bool Accepts { get; }

            public bool Match(TestEntity entity)
            {
                return Accepts && entity.Value > 0;
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

            ISlotContainer<TestEntity> mutableContainer = container;
            IReadOnlySlotContainer<TestEntity> readOnlyContainer = mutableContainer;

            Assert.That(mutableContainer.Slots.Count, Is.EqualTo(1));
            Assert.That(mutableContainer.Slots[0], Is.SameAs(slot));
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

            ISlotContainer<TestEntity> mutableContainer = container;
            IReadOnlySlotContainer<TestEntity> readOnlyContainer = mutableContainer;

            Assert.That(result, Is.True);
            Assert.That(mutableContainer.Slots, Is.Empty);
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
        public void AccessContainer_Add_WithNullAccess_ThrowsArgumentNullException()
        {
            var container = new SlotContainerAccess<TestEntity>();

            Assert.Throws<ArgumentNullException>(() => container.Add(null));
        }

        [Test]
        public void AccessContainer_Add_ExposesAccessThroughAccessAndReadOnlyViews()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(access);

            ISlotContainerAccess<TestEntity> accessContainer = container;
            IReadOnlySlotContainer<TestEntity> readOnlyContainer = accessContainer;

            Assert.That(accessContainer.Slots.Count, Is.EqualTo(1));
            Assert.That(accessContainer.Slots[0], Is.SameAs(access));
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
        public void AccessContainer_Remove_WithNullAccess_ThrowsArgumentNullException()
        {
            var container = new SlotContainerAccess<TestEntity>();

            Assert.Throws<ArgumentNullException>(() => container.Remove(null));
        }

        [Test]
        public void AccessContainer_Remove_WithMissingAccess_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            bool result = container.Remove(access);

            Assert.That(result, Is.False);
        }

        [Test]
        public void AccessContainer_Remove_WithContainedAccess_RemovesFromBothViews()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(access);

            bool result = container.Remove(access);

            ISlotContainerAccess<TestEntity> accessContainer = container;
            IReadOnlySlotContainer<TestEntity> readOnlyContainer = accessContainer;

            Assert.That(result, Is.True);
            Assert.That(accessContainer.Slots, Is.Empty);
            Assert.That(readOnlyContainer.Slots, Is.Empty);
        }

        [Test]
        public void AccessContainer_TryInsert_InsertsIntoFirstSuccessfulSlot()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            firstSlot.TrySet(new TestEntity(1));

            container.Add(new SlotAccess<TestEntity>(firstSlot));
            container.Add(new SlotAccess<TestEntity>(secondSlot));

            var entity = new TestEntity(2);

            bool result = container.TryInsert(entity);

            Assert.That(result, Is.True);
            Assert.That(firstSlot.Content.Value, Is.EqualTo(1));
            Assert.That(secondSlot.Content, Is.SameAs(entity));
        }

        [Test]
        public void AccessContainer_TryInsert_WhenFirstSlotRejectsByDefinition_UsesNextSlot()
        {
            var rejectingSlot = new Slot<TestEntity>(new TestDefinition(false));
            var acceptingSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(new SlotAccess<TestEntity>(rejectingSlot));
            container.Add(new SlotAccess<TestEntity>(acceptingSlot));

            var entity = new TestEntity(1);

            bool result = container.TryInsert(entity);

            Assert.That(result, Is.True);
            Assert.That(rejectingSlot.Content, Is.Null);
            Assert.That(acceptingSlot.Content, Is.SameAs(entity));
        }

        [Test]
        public void AccessContainer_TryInsert_WhenFirstSlotRejectsByAccessMatcher_UsesNextSlot()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(new SlotAccess<TestEntity>(firstSlot, _ => false));
            container.Add(new SlotAccess<TestEntity>(secondSlot));

            var entity = new TestEntity(1);

            bool result = container.TryInsert(entity);

            Assert.That(result, Is.True);
            Assert.That(firstSlot.Content, Is.Null);
            Assert.That(secondSlot.Content, Is.SameAs(entity));
        }

        [Test]
        public void AccessContainer_TryInsert_WhenNoSlotAccepts_ReturnsFalse()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            container.Add(new SlotAccess<TestEntity>(firstSlot, _ => false));
            container.Add(new SlotAccess<TestEntity>(secondSlot, _ => false));

            bool result = container.TryInsert(new TestEntity(1));

            Assert.That(result, Is.False);
            Assert.That(firstSlot.Content, Is.Null);
            Assert.That(secondSlot.Content, Is.Null);
        }

        [Test]
        public void AccessContainer_TryExtract_ExtractsFirstMatchingAvailableEntity()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var thirdSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            firstSlot.TrySet(new TestEntity(1));
            secondSlot.TrySet(new TestEntity(2));
            thirdSlot.TrySet(new TestEntity(2));

            container.Add(new SlotAccess<TestEntity>(firstSlot));
            container.Add(new SlotAccess<TestEntity>(secondSlot));
            container.Add(new SlotAccess<TestEntity>(thirdSlot));

            bool result = container.TryExtract(entity => entity.Value == 2, out TestEntity extracted);

            Assert.That(result, Is.True);
            Assert.That(extracted.Value, Is.EqualTo(2));
            Assert.That(firstSlot.Content.Value, Is.EqualTo(1));
            Assert.That(secondSlot.Content, Is.Null);
            Assert.That(thirdSlot.Content.Value, Is.EqualTo(2));
        }

        [Test]
        public void AccessContainer_TryExtract_WhenMatchingSlotRejectsExtraction_ContinuesSearching()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            var firstEntity = new TestEntity(2);
            var secondEntity = new TestEntity(2);

            firstSlot.TrySet(firstEntity);
            secondSlot.TrySet(secondEntity);

            container.Add(new SlotAccess<TestEntity>(firstSlot, extractMatcher: _ => false));
            container.Add(new SlotAccess<TestEntity>(secondSlot));

            bool result = container.TryExtract(entity => entity.Value == 2, out TestEntity extracted);

            Assert.That(result, Is.True);
            Assert.That(extracted, Is.SameAs(secondEntity));
            Assert.That(firstSlot.Content, Is.SameAs(firstEntity));
            Assert.That(secondSlot.Content, Is.Null);
        }

        [Test]
        public void AccessContainer_TryExtract_SkipsEmptySlots()
        {
            var emptySlot = new Slot<TestEntity>(new TestDefinition());
            var filledSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();
            var entity = new TestEntity(2);

            filledSlot.TrySet(entity);

            container.Add(new SlotAccess<TestEntity>(emptySlot));
            container.Add(new SlotAccess<TestEntity>(filledSlot));

            bool result = container.TryExtract(item => item.Value == 2, out TestEntity extracted);

            Assert.That(result, Is.True);
            Assert.That(extracted, Is.SameAs(entity));
            Assert.That(emptySlot.Content, Is.Null);
            Assert.That(filledSlot.Content, Is.Null);
        }

        [Test]
        public void AccessContainer_TryExtract_WhenAllMatchingSlotsRejectExtraction_ReturnsFalseAndDefault()
        {
            var firstSlot = new Slot<TestEntity>(new TestDefinition());
            var secondSlot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();
            var firstEntity = new TestEntity(2);
            var secondEntity = new TestEntity(2);

            firstSlot.TrySet(firstEntity);
            secondSlot.TrySet(secondEntity);

            container.Add(new SlotAccess<TestEntity>(firstSlot, extractMatcher: _ => false));
            container.Add(new SlotAccess<TestEntity>(secondSlot, extractMatcher: _ => false));

            bool result = container.TryExtract(item => item.Value == 2, out TestEntity extracted);

            Assert.That(result, Is.False);
            Assert.That(extracted, Is.Null);
            Assert.That(firstSlot.Content, Is.SameAs(firstEntity));
            Assert.That(secondSlot.Content, Is.SameAs(secondEntity));
        }

        [Test]
        public void AccessContainer_TryExtract_WhenNoEntityMatches_ReturnsFalseAndDefault()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var container = new SlotContainerAccess<TestEntity>();

            slot.TrySet(new TestEntity(1));
            container.Add(new SlotAccess<TestEntity>(slot));

            bool result = container.TryExtract(entity => entity.Value == 2, out TestEntity extracted);

            Assert.That(result, Is.False);
            Assert.That(extracted, Is.Null);
            Assert.That(slot.Content.Value, Is.EqualTo(1));
        }

        [Test]
        public void AccessContainer_TryExtract_WithNullMatcher_ThrowsArgumentNullException()
        {
            var container = new SlotContainerAccess<TestEntity>();

            Assert.Throws<ArgumentNullException>(() => container.TryExtract(null, out _));
        }
    }
}
