using NUnit.Framework;
using DotNet.J2Class;
using System.Reflection;

namespace DotNet.J2Class.Tests
{
    public class J2ClassTests
    {

        const string JSON_ONE_VALUE = @"{'Test1':'TestValue1'}";
        const string JSON_TWO_VALUES = @"{'Test1':'TestValue1', 'Test2':'TestValue2'}";
        const string JSON_THREE_VALUES = @"{'Test1':'TestValue1', 'Test2':'TestValue2', 'Test3':'TestValue3'}";
        const string JSON_FOUR_VALUES = @"{'Test1':'TestValue1', 'Test2':'TestValue2', 'Test3':'TestValue3', 'Test4': 'TestValue4'}";
        const string JSON_FIVE_VALUES = @"{'Test1':'TestValue1', 'Test2':'TestValue2', 'Test3':'TestValue3', 'Test4': 'TestValue4', 'Test5':'TestValue5'}";
        const string JSON_SIX_VALUES = @"{'Test1':'TestValue1', 'Test2':'TestValue2', 'Test3':'TestValue3', 'Test4': 'TestValue4', 'Test5':'TestValue5', 'Test6':'TestValue6'}";
        const string COMPLEX_JSON_TWO_VALUES = @"{'TestProp': { 'TestF1': 'TestValue1', 'TestF2':'TestValue2'}, 'TestProp2': { 'TestP2' : 'TestValueP2'} }";
        const string COMPLEX_JSON_THREE_VALUES = @"{'TestProp': { 'TestF1': 'TestValue1', 'TestF2':'TestValue2'}, 'TestProp2': { 'TestP2' : 'TestValueP2'},'TestProp3': { 'TestP3' : 'TestValueP3'} }";
        const string SIMPLE_PLUS_COMPLEX_JSON_VALUES = @"{'Test1': 'TestValue1', 'TestProp2': { 'TestP2' : 'TestValueP2'} }";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Return_An_Object_With_One_Property_And_Value()
        {
            object obj = J2Class.CreateObjectFromJson(JSON_ONE_VALUE, "TestClass", "TestModule");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test1");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Two_Properties_And_Values()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_TWO_VALUES, "TestClass2", "TestModule2");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test2");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Three_Properties_And_Values()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_THREE_VALUES, "TestClass3", "TestModule3");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test3");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Four_Properties_And_Values()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_FOUR_VALUES, "TestClass4", "TestModule4");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test4");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Five_Properties_And_Values()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_FIVE_VALUES, "TestClass5", "TestModule5");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test5");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Six_Properties_And_Values()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_SIX_VALUES, "TestClass6", "TestModule6");

            PropertyInfo propInfo = obj.GetType().GetProperty("Test6");

            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Return_An_Object_With_Two_ValuePairs()
        {
            var obj = J2Class.CreateObjectFromComplexJson(COMPLEX_JSON_TWO_VALUES, "TestComplexClass","TestComplexModule");

            Assert.IsNotNull(obj);            

        }

        [Test]
        public void Should_Return_An_Object_With_Three_ValuePairs()
        {
            var obj = J2Class.CreateObjectFromComplexJson(COMPLEX_JSON_THREE_VALUES, "TestComplexClass","TestComplexModule");

            Assert.IsNotNull(obj);

        }

        [Test]
        public void Should_Return_An_Object()
        {
            var obj = J2Class.CreateObjectFromComplexJson(SIMPLE_PLUS_COMPLEX_JSON_VALUES, "TestComplexClass","TestComplexModule");

            Assert.IsNotNull(obj);            

        }







    }
}