using Autofac.Extras.Moq;
using NUnit.Framework;
using Project.Services.Values;
using System.Collections.Generic;

namespace Project.Services.NUnitTest
{
    [TestFixture]
    public class ValuesServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Get()
        {
            using var moc = AutoMock.GetStrict();
            var expectedResult = new List<string> { "Value1", "Value2" };

            var mockValueService = moc.Mock<IValuesService>();
            mockValueService.Setup(x => x.Get()).Returns(expectedResult);

            var valueService = moc.Create<IValuesService>();
            var result = valueService.Get();

            moc.Mock<IValuesService>().Verify(x => x.Get());
            Assert.AreEqual(expectedResult, result);
            //Assert.Pass();
        }
    }
}