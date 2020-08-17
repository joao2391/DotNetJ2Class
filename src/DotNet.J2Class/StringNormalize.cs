using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.J2Class
{
    static class StringNormalize
    {
        internal static IDictionary<string, string> ReturnKeyValueFromJson(string json)
        {
            var removed = json.Remove(0, 1);
            removed = removed.Remove(removed.Length - 1);

            string[] newjson = removed.Split(',');

            var newjson3 = new string[newjson.Length * 2];

            Dictionary<string, string> keyValue = new Dictionary<string, string>();

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

            var keyValueResult = keyValue.Count > 0 ? keyValue : new Dictionary<string, string>();

            return keyValueResult;

        }
    }
}
