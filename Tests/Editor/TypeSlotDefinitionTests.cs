using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class TypeSlotDefinitionTests
    {
        private interface IAccepted
        {
        }

        private sealed class AcceptedEntity : IAccepted
        {
        }

        private sealed class RejectedEntity
        {
        }

        [Test]
        public void Match_WithAcceptedType_ReturnsTrue()
        {
            var definition = new TypeSlotDefinition<object, IAccepted>();

            Assert.That(definition.Match(new AcceptedEntity()), Is.True);
        }

        [Test]
        public void Match_WithRejectedType_ReturnsFalse()
        {
            var definition = new TypeSlotDefinition<object, IAccepted>();

            Assert.That(definition.Match(new RejectedEntity()), Is.False);
        }

        [Test]
        public void Match_WithNull_ReturnsFalse()
        {
            var definition = new TypeSlotDefinition<object, IAccepted>();

            Assert.That(definition.Match(null), Is.False);
        }
    }
}
