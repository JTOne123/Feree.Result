using System.Threading.Tasks;
using Feree.ResultType.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace Feree.ResultType.Tests
{
    [TestFixture]
    public class BindTests
    {
        [Test]
        public void Bind_GivenSuccess_CallsNextFunction()
        {
            var mock = new Mock<TestClass>();
            var success = ResultFactory.CreateSuccess();

            var result = success.Bind(() => mock.Object.TestMethod());

            mock.Verify(a => a.TestMethod(), Times.Once);
        }

        [Test]
        public void Bind_GivenSuccess_CallsNextFunctionWithResultOfSource()
        {
            var mock = new Mock<TestClass>();
            var success = ResultFactory.CreateSuccess(5);

            var result = success.Bind(mock.Object.TestMethod<int>);

            mock.Verify(a => a.TestMethod(5), Times.Once);
        }

        [Test]
        public void Bind_GivenSuccess_ReturnsResultOfNextFunction()
        {
            var mock = new Mock<TestClass>();
            mock.Setup(a => a.TestMethod(It.IsAny<int>())).Returns(123.AsResult());
            var success = ResultFactory.CreateSuccess();

            var result = success.Bind(() => mock.Object.TestMethod(5));

            Assert.That(result.AsSuccess().Payload, Is.EqualTo(123));
        }

        [Test]
        public void Bind_GivenSuccessAndNextFunctionFailure_ReturnsFailure()
        {
            var success = ResultFactory.CreateSuccess();
            var failure = ResultFactory.CreateFailure("error");

            var result = success.Bind(() => failure);

            Assert.That(result is Failure, Is.True);
        }

        [Test]
        public void Bind_GivenFailure_ReturnsFailure()
        {
            var failure = ResultFactory.CreateFailure("error message");
            var success = ResultFactory.CreateSuccess(5);

            var result = failure.Bind(() => success);

            Assert.That(result is Failure<int>, Is.True);
        }

        [Test]
        public void Bind_GivenFailure_DoesNotCallNextFunction()
        {
            var mock = new Mock<TestClass>();
            var success = ResultFactory.CreateFailure("error message");

            var result = success.Bind(() => mock.Object.TestMethod());

            mock.Verify(a => a.TestMethod(), Times.Never);
        }

        [Test]
        public void BindAsync_GivenTaskOfSuccess_CallsNextFunction()
        {
            var mock = new Mock<TestClass>();
            var success = new TaskFactory().StartNew(() => 123).AsResultAsync();

            var result = success.BindAsync(() => mock.Object.TestMethod());

            mock.Verify(a => a.TestMethod(), Times.Once);
        }

        [Test]
        public void BindAsync_GivenTaskOfSuccess_CallsNextFunctionThatReturnsTask()
        {
            var mock = new Mock<TestClass>();
            var success = new TaskFactory().StartNew(() => 123).AsResultAsync();

            var result = success.BindAsync(() => mock.Object.TestMethodAsync());

            mock.Verify(a => a.TestMethodAsync(), Times.Once);
        }

        [Test]
        public void BindAsync_GivenSuccess_CallsNextFunctionThatReturnsTask()
        {
            var mock = new Mock<TestClass>();
            var success = 123.AsResult();

            var result = success.BindAsync(() => mock.Object.TestMethodAsync());

            mock.Verify(a => a.TestMethodAsync(), Times.Once);
        }

        [Test]
        public async Task BindAsync_GivenTaskOfFailure_DoesNotCallNextFunction()
        {
            var mock = new Mock<TestClass>();
            var failure = new TaskFactory().StartNew(() => ResultFactory.CreateFailure("error"));

            var result = await failure.BindAsync(() => mock.Object.TestMethodAsync());

            mock.Verify(a => a.TestMethodAsync(), Times.Never);
        }

        [Test]
        public void BindAsync_GivenTaskOfFailure_DoesNotCallFunctionThatReturnsTask()
        {
            var mock = new Mock<TestClass>();
            var failure = new TaskFactory().StartNew(() => ResultFactory.CreateFailure("error"));

            var result = failure.BindAsync(() => mock.Object.TestMethodAsync());

            mock.Verify(a => a.TestMethodAsync(), Times.Never);
        }

        [Test]
        public void BindAsync_GivenFailure_DoesNotCallFunctionThatReturnsTask()
        {
            var mock = new Mock<TestClass>();
            var failure = ResultFactory.CreateFailure("error");

            var result = failure.BindAsync(() => mock.Object.TestMethodAsync());

            mock.Verify(a => a.TestMethodAsync(), Times.Never);
        }
    }
}