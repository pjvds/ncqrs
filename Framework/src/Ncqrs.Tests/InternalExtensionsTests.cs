using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace Ncqrs.Tests
{
    [TestFixture]
    public class InternalExtensionsTests
    {
        [Test]
        public void IsEmpty_should_return_true_on_empty_array()
        {
            var target = new object[0];
            var isEmpty = InternalExtensions.IsEmpty(target);

            Assert.IsTrue(isEmpty);
        }

        [Test]
        public void IsEmpty_should_return_true_on_empty_list()
        {
            var target = new List<object>(0);
            var isEmpty = InternalExtensions.IsEmpty(target);

            Assert.IsTrue(isEmpty);
        }

        [Test]
        public void IsEmpty_should_return_false_on_non_empty_array()
        {
            var target = new object[1];
            var isEmpty = InternalExtensions.IsEmpty(target);

            Assert.IsFalse(isEmpty);
        }

        [Test]
        public void IsEmpty_should_return_false_on_non_empty_list()
        {
            var target = new List<object>();
            target.Add(new object());
            var isEmpty = InternalExtensions.IsEmpty(target);

            Assert.IsFalse(isEmpty);
        }

        [Test]
        public void IsEmpty_should_throw_ArgumentNullException_on_null_reference()
        {
            List<object> target = null;
            Action act = ()=>InternalExtensions.IsEmpty(target);

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [Test]
        public void IsNullOrEmpty_should_return_true_on_null()
        {
            var result = InternalExtensions.IsNullOrEmpty(null);
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmpty_should_return_true_on_empty()
        {
            var result = InternalExtensions.IsNullOrEmpty(string.Empty);
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmpty_should_return_false_on_string_with_text()
        {
            var result = InternalExtensions.IsNullOrEmpty("Hello world");
            result.Should().BeFalse();
        }

        [Test]
        public void Clone_should_return_other_instance()
        {
            var source = new List<object>();
            var clone = InternalExtensions.Clone(source);

            Assert.AreNotSame(source, clone);
        }

        [Test]
        public void Clone_should_return_list_with_same_values()
        {
            var source = new List<int>{1,2,3,4,5};
            var clone = InternalExtensions.Clone(source);

            clone.Should().ContainInOrder(source);
        }

        [Test]
        public void Clone_should_throw_ArgumentNullException_when_target_is_null()
        {
            List<string> nullTarget = null;
            Action act = ()=> InternalExtensions.Clone(nullTarget);
            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("source");
        }
    }
}
