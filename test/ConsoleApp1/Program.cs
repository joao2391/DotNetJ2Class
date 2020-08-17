using DotNet.J2Class;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string json = @"{'Test':'TestValue', 'Test2':'TestValue2', 'Test3':'TestValue3'}";
           
            var myObject = J2Class.CreateObjectFromJson(json, "TesteClass", "");

            Console.ReadKey();
        }
    }
}
