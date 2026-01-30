using NUnit.Framework;

namespace NUnitExample
{
    [TestFixture]
    public class ExampleTest
    {
        [OneTimeSetUp]
        // This method will run once before the test run, if included in a SetUpFixture class
        // Or it will run once before the test class, if included in a TestFixture class
        public void BeforeClass()
        {
        }


        [SetUp]
        // This method will execute before each test in the class
        public void BeforeTest()
        {
        }


        [Test]
        // This is the test method
        public void TestMethod()
        {
        }


        [TearDown]
        // This method will execute after each test in the class
        public void AfterTest()
        {
        }


        [OneTimeTearDown]
        // This method will run once after the test run, if included in a SetUpFixture class
        // Or it will run once after the test class, if included in a TestFixture class
        public void AfterClass()
        {
        }
    }
}