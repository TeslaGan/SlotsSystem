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
            var definition = new TestDefinition
            {
                IsAccepted = false
            };

            var slot = new Slot<TestEntity>(definition);

            Assert.That(slot.CanAccept(new TestEntity(1)), Is.False);
        }

        [Test]
        public void CanAccept_WhenDefinitionAccepts_ReturnsTrue()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());

            Assert.That(slot.CanAccept(new TestEntity(1)), Is.True);
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
        public void TrySet_WithRejectedEntity_ReturnsFalseAndKeepsContent()
        {
            var definition = new TestDefinition();
            var slot = new Slot<TestEntity>(definition);
            var first = new TestEntity(1);
            var second = new TestEntity(2);

            slot.TrySet(first);
            definition.IsAccepted = false;

            bool result = slot.TrySet(second, out TestEntity previousContent);

            Assert.That(result, Is.False);
            Assert.That(previousContent, Is.SameAs(first));
            Assert.That(slot.Content, Is.SameAs(first));
        }

        [Test]
        public void TrySet_WithNull_ClearsContentAndReturnsPreviousContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);

            slot.TrySet(entity);

            bool result = slot.TrySet(null, out TestEntity previousContent);

            Assert.That(result, Is.True);
            Assert.That(previousContent, Is.SameAs(entity));
            Assert.That(slot.Content, Is.Null);
        }

        [Test]
        public void TrySet_WithNull_DoesNotUseDefinition()
        {
            var definition = new TestDefinition
            {
                IsAccepted = false
            };

            var slot = new Slot<TestEntity>(definition);

            bool result = slot.TrySet(null);

            Assert.That(result, Is.True);
            Assert.That(slot.Content, Is.Null);
        }

        [Test]
        public void TrySet_ReplacesExistingContentAndReturnsPreviousContent()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var first = new TestEntity(1);
            var second = new TestEntity(2);

            slot.TrySet(first);

            bool result = slot.TrySet(second, out TestEntity previousContent);

            Assert.That(result, Is.True);
            Assert.That(previousContent, Is.SameAs(first));
            Assert.That(slot.Content, Is.SameAs(second));
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
        public void TrySet_WithRejectedEntity_DoesNotRaiseChanged()
        {
            var definition = new TestDefinition
            {
                IsAccepted = false
            };

            var slot = new Slot<TestEntity>(definition);
            var changeCount = 0;

            slot.Changed += _ => changeCount++;

            slot.TrySet(new TestEntity(1));

            Assert.That(changeCount, Is.EqualTo(0));
        }

        [Test]
        public void TrySet_WithSameContent_RaisesChanged()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var entity = new TestEntity(1);
            var changeCount = 0;

            slot.TrySet(entity);
            slot.Changed += _ => changeCount++;

            bool result = slot.TrySet(entity);

            Assert.That(result, Is.True);
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void TrySet_WithNullOnEmptySlot_RaisesChanged()
        {
            var slot = new Slot<TestEntity>(new TestDefinition());
            var changeCount = 0;

            slot.Changed += _ => changeCount++;

            bool result = slot.TrySet(null);

            Assert.That(result, Is.True);
            Assert.That(changeCount, Is.EqualTo(1));
        }
    }
}
