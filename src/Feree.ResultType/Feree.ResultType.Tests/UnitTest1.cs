using Feree.ResultType.Core;
using NUnit.Framework;

namespace Feree.ResultType.Tests
{
    public class UnitTest1
    {
        [Test]
        public void Success_GivenObject_ShouldKeepItAsPayload()
        {
            var success = ResultFactory.CreateSuccess("string");

            Assert.That(success.Payload, Is.EqualTo("string"));
        }
        
        [Test]
        public void Bind_GivenSuccess_CallsNextFunction()
        {
            var success = ResultFactory.CreateSuccess();

            var result = success.Bind(() => ResultFactory.CreateSuccess(5));

            Assert.That(((Success<int>) result).Payload, Is.EqualTo(5));
        }
        
        [Test]
        public void Bind_GivenFailure_ReturnsFailure()
        {
            var success = ResultFactory.CreateFailure("error message");

            var result = success.Bind(() => ResultFactory.CreateSuccess(5));

            Assert.That(result is Failure<int>, Is.True);
            Assert.That(((Failure<int>) result).Error.Message, Is.EqualTo("error message"));
        }
        
        [Test]
        public void Bind_GivenSuccess_CallsNextFunctionWithResultOfSource()
        {
            var success = ResultFactory.CreateSuccess("string");

            var result = success.Bind(s => ResultFactory.CreateSuccess(s.Length));

            Assert.That(((Success<int>) result).Payload, Is.EqualTo(6));
        }
    }
}