using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Ncqrs.Tests
{
    [TestFixture]
    class GuidCombGeneratorTests
    {
        [Test]
        public void Calling_generate_multiple_times_should_return_unique_results()
        {
            int count = 1000000;
            var generator = new GuidCombGenerator();

            var results = new List<Guid>();

            for (int i = 0; i < count; i++)
            {
                var id = generator.GenerateNewId();
                results.Add(id);
            }

            results.Should().OnlyHaveUniqueItems();
        }
    }
}
