using DotNet.J2Class;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var resp = client.GetAsync("https://gorest.co.in/public/v2/users/1").Result.Content.ReadAsStringAsync().Result;

            string json = @"{'Test':'TestValue', 'Test2':'TestValue2', 'Test3':'TestValue3'}";
            string json2 = @"{'TestProp': { 'Test': 'TestValue'} }";
            
            var result = JsonConvert.DeserializeObject<object>(json);
            object result2 = JsonConvert.DeserializeObject<object>(json2);
            var tipo = result2.GetType();

            Console.WriteLine(result);
            Console.WriteLine(tipo.GetProperties());
           
            var myObject = J2Class.CreateObjectFromComplexJson(resp, "TesteClass");

            Console.ReadKey();
        }
    }

    
}
