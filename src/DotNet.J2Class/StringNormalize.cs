using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using System.Linq;
using LitJson;

namespace DotNet.J2Class
{
    static class StringNormalize
    {
        internal static IDictionary<string, object> ReturnKeyValueFromJson(string json)
        {
            var removed = json.Remove(0, 1);
            removed = removed.Remove(removed.Length - 1);

            string[] newjson = removed.Split(',');

            var newjson3 = new string[newjson.Length * 2];

            Dictionary<string, object> keyValue = new Dictionary<string, object>();

            for (int i = 0; i < newjson.Length; i++)
            {
                if (newjson3[i] == null)
                {
                    newjson[i].Split(':').CopyTo(newjson3, i);
                }
                else
                {
                    newjson[i].Split(':').CopyTo(newjson3, i * 2);
                }

            }

            for (int i = 0; i < newjson3.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var removeChave = newjson3[i].Trim().Remove(0, 1);
                    removeChave = removeChave.Remove(removeChave.Length - 1);

                    var removeValor = newjson3[i + 1].Remove(0, 1);
                    removeValor = removeValor.Remove(removeValor.Length - 1);

                    keyValue.Add(removeChave, removeValor);
                }
            }

            var keyValueResult = keyValue.Count > 0 ? keyValue : new Dictionary<string, object>();

            return keyValueResult;

        }


        internal static IDictionary<string, IDictionary<string, object>> ReturnKeyValueFromComplexJson(string json)
        {          
            
            Dictionary<string, string[]> keyValues = new Dictionary<string, string[]>();
            Dictionary<string, IDictionary<string, object>> dicViga = new Dictionary<string, IDictionary<string, object>>();

            var json2 = JObject.Parse(json);

            foreach (var item in json2)
            {
                var result = ReturnKeyValueFromJson(item.Value.ToString());
                dicViga.Add(item.Key, result);
            }

            foreach (var item3 in dicViga)
            {
                var sas = item3.Value;
            }

            var resultCollection = dicViga.Count > 0 ? dicViga : new Dictionary<string, IDictionary<string, object>>();

            return resultCollection;

        }
    }
}
