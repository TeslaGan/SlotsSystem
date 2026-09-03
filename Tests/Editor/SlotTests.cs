using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class SlotTests
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
            public bool IsAccepted { get; set; } = true;

            public bool Match(TestEntity entity)
            {
                return IsAccepted;
            }
        }

        [Test]
        public void Constructor_WithNullDefinition_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Slot<TestEntity>(null));
        }

        [Test]
        public void CanAccept_WithNull_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());

            Assert.That(slot.CanAccept(null), Is.False);
        }

        [Test]
        public void CanAccept_WhenDefinitionRejects_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition { IsAccepted = false });

            Assert.That(slot.CanAccept(new TestEntity(1)), Is.False);
        }

        [Test]
        public void CanAccept_WhenParentMatcherRejects_ReturnsFalse()
        {
            var slot = new Slot<TestEntity>(new TestDefinition(), entity => entity.Value > 10);

            Assert.That(slot.CanAccept(new TestEntity(5)), Is.False);
        }

        [Test]
        public void TrySet_WithAcceptedEntity_SetsContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);

            bool result = slot.TrySet(entity);

            Assert.That(result, Is.True);
            Assert.That(slot.Content, Is.SameAs(entity));
        }

        [Test]
        public void TrySet_WithRejectedEntity_KeepsContentAndDoesNotRaiseChanged()
        {
            var definition = new TestDefinition();
            var slot = new Slot<TestEntity>(definition);
            var first = new TestEntity(1);
            var second = new TestEntity(2);
            var changeCount = 0;

            slot.TrySet(first);
            definition.IsAccepted = false;
            slot.Changed += _ => changeCount++;

            bool result = slot.TrySet(second);

            Assert.That(result, Is.False);
            Assert.That(slot.Content, Is.SameAs(first));
            Assert.That(changeCount, Is.EqualTo(0));
        }

        [Test]
        public void TrySet_WithNull_ClearsContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);
            slot.TrySet(entity);

            bool result = slot.TrySet(null);

            Assert.That(result, Is.True);
            Assert.That(slot.Content, Is.Null);
        }

        [Test]
        public void TrySet_WithNull_DoesNotUseParentMatcher()
        {
            var slot = new Slot<TestEntity>(new TestDefinition(), _ => false);

            Assert.That(slot.TrySet(null), Is.True);
        }

        [Test]
        public void TrySet_RaisesChangedAfterContentIsUpdated()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);
            TestEntity contentDuringEvent = null;

            slot.Changed += _ => contentDuringEvent = slot.Content;
            slot.TrySet(entity);

            Assert.That(contentDuringEvent, Is.SameAs(entity));
        }

        [Test]
        public void TrySet_RaisesChangedWithPreviousAndCurrentContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var first = new TestEntity(1);
            var second = new TestEntity(2);
            SlotChangeData<TestEntity> change = default;

            slot.TrySet(first);
            slot.Changed += value => change = value;
            slot.TrySet(second);

            Assert.That(change.Slot, Is.SameAs(slot));
            Assert.That(change.PreviousContent, Is.SameAs(first));
            Assert.That(change.Content, Is.SameAs(second));
        }

        [Test]
        public void TrySet_WithSameContent_RaisesChanged()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);
            var changeCount = 0;

            slot.TrySet(entity);
            slot.Changed += _ => changeCount++;
            slot.TrySet(entity);

            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void TrySet_WithNullOnEmptySlot_RaisesChanged()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var changeCount = 0;

            slot.Changed += _ => changeCount++;
            slot.TrySet(null);

            Assert.That(changeCount, Is.EqualTo(1));
        }
    }
}
