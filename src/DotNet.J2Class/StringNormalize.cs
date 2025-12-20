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
                var token = JToken.Parse(json);

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
                            keyValue.Add(prop.Name, val.ToObject<List<object>>());
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
                var jObject = JObject.Parse(json);

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
