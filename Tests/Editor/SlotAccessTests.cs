using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class SlotAccessTests
    {
        private sealed class TestEntity
        {
            public TestEntity(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private sealed class TestDefinition : ISlotDefinition<TestEntity>
        {
            public bool Match(TestEntity entity)
            {
                return entity.Value > 0;
            }
        }

        [Test]
        public void Constructor_WithNullSlot_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SlotAccess<TestEntity>(null));
        }

        [Test]
        public void CanInsert_WhenSlotIsEmptyAndMatchersAccept_ReturnsTrue()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot, entity => entity.Value < 10);

            Assert.That(access.CanInsert(new TestEntity(5)), Is.True);
        }

        [Test]
        public void CanInsert_WhenSlotIsOccupied_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);

            slot.TrySet(new TestEntity(1));

            Assert.That(access.CanInsert(new TestEntity(2)), Is.False);
        }

        [Test]
        public void CanInsert_WhenDefinitionRejects_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);

            Assert.That(access.CanInsert(new TestEntity(0)), Is.False);
        }

        [Test]
        public void CanInsert_WhenInsertMatcherRejects_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot, _ => false);

            Assert.That(access.CanInsert(new TestEntity(1)), Is.False);
        }

        [Test]
        public void TryInsert_WhenAllowed_SetsSlotContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var entity = new TestEntity(1);

            bool result = access.TryInsert(entity);

            Assert.That(result, Is.True);
            Assert.That(slot.Content, Is.SameAs(entity));
        }

        [Test]
        public void CanExtract_WhenSlotIsEmpty_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);

            Assert.That(access.CanExtract, Is.False);
        }

        [Test]
        public void CanExtract_WhenExtractMatcherRejects_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot, extractMatcher: _ => false);

            slot.TrySet(new TestEntity(1));

            Assert.That(access.CanExtract, Is.False);
        }

        [Test]
        public void TryExtract_WhenAllowed_ClearsSlotAndReturnsContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var entity = new TestEntity(1);

            slot.TrySet(entity);

            bool result = access.TryExtract(out TestEntity extracted);

            Assert.That(result, Is.True);
            Assert.That(extracted, Is.SameAs(entity));
            Assert.That(slot.Content, Is.Null);
        }

        [Test]
        public void TryExtract_WhenRejected_KeepsContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot, extractMatcher: _ => false);
            var entity = new TestEntity(1);

            slot.TrySet(entity);

            bool result = access.TryExtract(out TestEntity extracted);

            Assert.That(result, Is.False);
            Assert.That(extracted, Is.SameAs(entity));
            Assert.That(slot.Content, Is.SameAs(entity));
        }

        [Test]
        public void SlotTrySet_BypassesAccessMatchers()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot, _ => false, _ => false);
            var entity = new TestEntity(1);

            bool result = slot.TrySet(entity);

            Assert.That(result, Is.True);
            Assert.That(access.Content, Is.SameAs(entity));
        }

        [Test]
        public void Changed_ForwardsUnderlyingSlotEvent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var access = new SlotAccess<TestEntity>(slot);
            var entity = new TestEntity(1);
            SlotChangeData<TestEntity> change = default;

            access.Changed += value => change = value;
            slot.TrySet(entity);

            Assert.That(change.Slot, Is.SameAs(slot));
            Assert.That(change.Content, Is.SameAs(entity));
        }
    }
}
