using System;
using NUnit.Framework;
using DotNet.J2Class;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

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
        const string JSON_ARRAY = @"{'Items': ['A','B','C']}";
        const string JSON_EMPTY = @"{}";
        const string JSON_NULL_VALUE = @"{'Maybe': null}";
        const string JSON_MIXED_LIST = @"{'Mixed': ['A', 1, null]}";

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

        [Test]
        public void Should_Handle_Array_Property()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_ARRAY, "ArrayClass", "ArrayModule");

            PropertyInfo propInfo = obj.GetType().GetProperty("Items");

            Assert.IsNotNull(propInfo);

            var val = propInfo.GetValue(obj) as System.Collections.IEnumerable;
            Assert.IsNotNull(val);
            int count = 0;
            foreach (var _ in val) count++;
            Assert.AreEqual(3, count);
        }

        [Test]
        public void Should_Handle_Empty_Json()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_EMPTY, "EmptyClass", "EmptyModule");

            Assert.IsNotNull(obj);
        }

        [Test]
        public void Should_Handle_Null_Value()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_NULL_VALUE, "NullClass", "NullModule");

            PropertyInfo propInfo = obj.GetType().GetProperty("Maybe");
            Assert.IsNotNull(propInfo);
        }

        [Test]
        public void Should_Handle_Mixed_List()
        {
            var obj = J2Class.CreateObjectFromJson(JSON_MIXED_LIST, "MixedClass", "MixedModule");

            PropertyInfo propInfo = obj.GetType().GetProperty("Mixed");
            Assert.IsNotNull(propInfo);

            var val = propInfo.GetValue(obj) as System.Collections.IEnumerable;
            Assert.IsNotNull(val);
        }

        [Test]
        public void Concurrent_Generation_Returns_Same_Type()
        {
            const int concurrency = 50;
            const string json = "{'A':'1','B':'2','C':{'X':'x'}}";

            var tasks = new Task<Type>[concurrency];

            for (int i = 0; i < concurrency; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var obj = J2Class.CreateObjectFromJson(json, "ConcClass", "ConcModule");
                    return obj.GetType();
                });
            }

            Task.WaitAll(tasks);

            var types = tasks.Select(t => t.Result).ToArray();
            Assert.IsTrue(types.All(t => t == types[0]));
        }

        [Test]
        public void Repeated_Generation_Performance_Simple()
        {
            const int iterations = 200;
            const string json = "{'P1':'v1','P2':'v2','P3':'v3'}";

            // warm
            var first = J2Class.CreateObjectFromJson(json, "PerfClass", "PerfModule");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var obj = J2Class.CreateObjectFromJson(json, "PerfClass", "PerfModule");
                Assert.IsNotNull(obj);
            }
            sw.Stop();

            // Loose bound: repeated generation should finish quickly with caching
            Assert.Less(sw.Elapsed.TotalSeconds, 10, $"Repeated generation took too long: {sw.Elapsed}");
        }







    }
}