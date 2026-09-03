using System;
using NUnit.Framework;

namespace Core.SlotsSystem.Tests.Editor
{
    internal sealed class FlagMatchTests
    {
        [Flags]
        private enum TestFlags
        {
            None = 0,
            First = 1 << 0,
            Second = 1 << 1,
            Third = 1 << 2
        }

        [Test]
        public void Any_WithMatchingFlag_ReturnsTrue()
        {
            TestFlags value = TestFlags.First | TestFlags.Second;

            Assert.That(FlagMatch.Any(value, TestFlags.Second), Is.True);
        }

        [Test]
        public void Any_WithoutMatchingFlag_ReturnsFalse()
        {
            TestFlags value = TestFlags.First | TestFlags.Second;

            Assert.That(FlagMatch.Any(value, TestFlags.Third), Is.False);
        }

        [Test]
        public void Any_WithNoRequiredFlags_ReturnsFalse()
        {
            Assert.That(FlagMatch.Any(TestFlags.First, TestFlags.None), Is.False);
        }

        [Test]
        public void All_WithAllRequiredFlags_ReturnsTrue()
        {
            TestFlags value = TestFlags.First | TestFlags.Second | TestFlags.Third;
            TestFlags required = TestFlags.First | TestFlags.Third;

            Assert.That(FlagMatch.All(value, required), Is.True);
        }

        [Test]
        public void All_WithMissingRequiredFlag_ReturnsFalse()
        {
            TestFlags value = TestFlags.First | TestFlags.Second;
            TestFlags required = TestFlags.First | TestFlags.Third;

            Assert.That(FlagMatch.All(value, required), Is.False);
        }

        [Test]
        public void All_WithNoRequiredFlags_ReturnsTrue()
        {
            Assert.That(FlagMatch.All(TestFlags.First, TestFlags.None), Is.True);
        }
    }
}
