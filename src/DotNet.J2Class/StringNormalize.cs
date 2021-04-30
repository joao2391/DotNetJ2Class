﻿using System;
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

            var jsonWithInfoFormatted = new string[newjson.Length * 2];

            Dictionary<string, object> keyValue = new Dictionary<string, object>();

            for (int i = 0; i < newjson.Length; i++)
            {
                if (jsonWithInfoFormatted[i] == null)
                {
                    newjson[i].Split(':').CopyTo(jsonWithInfoFormatted, i);
                }
                else
                {
                    newjson[i].Split(':').CopyTo(jsonWithInfoFormatted, i * 2);
                }

            }

            for (int i = 0; i < jsonWithInfoFormatted.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var removeChave = jsonWithInfoFormatted[i].Trim().Remove(0, 1);
                    removeChave = removeChave.Remove(removeChave.Length - 1);

                    var removeValor = jsonWithInfoFormatted[i + 1].Remove(0, 1);
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
            Dictionary<string, IDictionary<string, object>> keyValueObjDic = new Dictionary<string, IDictionary<string, object>>();

            var jObject = JObject.Parse(json);

            foreach (var item in jObject)
            {
                var result = ReturnKeyValueFromJson(item.Value.ToString());
                keyValueObjDic.Add(item.Key, result);
            }

            var resultCollection = keyValueObjDic.Count > 0 ? keyValueObjDic : new Dictionary<string, IDictionary<string, object>>();

            return resultCollection;

        }
    }
}
