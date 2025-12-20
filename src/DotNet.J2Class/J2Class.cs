using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace DotNet.J2Class
{
    /// <summary>
    /// Create an object at run time
    /// with properties extracted from
    /// JSON.
    /// </summary>
    public static class J2Class
    {
        private static readonly ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        private static string GetSchemaSignature(IDictionary<string, object> dict)
        {
            var sb = new StringBuilder();
            foreach (var kv in dict)
            {
                sb.Append(kv.Key).Append(':');
                if (kv.Value is IDictionary<string, object> nested)
                {
                    sb.Append('{').Append(GetSchemaSignature(nested)).Append('}');
                }
                else if (kv.Value is IList<object> list)
                {
                    if (list.Count > 0)
                    {
                        var first = list[0];
                        if (first is IDictionary<string, object> nestedListDict)
                            sb.Append("List<{" + GetSchemaSignature(nestedListDict) + "}>");
                        else
                            sb.Append("List<" + (first?.GetType().Name ?? "object") + ">");
                    }
                    else
                    {
                        sb.Append("List<object>");
                    }
                }
                else
                {
                    sb.Append(kv.Value?.GetType().Name ?? "object");
                }
                sb.Append(';');
            }
            return sb.ToString();
        }

        private static string GetComplexSchemaSignature(IDictionary<string, IDictionary<string, object>> dict)
        {
            var sb = new StringBuilder();
            foreach (var kv in dict)
            {
                sb.Append(kv.Key).Append(':').Append('{').Append(GetSchemaSignature(kv.Value)).Append('}').Append(';');
            }
            return sb.ToString();
        }
        /// <summary>
        /// Create an object from Json
        /// that is passed as first parameter
        /// at Runtime
        /// </summary>
        /// <param name="json">JSON to be transformed in a class</param>
        /// <param name="className">The object will be create with this class name</param>
        /// <param name="moduleName">The object will be create with this module name</param>
        /// <returns>Returns an object with properties and values extracts from JSON</returns>
        public static object CreateObjectFromJson(string json, string className = Constants.DEFAULT_CLASS_NAME, string moduleName = Constants.DEFAULT_MODULE_NAME)
        {
            try
            {
                var keyValue = StringNormalize.ReturnKeyValueFromJson(json);

                Type myType = CompileResultType(keyValue, className, moduleName);

                object myObject = Activator.CreateInstance(myType);

                foreach (var item in keyValue)
                {
                    PropertyInfo info = myType.GetProperty(item.Key);

                    var valueToSet = item.Value;

                    if (item.Value is IDictionary<string, object> dictValue)
                    {
                        var propType = info.PropertyType;
                        valueToSet = BuildObjectFromDict(dictValue, propType, item.Key, className, moduleName);
                    }
                    else if (item.Value is IList<object> listValue)
                    {
                        var propType = info.PropertyType;
                        if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var elemType = propType.GetGenericArguments()[0];
                            var listInstance = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
                            foreach (var el in listValue)
                            {
                                if (el is IDictionary<string, object> nestedElDict)
                                {
                                    listInstance.Add(BuildObjectFromDict(nestedElDict, elemType, item.Key + "Item", className, moduleName));
                                }
                                else
                                {
                                    listInstance.Add(el);
                                }
                            }
                            valueToSet = listInstance;
                        }
                    }

                    info.SetValue(myObject, valueToSet);
                }

                return (dynamic)myObject;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create object from JSON.", ex);
            }
            
        }

        /// <summary>
        /// Create an object from ComplexJson
        /// that is passed as first parameter
        /// at Runtime
        /// </summary>
        /// <param name="json">JSON to be transformed in a class</param>
        /// <param name="className">The object will be create with this class name</param>
        /// <param name="moduleName">The object will be create with this module name</param>
        /// <returns>Returns an object with properties and values extracts from JSON</returns>
        public static object CreateObjectFromComplexJson(string json, string className = Constants.DEFAULT_CLASS_NAME, string moduleName = Constants.DEFAULT_MODULE_NAME)
        {
            try
            {
                var keyValues = StringNormalize.ReturnKeyValueFromComplexJson(json);                

                Type myType = CompileResultTypeForComplexJson(keyValues, className, moduleName);

                object myObject = Activator.CreateInstance(myType);

                foreach (var item in keyValues)
                {
                    PropertyInfo info = myType.GetProperty(item.Key);

                    var dict = item.Value;
                    var valueToSet = (object)null;
                    if (dict is IDictionary<string, object> simpleDict)
                    {
                        valueToSet = BuildObjectFromDict(simpleDict, info.PropertyType, item.Key, className, moduleName);
                    }
                    else
                    {
                        valueToSet = dict;
                    }

                    info.SetValue(myObject, valueToSet);
                }

                return (dynamic)myObject;
            }
            catch (Exception)
            {
                //TODO Improve Exception's return
                throw;
            }
            
        }

        private static Type CompileResultTypeForComplexJson(IDictionary<string, IDictionary<string, object>> keyValues, string className, string moduleName)
        {
            var signature = GetComplexSchemaSignature(keyValues);
            var cacheKey = string.Concat(className, "|", moduleName, "|", signature);

            return _typeCache.GetOrAdd(cacheKey, _ =>
            {
                TypeBuilder tb = GetTypeBuilderForComplexJson(className, moduleName);

                CreatePropertyForComplexJson(tb, keyValues, className, moduleName);

                var created = tb.CreateTypeInfo().AsType();
                return created;
            });
        }

        private static TypeBuilder GetTypeBuilderForComplexJson(string className, string moduleName)
        {               
            var an = new AssemblyName(className);

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            TypeBuilder tb = moduleBuilder.DefineType(className,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;            
        }

        private static void CreatePropertyForComplexJson(TypeBuilder tb, IDictionary<string, IDictionary<string, object>> keyValues, string className, string moduleName)
        {
            foreach (var item in keyValues)
            {
                // Compile a nested generated type for each complex property so callers get a proper POCO
                Type propType = CompileResultType(item.Value, $"{className}_{item.Key}", moduleName);
                CreateProperty(tb, item.Key, propType);
            }

        }

         private static Type CompileResultType(IDictionary<string, object> keyValue, string className, string moduleName)
        {
            var signature = GetSchemaSignature(keyValue);
            var cacheKey = string.Concat(className, "|", moduleName, "|", signature);

            return _typeCache.GetOrAdd(cacheKey, _ =>
            {
                TypeBuilder tb = GetTypeBuilder(className, moduleName);

                foreach (var field in keyValue)
                {
                    CreateProperty(tb, field.Key, GetPropertyType(field.Value, field.Key, className, moduleName));
                }

                var created = tb.CreateTypeInfo().AsType();
                return created;
            });
        }

        private static TypeBuilder GetTypeBuilder(string className, string moduleName)
        {            
            var an = new AssemblyName(className);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            TypeBuilder tb = moduleBuilder.DefineType(className,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);            

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);

            ILGenerator getIl = getPropMthdBldr.GetILGenerator();
            
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
            
            
        }

        private static Type GetPropertyType(object value, string propertyName, string className, string moduleName)
        {
            if (value is IDictionary<string, object> nestedDict)
            {
                return CompileResultType(nestedDict, $"{className}_{propertyName}", moduleName);
            }
            if (value is IList<object> list)
            {
                if (list.Count == 0)
                    return typeof(List<object>);

                // find first non-null element to infer type
                object firstNonNull = null;
                foreach (var el in list) { if (el != null) { firstNonNull = el; break; } }

                if (firstNonNull == null)
                    return typeof(List<object>);

                var firstType = firstNonNull.GetType();

                bool homogeneous = true;
                foreach (var el in list)
                {
                    if (el == null) continue;
                    if (el.GetType() != firstType) { homogeneous = false; break; }
                }

                if (!homogeneous)
                    return typeof(List<object>);

                if (firstNonNull is IDictionary<string, object>)
                {
                    var elementType = GetPropertyType(firstNonNull, propertyName + "Item", className, moduleName);
                    return typeof(List<>).MakeGenericType(elementType);
                }

                return typeof(List<>).MakeGenericType(firstType);
            }
            return value?.GetType() ?? typeof(object);
        }

        private static object BuildObjectFromDict(IDictionary<string, object> dict, Type targetType, string propertyName, string className, string moduleName)
        {
            if (dict == null)
                return null;

            // If targetType is object, compile a concrete type for this dictionary
            Type instType = targetType;
            if (instType == typeof(object))
            {
                instType = CompileResultType(dict, $"{className}_{propertyName}", moduleName);
            }

            var instance = Activator.CreateInstance(instType);

            foreach (var kv in dict)
            {
                var pi = instType.GetProperty(kv.Key);
                if (pi == null) continue;

                var val = kv.Value;
                if (val is IDictionary<string, object> nestedDict)
                {
                    var nested = BuildObjectFromDict(nestedDict, pi.PropertyType, kv.Key, className, moduleName);
                    pi.SetValue(instance, nested);
                }
                else if (val is IList<object> listVal)
                {
                    if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elemType = pi.PropertyType.GetGenericArguments()[0];
                        var listInstance = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elemType));
                        foreach (var el in listVal)
                        {
                            if (el is IDictionary<string, object> elDict)
                            {
                                listInstance.Add(BuildObjectFromDict(elDict, elemType, kv.Key + "Item", className, moduleName));
                            }
                            else
                            {
                                listInstance.Add(el);
                            }
                        }
                        pi.SetValue(instance, listInstance);
                    }
                }
                else
                {
                    pi.SetValue(instance, val);
                }
            }

            return instance;
        }
    }
}
