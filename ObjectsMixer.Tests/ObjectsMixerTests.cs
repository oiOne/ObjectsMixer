using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ObjectsMixer.Tests.Models;
using ObjectsMixer.Tests.Services;
using Xunit;
using Xunit.Abstractions;

namespace ObjectsMixer.Tests
{

    public class ObjectsMixerTests
    {
        private readonly ITestOutputHelper _output;

        public ObjectsMixerTests(ITestOutputHelper output)
        {
            _output = output;
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
                $"{{\r\n\t\"Wastage\": \"1\",\r\n\t\"Qty\": \"1.5\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\"\r\n}}";
            var json_2 =
                $"{{\r\n\t\"Wastage\": \"2\",\r\n\t\"Qty\": \"2.5\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\",\r\n\t\"QtyView\": \"\'Not Required\'\",\r\n}}";

            var jObj_1 = JObject.Parse(json_1);
            var jObj_2 = JObject.Parse(json_2);

            dynamic result = Mixer.MixObjects(jObj_1, jObj_2);

            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1.5", result.Qty.ToString());
            Assert.Equal($"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)", result.ROUNDUP_Result.ToString());
            Assert.Equal($"'Not Required'", result.QtyView.ToString());
        }

        [Fact]
        public void Mix_JObjects_With_Some_Left_Empty_Props()
        {
            var json_1 =
                $"{{\r\n\t\"Wastage\": \"\",\r\n\t\"Qty\": \"\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\"\r\n}}";
            var json_2 =
                $"{{\r\n\t\"Wastage\": \"2\",\r\n\t\"Qty\": \"2.5\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\",\r\n\t\"QtyView\": \"\'Not Required\'\",\r\n}}";

            var jObj_1 = JObject.Parse(json_1);
            var jObj_2 = JObject.Parse(json_2);

            dynamic result = Mixer.MixObjects(jObj_1, jObj_2);

            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2.5", result.Qty.ToString());
            Assert.Equal($"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)", result.ROUNDUP_Result.ToString());
            Assert.Equal($"'Not Required'", result.QtyView.ToString());
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
        public void Mix_Lines_Formulas_And_Default()
        {

            var obj_default = InternalLeaf.GetWithDefault();
            var obj_formulas = InternalLeaf.GetWithFormulas();

            dynamic result = Mixer.MixObjects(obj_formulas, obj_default);

            var expected = InternalLeaf.GetWithDefaultAndFormulas();

            // default
            Assert.Equal(expected.RowName, result.RowName.ToString());
            Assert.Equal(expected.Enabled, result.Enabled.ToString());

            // formulas
            Assert.Equal(expected.Qty, result.Qty.ToString());
            Assert.Equal(expected.QtyView, result.QtyView.ToString());

        }

        public void Mix_Objects_Which_Have_Enumerable()
        {
            var brick_config_default = new BrickBrickModuleConfiguration();
            brick_config_default.MAT = "0.06";
            var line_default = InternalLeaf.GetWithDefault();
            brick_config_default.Lines = new List<Line>{ line_default };

            var line_formulas = InternalLeaf.GetWithFormulas();
            var brick_config_formulas = new BrickBrickModuleConfiguration();
            brick_config_formulas.Lines = new List<Line> { line_formulas };

            dynamic result = Mixer.MixObjects(brick_config_default, brick_config_formulas);

            var expected = InternalLeaf.GetWithDefaultAndFormulas();

            // default
            Assert.Equal(expected.RowName, result.RowName.ToString());
            Assert.Equal(expected.Enabled, result.Enabled.ToString());

            // formulas
            Assert.Equal(expected.Qty, result.Qty.ToString());
            Assert.Equal(expected.QtyView, result.QtyView.ToString());

        }

        [Fact]
        public void Check_Enumerable_Property_String()
        {
            var str = "0.06";
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
        public void Handle_Objects_In_The_Loop()
        {
            var module_1_left = new ModuleInfo { Id = Guid.NewGuid(), Name = string.Empty };
            var module_1_right = new ModuleInfo { Id = Guid.Empty, Name = "Module_1" };

            var module_2_left = new ModuleInfo { Id = Guid.NewGuid(), Name = string.Empty };
            var module_2_right = new ModuleInfo { Id = Guid.Empty, Name = "Module_2" };


            List<ModuleInfo> leftSet = new List<ModuleInfo> { module_1_left, module_2_left };
            List<ModuleInfo> rightSet = new List<ModuleInfo> { module_1_right, module_2_right };

            var resultSetMerged = new List<ModuleInfo>(); 
            for (int i = 0; i < leftSet.Count(); i++)
            {
                dynamic mixedObject = Mixer.MixObjects(leftSet[i], rightSet[i]);
                var target = new ModuleInfo();
                Mapper<ModuleInfo>.Map(mixedObject, target);

                resultSetMerged.Add(target);
            }

            Assert.Equal("Module_1", resultSetMerged[0].Name);
            Assert.Equal("Module_2", resultSetMerged[1].Name);
            Assert.NotEqual(Guid.Empty, resultSetMerged[0].Id);
            Assert.NotEqual(Guid.Empty, resultSetMerged[1].Id);

        }
    }

}