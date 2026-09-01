using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class SlotDefinitionTests
    {
        [Test]
        public void Match_ReturnsTrue()
        {
            var definition = new SlotDefinition<object>();

            Assert.That(definition.Match(new object()), Is.True);
        }

        [Test]
        public void Match_WithNull_ReturnsTrue()
        {
            var definition = new SlotDefinition<object>();

            Assert.That(definition.Match(null), Is.True);
        }
    }
}
