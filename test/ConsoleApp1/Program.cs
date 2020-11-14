using DotNet.J2Class;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string json = @"{'Test':'TestValue', 'Test2':'TestValue2', 'Test3':'TestValue3'}";
            string json2 = @"{'TestProp': { 'Test': 'TestValue'} }";
            
            var result = JsonConvert.DeserializeObject<object>(json);
            object result2 = JsonConvert.DeserializeObject<object>(json2);
            var tipo = result2.GetType();

            Console.WriteLine(result);
            Console.WriteLine(tipo.GetProperties());
           
            var myObject = J2Class.CreateObjectFromJson(json, "TesteClass", "");

            Console.ReadKey();
        }
    }

    
}
