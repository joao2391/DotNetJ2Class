using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DotNet.J2Class
{
    /// <summary>
    /// Create an object at run time
    /// with properties extracted from
    /// JSON.
    /// </summary>
    public static class J2Class
    {
        /// <summary>
        /// Create an object from Json
        /// that is passed as first parameter
        /// at Runtime
        /// </summary>
        /// <param name="json">JSON to be transformed in a class</param>
        /// <param name="className">The object will be create with this class name</param>
        /// /// <param name="moduleName">The object will be create with this module name</param>
        /// <returns>Returns an object with properties and values extracts from JSON</returns>
        public static object CreateObjectFromJson(string json, string className = Constants.DEFAULT_CLASS_NAME, string moduleName = Constants.DEFAULT_MODULE_NAME)
        {
            try
            {
                //var keyValueTeste = StringNormalize.ReturnKeyValueFromComplexJson(json);

                var teste = CreateObjectFromComplexJson(json,className,moduleName);

                var keyValue = StringNormalize.ReturnKeyValueFromJson(json);

                Type myType = CompileResultType(keyValue, className, moduleName);

                object myObject = Activator.CreateInstance(myType);

                foreach (var item in keyValue)
                {
                    PropertyInfo info = myType.GetProperty(item.Key);

                    info.SetValue(myObject, item.Value);
                }

                return myObject;
            }
            catch (Exception ex)
            {
                //TODO Improve Exception's return
                throw ex;
            }
            
        }

        private static object CreateObjectFromComplexJson(string json, string className = Constants.DEFAULT_CLASS_NAME, string moduleName = Constants.DEFAULT_MODULE_NAME)
        {
            try
            {
                var keyValues = StringNormalize.ReturnKeyValueFromComplexJson(json);                

                Type myType = CompileResultTypeForComplexJson(keyValues, className, moduleName);

                object myObject = Activator.CreateInstance(myType);

                foreach (var item in keyValues)
                {
                    PropertyInfo info = myType.GetProperty(item.Key);

                    info.SetValue(myObject, item.Value);
                }

                return myObject;
            }
            catch (Exception ex)
            {
                //TODO Improve Exception's return
                throw ex;
            }
            
        }

        private static Type CompileResultTypeForComplexJson(IDictionary<string, IDictionary<string, object>> keyValues, string className, string moduleName)
        {
            TypeBuilder tb = GetTypeBuilderForComplexJson(className, moduleName);

            CreatePropertyForComplexJson(tb, keyValues);

            Type objectType = tb.CreateTypeInfo();
            
            return objectType;
        }

        private static TypeBuilder GetTypeBuilderForComplexJson(string className, string moduleName)
        {   
            //!Define o nome do Assembly  
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

        private static void CreatePropertyForComplexJson(TypeBuilder tb, IDictionary<string, IDictionary<string, object>> keyValues)//string propertyName, Type propertyType)
        {

            //var lstProps = new List<PropertyBuilder>();
            //var lstClasses = new List<TypeBuilder>();
            //var lstFields = new List<FieldBuilder>();
            //var lstGetMethods = new List<MethodBuilder>();
            //var lstSetMethods = new List<MethodBuilder>();

            //TypeBuilder childBuilder = null;
            //PropertyBuilder propBuild = null;

            foreach (var item in keyValues)
            {
                CreateProperty(tb, item.Key, item.Value.GetType());

                // childBuilder = tb.DefineNestedType(item.Key, TypeAttributes.NestedPublic);

                // lstClasses.Add(childBuilder); 

                // foreach (var item2 in item.Value.Keys)
                // {                 
                //    lstProps.Add(childBuilder.DefineProperty(item2, PropertyAttributes.HasDefault, item2.GetType(), null));
                //    propBuild = tb.DefineProperty(item2, PropertyAttributes.HasDefault, item2.GetType(), null);
                //    var testebld = tb.DefineProperty(item2, PropertyAttributes.HasDefault, childBuilder, null);
                // }
                
            }

            //foreach (var item in lstClasses)
            //{
                // lstFields.Add(tb.DefineField(item.Name, item, FieldAttributes.Private));
                // lstGetMethods.Add(tb.DefineMethod(string.Concat("get_",item.Name),
                //                                 MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                //                                 item,
                //                                 Type.EmptyTypes));
                // lstSetMethods.Add(tb.DefineMethod(string.Concat("set_",item.Name),
                //                                 MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                //                                 null, new Type[] { childBuilder }));



                // var setMethods = tb.DefineMethod(string.Concat("set_",item.Name),
                //                                  MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                //                                  null, new Type[] { childBuilder });

                // var getMethods = tb.DefineMethod(string.Concat("get_",item.Name),
                //                                 MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                //                                 childBuilder,
                //                                 Type.EmptyTypes);
                
                // var fieldBuilder = tb.DefineField(item.Name, childBuilder, FieldAttributes.Private);
                // ILGenerator getterIL = getMethods.GetILGenerator();
                // getterIL.Emit(OpCodes.Ldarg_0);
                // getterIL.Emit(OpCodes.Ldfld, fieldBuilder);
                // getterIL.Emit(OpCodes.Ret);

                // ILGenerator setterIL = setMethods.GetILGenerator();
                // // setterIL.Emit(OpCodes.Ldarg_0);
                // // setterIL.Emit(OpCodes.Ldarg_1);
                // // setterIL.Emit(OpCodes.Stfld, fieldBuilder);
                // // setterIL.Emit(OpCodes.Ret);
                // Label modifyProperty = setterIL.DefineLabel();
                // Label exitSet = setterIL.DefineLabel();

                // setterIL.MarkLabel(modifyProperty);
                // setterIL.Emit(OpCodes.Ldarg_0);
                // setterIL.Emit(OpCodes.Ldarg_1);
                // setterIL.Emit(OpCodes.Stfld, fieldBuilder);

                // setterIL.Emit(OpCodes.Nop);
                // setterIL.MarkLabel(exitSet);
                // setterIL.Emit(OpCodes.Ret);

                // propBuild.SetGetMethod(getMethods);
                // propBuild.SetSetMethod(setMethods);
            //}
                      
            
        }

         private static Type CompileResultType(IDictionary<string, object> keyValue, string className, string moduleName)
        {
            TypeBuilder tb = GetTypeBuilder(className, moduleName);
            //ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
          
            foreach (var field in keyValue)
            {
                CreateProperty(tb, field.Key, field.Value.GetType());
            }
                
            
            Type objectType = tb.CreateTypeInfo();
            
            return objectType;
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
    }
}
