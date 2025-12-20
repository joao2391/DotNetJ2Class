using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DotNet.J2Class
{
    static class StringNormalize
    {
        internal static IDictionary<string, object> ReturnKeyValueFromJson(string json)
        {
            var keyValue = new Dictionary<string, object>();

            if (string.IsNullOrWhiteSpace(json))
                return keyValue;

            try
            {
                var normalizedJson = json?.Replace('\'', '"') ?? json;
                var token = JToken.Parse(normalizedJson);

                if (token is JObject jObject)
                {
                    foreach (var prop in jObject.Properties())
                    {
                        var val = prop.Value;

                        if (val.Type == JTokenType.Object)
                        {
                            // keep nested objects as dictionaries when requested
                            keyValue.Add(prop.Name, ReturnKeyValueFromJson(val.ToString()));
                        }
                        else if (val.Type == JTokenType.Array)
                        {
                            var arr = (JArray)val;
                            if (arr.Count == 0)
                            {
                                keyValue.Add(prop.Name, new List<object>());
                            }
                            else
                            {
                                // detect homogeneous primitive arrays and materialize typed lists when possible
                                bool allValues = true;
                                var firstType = arr[0].Type;
                                foreach (var t in arr)
                                {
                                    if (t.Type != firstType)
                                    {
                                        allValues = false; break;
                                    }
                                }

                                if (allValues && (firstType == JTokenType.String || firstType == JTokenType.Integer || firstType == JTokenType.Float || firstType == JTokenType.Boolean))
                                {
                                    // materialize typed list
                                    if (firstType == JTokenType.String)
                                        keyValue.Add(prop.Name, arr.ToObject<List<string>>());
                                    else if (firstType == JTokenType.Integer)
                                        keyValue.Add(prop.Name, arr.ToObject<List<long>>());
                                    else if (firstType == JTokenType.Float)
                                        keyValue.Add(prop.Name, arr.ToObject<List<double>>());
                                    else if (firstType == JTokenType.Boolean)
                                        keyValue.Add(prop.Name, arr.ToObject<List<bool>>());
                                    else
                                        keyValue.Add(prop.Name, arr.ToObject<List<object>>());
                                }
                                else if (allValues && firstType == JTokenType.Object)
                                {
                                    var listObj = new List<IDictionary<string, object>>();
                                    foreach (var element in arr)
                                    {
                                        listObj.Add(ReturnKeyValueFromJson(element.ToString()));
                                    }
                                    keyValue.Add(prop.Name, listObj);
                                }
                                else
                                {
                                    keyValue.Add(prop.Name, arr.ToObject<List<object>>());
                                }
                            }
                        }
                        else
                        {
                            keyValue.Add(prop.Name, val.ToObject<object>());
                        }
                    }
                }
            }
            catch
            {
                // fall back to empty dictionary on parse errors (caller handles failures)
            }

            return keyValue;
        }


        internal static IDictionary<string, IDictionary<string, object>> ReturnKeyValueFromComplexJson(string json)
        {
            var keyValueObjDic = new Dictionary<string, IDictionary<string, object>>();

            if (string.IsNullOrWhiteSpace(json))
                return keyValueObjDic;

            try
            {
                var normalizedJson = json?.Replace('\'', '"') ?? json;
                var jObject = JObject.Parse(normalizedJson);

                foreach (var prop in jObject.Properties())
                {
                    var val = prop.Value;

                    if (val.Type == JTokenType.Object)
                    {
                        var inner = ReturnKeyValueFromJson(val.ToString());
                        keyValueObjDic.Add(prop.Name, inner);
                    }
                    else
                    {
                        var result = new Dictionary<string, object>();
                        result.Add(prop.Name, val.ToObject<object>());
                        keyValueObjDic.Add(prop.Name, result);
                    }
                }
            }
            catch
            {
                // return empty collection on parse errors
            }

            return keyValueObjDic;
        }
    }
}
