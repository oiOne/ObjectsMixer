﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectsMixer.UnitTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace ObjectsMixer.UnitTests
{

    public class ObjectsMixerTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ObjectsMapperService _svc;
        public ObjectsMixerTests(ITestOutputHelper output)
        {
            _output = output;
            _svc = new ObjectsMapperService();
        }   

        [Fact]
        public void Mix_Returns_Left_As_Default_Comparison_With_Additional_Prop_Left()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1,
                AdditionalLeft = "Left"
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2
            };

            dynamic result = Mixer.WithLeftPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
        }

        [Fact]
        public void Mix_Returns_Right_As_Default_Comparison_With_Additional_Prop_Left()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1,
                AdditionalLeft = "Left"
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2
            };

            dynamic result = Mixer.WithRightPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
        }

        [Fact]
        public void Mix_Returns_Left_As_Default_Comparison_With_Additional_Prop_Right()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2,
                AdditionalRight = "Right"
            };

            dynamic result = Mixer.WithLeftPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_Returns_Right_As_Default_Comparison_With_Additional_Prop_Right()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2,
                AdditionalRight = "Right"
            };

            dynamic result = Mixer.WithRightPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_Returns_Left_As_Default_Comparison_With_Additional_Props_Both_Sides()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1,
                AdditionalLeft = "Left"
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2,
                AdditionalRight = "Right"
            };

            dynamic result = Mixer.WithLeftPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_Returns_Right_As_Default_Comparison_With_Additional_Props_Both_Sides()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1,
                AdditionalLeft = "Left"
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2,
                AdditionalRight = "Right"
            };

            dynamic result = Mixer.WithRightPriority().Mix(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_Returns_Default_Comparison_If_Not_Specified()
        {

            var obj1 = new
            {
                LineName = "LineName_obj_1",
                Wastage = 1,
                Qty = 1,
                AdditionalLeft = "Left"
            };

            var obj2 = new
            {
                LineName = "LineName_obj_2",
                Wastage = 2,
                Qty = 2,
                AdditionalRight = "Right"
            };

            dynamic result = Mixer.MixObjects(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_JObjects_ByDefault_Left_With_Right_Additional_Prop()
        {
            var json_1 =
                $"{{\r\n\t\"Option1\": \"1\",\r\n\t\"Option2\": \"1.5\",\r\n\t\"OptionResult\": \"{{expression}}\"\r\n}}";
            var json_2 =
                $"{{\r\n\t\"Option1\": \"2\",\r\n\t\"Option2\": \"2.5\",\r\n\t\"OptionResult\": \"{{expression}}\",\r\n\t\"Additional1\": \"\'Not Required\'\",\r\n}}";

            var jObj_1 = JObject.Parse(json_1);
            var jObj_2 = JObject.Parse(json_2);

            dynamic result = Mixer.MixObjects(jObj_1, jObj_2);

            Assert.Equal("1", result.Option1.ToString());
            Assert.Equal("1.5", result.Option2.ToString());
            Assert.Equal($"{{expression}}", result.OptionResult.ToString());
            Assert.Equal($"'Not Required'", result.Additional1.ToString());
        }

        [Fact]
        public void Mix_JObjects_With_Some_Left_Empty_Props()
        {
            var json_1 =
                $"{{\r\n\t\"Option1\": \"\",\r\n\t\"Option2\": \"\",\r\n\t\"OptionResult\": \"{{expression}}\"\r\n}}";
            var json_2 =
                $"{{\r\n\t\"Option1\": \"2\",\r\n\t\"Option2\": \"2.5\",\r\n\t\"OptionResult\": \"{{expression}}\",\r\n\t\"Additional1\": \"\'Not Required\'\",\r\n}}";

            var jObj_1 = JObject.Parse(json_1);
            var jObj_2 = JObject.Parse(json_2);

            dynamic result = Mixer.MixObjects(jObj_1, jObj_2);

            Assert.Equal("2", result.Option1.ToString());
            Assert.Equal("2.5", result.Option2.ToString());
            Assert.Equal($"{{expression}}", result.OptionResult.ToString());
            Assert.Equal($"'Not Required'", result.Additional1.ToString());
        }

        [Fact]
        public void Check_4_Array_Property_Within_Object()
        {
            
            var myClass = new 
            {
                Id = 1,
                Name = "My Name",
                IntSet = new List<int> { 2, 5, 8 }
            };

            var props = myClass.GetType().GetProperties();
            foreach (var info in props)
            {
                var type = info.PropertyType;

                if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    foreach (var listitem in info.GetValue(myClass, null) as IEnumerable)
                    {
                        _output.WriteLine("Item: " + listitem.ToString());
                    }

                    continue;
                }
                _output.WriteLine(info.GetValue(myClass, null).ToString());
            }
        }
        

        [Fact]
        public void Mix_Objects_Props_Where_Values_AreNot_Defined()
        {

            var obj_1 = new
            {
                Name = "Name_1",
                Id = ""
            };
            var obj_2 = new
            {
                Name = "",
                Id = 2
            };

            dynamic result = Mixer.MixObjects(obj_1, obj_2);

            Assert.Equal("Name_1", result.Name.ToString());
            Assert.Equal("2", result.Id.ToString());
        }



        [Fact]
        public void Mix_Objects_With_Nested_Object_Props()
        {
            var obj_1_nested = new
            {
                Id = 11,
                Name = ""
            };
            var obj_2_nested = new
            {
                Id = 22,
                Name = "Nested_2"
            };
            
            var obj_1 = new
            {
                Id = "",
                Name = "Name_1",
                NestedProp = obj_1_nested
            };
            var obj_2 = new
            {
                Id = 2,
                Name = "",
                NestedProp = obj_2_nested
            };

            dynamic result = Mixer.MixObjects(obj_1, obj_2);

            Assert.Equal("Name_1", result.Name.ToString());
            Assert.Equal("2", result.Id.ToString());
            Assert.Equal("Nested_2", result.NestedProp.Name.ToString());
            Assert.Equal("11", result.NestedProp.Id.ToString());
        }

        [Fact]
        public void Check_Enumerable_Property_String()
        {
            var str = "0.01";
            var indicator = str.GetType().GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));

            Assert.False(indicator);

        }
        [Fact]
        public void Check_Enumerable_Property_Of_Object()
        {
            var obj_1 = new {Id = 1};
            IEnumerable<object> enumProp = new List<object>{ obj_1 };

            var indicator = enumProp.GetType().GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));

            Assert.True(indicator);

        }

        [Fact]
        public void Check_Enumerable_Property_Of_Certain_Type()
        {
            var obj_1 = new Some { Id = Guid.NewGuid(), Name = string.Empty };
            var enumProp = new List<object> { obj_1 };

            var indicator = enumProp.GetType().GetInterfaces()
                .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>));

            Assert.True(indicator);

        }

        [Fact]
        public void Handle_Objects_In_The_Loop()
        {
            var module_1_left = new Some { Id = Guid.NewGuid(), Name = string.Empty };
            var module_1_right = new Some { Id = Guid.Empty, Name = "Module_1" };

            var module_2_left = new Some { Id = Guid.NewGuid(), Name = string.Empty };
            var module_2_right = new Some { Id = Guid.Empty, Name = "Module_2" };


            List<Some> leftSet = new List<Some> { module_1_left, module_2_left };
            List<Some> rightSet = new List<Some> { module_1_right, module_2_right };

            var resultSetMerged = new List<Some>(); 
            for (int i = 0; i < leftSet.Count(); i++)
            {
                ExpandoObject mixedObject = Mixer.MixObjects(leftSet[i], rightSet[i]);
                
                var target = ObjectsMapper.MapInto<Some>(mixedObject);

                resultSetMerged.Add(target);
            }

            Assert.Equal("Module_1", resultSetMerged[0].Name);
            Assert.Equal("Module_2", resultSetMerged[1].Name);
            Assert.NotEqual(Guid.Empty, resultSetMerged[0].Id);
            Assert.NotEqual(Guid.Empty, resultSetMerged[1].Id);

        }

        [Fact]
        public void Handle_Objects_In_The_Loop_LeftPriority()
        {
            var module_1_left = new Some { Id = Guid.NewGuid(), Name = "Module_1" };
            var module_1_right = new Some { Id = Guid.Empty, Name = "Module_2" };

            var module_2_left = new Some { Id = Guid.NewGuid(), Name = string.Empty };
            var module_2_right = new Some { Id = Guid.NewGuid(), Name = "Module_2" };


            List<Some> leftSet = new List<Some> { module_1_left, module_2_left };
            List<Some> rightSet = new List<Some> { module_1_right, module_2_right };

            var resultSetMerged = new List<Some>();
            for (int i = 0; i < leftSet.Count(); i++)
            {
                dynamic mixedObject = Mixer.MixObjects(leftSet[i], rightSet[i]);

                var target = ObjectsMapper.MapInto<Some>(mixedObject);

                resultSetMerged.Add(target);
            }

            Assert.Equal("Module_1", resultSetMerged[0].Name);
            Assert.Equal("Module_2", resultSetMerged[1].Name);
            Assert.NotEqual(Guid.Empty, resultSetMerged[0].Id);
            Assert.NotEqual(module_2_right.Id, resultSetMerged[1].Id);

        }

        [Fact]
        public void Handle_Objects_In_The_Loop_RightPriority()
        {
            var module_1_left = new Some { Id = Guid.NewGuid(), Name = "Module_1" };
            var module_1_right = new Some { Id = Guid.Empty, Name = "Module_2" };

            var module_2_left = new Some { Id = Guid.NewGuid(), Name = string.Empty };
            var module_2_right = new Some { Id = Guid.NewGuid(), Name = "Module_2" };


            List<Some> leftSet = new List<Some> { module_1_left, module_2_left };
            List<Some> rightSet = new List<Some> { module_1_right, module_2_right };

            var resultSetMerged = new List<Some>();
            for (int i = 0; i < leftSet.Count(); i++)
            {
                dynamic mixedObject = Mixer.WithRightPriority().Mix(leftSet[i], rightSet[i]);

                var target = ObjectsMapper.MapInto<Some>(mixedObject);

                resultSetMerged.Add(target);
            }

            Assert.Equal("Module_2", resultSetMerged[0].Name);
            Assert.Equal("Module_2", resultSetMerged[1].Name);
            Assert.Equal(module_2_right.Id, resultSetMerged[1].Id);

        }

        [Fact]
        public void Replace_Unnecessary_Symbols_InTheEnd_Of_String()
        {
            var input = new string[] {
                "6.61 /*",
                "6.61   ///",
                "6.61 *",
                "6.61 /*/",
                "6.61 *//*/**",
                "6.61 *//*/",
                "6.61  /*/",
                "6.61 *//*/",
                "6.61*//*/*",

            };
            for (int i = 0; i < input.Length; i++)
            {
                var matched = Regex.Match(input[i], @"(.*?)[\s\/*]+$");
                input[i] = matched.Groups[1].Value.ToString();

                Assert.Equal("6.61", input[i]);
            }


        }

        string GetDataFilesPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models"));
        }

        [Fact]
        public void Find_Input_JSON_file_And_Deserialize_Into_Dynamic()
        {
            var filePath = Path.Combine(GetDataFilesPath(), "left_json.json");

            var fileExists = File.Exists(filePath);
            Assert.True(fileExists);
            using (StreamReader sr = File.OpenText(filePath))
            {
                var objJSON = JsonConvert.DeserializeObject<ExpandoObject>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                var targetProps = typeof(TestClass).GetProperties();
                Assert.Equal(4, targetProps.Count());

                var target = new TestClass();
                new ObjectsMapper().Map<TestClass, Inner>((ExpandoObject)objJSON, target);

                // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);
                Assert.NotNull(target);
            }

        }

        [Fact]
        public void Complex_UsersInput_And_Formulas_Extracting_Deserialize_And_Final_Merge()
        {
            var filePathLeft = Path.Combine(GetDataFilesPath(), "left_json.json");
            var filePathRight = Path.Combine(GetDataFilesPath(), "right_json.json");
            
            var fileExistsLeft = File.Exists(filePathLeft);
            var fileExistsRight = File.Exists(filePathRight);
            Assert.True(fileExistsLeft);
            Assert.True(fileExistsRight);

            var targetLeft = new TestClass();
            var targetRight = new TestClass();

            using (StreamReader sr = File.OpenText(filePathLeft))
            {
                var objJSON = JsonConvert.DeserializeObject<ExpandoObject>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                new ObjectsMapper().Map<TestClass, Inner>((ExpandoObject)objJSON, targetLeft);
                // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);
            }
            using (StreamReader sr = File.OpenText(filePathRight))
            {
                var objJSON = JsonConvert.DeserializeObject<ExpandoObject>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                new ObjectsMapper().Map<TestClass, Inner>((ExpandoObject)objJSON, targetRight);
                // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);
            }

            Assert.NotNull(targetLeft);
            Assert.NotNull(targetRight);
            _output.WriteLine("Left:");
            _output.WriteLine($"Id: {targetLeft.Id}");
            _output.WriteLine($"Name: {targetLeft.Name}");
            _output.WriteLine($"Total: {targetLeft.Total}");
            _output.WriteLine($"Inner 1-st Class: {targetLeft.Inners.First().Class}");

            _output.WriteLine("Right:");
            _output.WriteLine($"Id: {targetRight.Id}");
            _output.WriteLine($"Name: {targetRight.Name}");
            _output.WriteLine($"Total: {targetRight.Total}");
            _output.WriteLine($"Inner 1-st Class: {targetRight.Inners.First().Class}");

            dynamic mixedObject = Mixer.MixObjects(targetLeft, targetRight);
            _output.WriteLine("====Mixed====:");
            _output.WriteLine($"Id: {mixedObject.Id}");
            _output.WriteLine($"Name: {mixedObject.Name}");
            _output.WriteLine($"Total: {mixedObject.Total}");
            _output.WriteLine($"Inner 1-st Class: {mixedObject.Inners[0].Class}");


            var target = new TestClass();
            new ObjectsMapper().Map<TestClass, Inner>(mixedObject, target);
            // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);

            _output.WriteLine("====Mapped====:");
            _output.WriteLine($"Id: {target.Id}");
            _output.WriteLine($"Name: {target.Name}");
            _output.WriteLine($"Total: {target.Total}");
            _output.WriteLine($"Inner 1-st Class: {target.Inners.First().Class}");

            Assert.NotNull(target);
        }

        [Fact]
        public void Not_It_1()
        {
            var filePathLeft = Path.Combine(GetDataFilesPath(), "json1.json");
            var fileExistsLeft = File.Exists(filePathLeft);
            Assert.True(fileExistsLeft);

            using (StreamReader sr = File.OpenText(filePathLeft))
            {
                var objJSON = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                // search for types where keys are equal to value.Name property if it property exists and values is JObject
                

                // todo: check this type of determination in Calculator svc
                var anonymousKeys = _svc.GetNamesOfPropertiesWhichAreJObjects(objJSON);
                Assert.True(anonymousKeys.Count() == 3);

                var listOfJObjects = new List<Inner>();

                foreach (var key in anonymousKeys)
                {
                    // TODO: check it for type with diff types of prop (bool, string, int, double, guid)
                    // in case we catch exception we need to use object mapper class
                    // also check for bool type in mapping for cases we use config QtyChanged and CostChanged

                    var currentVal = JsonConvert.DeserializeObject<Inner>(objJSON[key].ToString());
                    Assert.NotNull(currentVal);
                    Assert.NotEmpty(currentVal.Name);

                    listOfJObjects.Add(currentVal);
                }

                Assert.True(listOfJObjects.Count == 3);
                //new ObjectsMapper().Map<TestClass, Inner>((ExpandoObject)objJSON, targetLeft);
                // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);
            }

        }

        [Fact]
        public void Not_It_1_Typed_Inner()
        {
            var filePathLeft = Path.Combine(GetDataFilesPath(), "json2.json");
            var fileExistsLeft = File.Exists(filePathLeft);
            Assert.True(fileExistsLeft);

            using (StreamReader sr = File.OpenText(filePathLeft))
            {
                var objJSON = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                // search for types where keys are equal to value.Name property if it property exists and values is JObject
                var svc = new ObjectsMapperService();

                // todo: check this type of determination in Calculator svc
                var anonymousKeys = svc.GetNamesOfPropertiesWhichAreJObjects(objJSON);
                Assert.True(anonymousKeys.Count() == 3);

                var listOfJObjects = new List<TypedInner>(); //remove it to have mapping in the loop

                foreach (var key in anonymousKeys)
                {
                    // TODO: check it for type with diff types of prop (bool, string, int, double, guid)
                    // in case we catch exception we need to use object mapper class
                    // also check for bool type in mapping for cases we use config QtyChanged and CostChanged

                    var converted = JsonConvert.DeserializeObject<ExpandoObject>(objJSON[key].ToString());
                    var currentVal = ObjectsMapper.MapInto<TypedInner>(converted);

                    Assert.NotNull(currentVal);
                    Assert.NotEmpty(currentVal.Name);

                    listOfJObjects.Add(currentVal);
                }

                Assert.True(listOfJObjects.Count == 3);
                Assert.True(listOfJObjects.Last().Class);
                Assert.True(listOfJObjects.Last().Amount == 33.3d);
                //new ObjectsMapper().Map<TestClass, Inner>((ExpandoObject)objJSON, targetLeft);
                // TODO: like var target = ObjectsMapper.MapInto<TestClass>(objJSON);
            }

        }

        [Fact]
        public void MapInto__SomeWithTypedInner()
        {
            var filePathLeft = Path.Combine(GetDataFilesPath(), "json2.json");
            var fileExistsLeft = File.Exists(filePathLeft);
            Assert.True(fileExistsLeft);

            using (StreamReader sr = File.OpenText(filePathLeft))
            {
                var objJSON = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                var svc = new ObjectsMapperService();
                var anonymousKeys = svc.GetNamesOfPropertiesWhichAreJObjects(objJSON);
                Assert.True(anonymousKeys.Count() == 3);

                var targetType = new SomeWithTypedInner();
                var nameOfEnumerableProperty = svc.GetPropertiesWhichAreEnumerable(targetType);
                Assert.Equal("Inners", nameOfEnumerableProperty.First().Name);

                var listOfJObjects = _svc.CreateListOf<TypedInner>(objJSON);

                Assert.True(listOfJObjects.Count() == 3);
                Assert.True(listOfJObjects.Last().Class);
                Assert.True(listOfJObjects.Last().Amount == 33.3d);
            }

        }

        [Fact]
        public void Not_It_1_Typed_Inner_22()
        {
            var filePathLeft = Path.Combine(GetDataFilesPath(), "json2.json");
            var fileExistsLeft = File.Exists(filePathLeft);
            Assert.True(fileExistsLeft);

            using (StreamReader sr = File.OpenText(filePathLeft))
            {
                var objJSON = JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());
                Assert.NotNull(objJSON);

                var targetList = _svc.CreateListOf<TypedInner>(objJSON);

                Assert.True(targetList.Count() == 3);
                Assert.True(targetList.Last().Class);
                Assert.True(targetList.Last().Amount == 33.3d);
            }

        }
        
        /* Ignore Props */
        [Fact]
        public void Merge_Types_with_Ignore()
        {
            var obj1 = new
            {
                Property1 = "value_1",
                Property2 = "value_2"
            };
            var obj2 = new
            {
                Property1 = "value_11",
                Property4 = "value_4" /* ignore it */
            };

            dynamic result = Mixer
                .Ignore(() => obj2.Property4)
                .Mix(obj1, obj2);

            Assert.NotNull(result.Property1.ToString());
            Assert.Equal("value_1", result.Property1.ToString());
            Assert.NotNull(result.Property2.ToString());
            Assert.Equal("value_2", result.Property2.ToString());
        }

        private static object GetResultValue(ExpandoObject result, string propName)
        {
            return result.GetType().GetProperty(propName).GetValue(result);
        }

    }

}