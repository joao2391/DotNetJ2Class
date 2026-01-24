using DotNet.J2Class;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Diagnostics;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var resp = client.GetAsync("https://gorest.co.in/public/v2/users/8343295").Result.Content.ReadAsStringAsync().Result;
            var resp2 = client.GetAsync("https://gorest.co.in/public/v2/users").Result.Content.ReadAsStringAsync().Result;

            string json = @"{'Test':'TestValue', 'Test2':'TestValue2', 'Test3':'TestValue3'}";
            string json2 = @"{'TestProp': { 'TestF1': 'TestValue1', 'TestF2':'TestValue2'}, 'TestProp2': { 'TestP2' : 'TestValueP2'},'TestProp3': { 'TestP3' : 'TestValueP3'} }";
            
            var result = JsonConvert.DeserializeObject<object>(json);
            object result2 = JsonConvert.DeserializeObject<object>(json2);
            var tipo = result2.GetType();

            Console.WriteLine(result);
            Console.WriteLine(tipo.GetProperties());
           
            var myObject = J2Class.CreateObjectFromJson(resp, "TesteClass");
            dynamic dyn = myObject; 
            Console.WriteLine(dyn.email);
            
            var myObject2 = J2Class.CreateObjectFromComplexJson(json2, "TesteClass2");
            dynamic dyn2 = myObject2;
            //Console.WriteLine(dyn2.TestProp.TestF1);
            Console.WriteLine(dyn2);
            Console.ReadKey();
        }
    }

    
}
