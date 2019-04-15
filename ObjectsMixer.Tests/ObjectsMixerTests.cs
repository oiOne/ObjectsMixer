using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
        public void ObjectMixer_Returns_Left_As_Default_Comparison_With_Additional_Prop_Left()
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

            dynamic result = ObjectMixer.WithLeftPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Right_As_Default_Comparison_With_Additional_Prop_Left()
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

            dynamic result = ObjectMixer.WithRightPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Left_As_Default_Comparison_With_Additional_Prop_Right()
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

            dynamic result = ObjectMixer.WithLeftPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Right_As_Default_Comparison_With_Additional_Prop_Right()
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

            dynamic result = ObjectMixer.WithRightPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Left_As_Default_Comparison_With_Additional_Props_Both_Sides()
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

            dynamic result = ObjectMixer.WithLeftPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Right_As_Default_Comparison_With_Additional_Props_Both_Sides()
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

            dynamic result = ObjectMixer.WithRightPriority().Merge(obj1, obj2);

            Assert.Equal("LineName_obj_2", result.LineName.ToString());
            Assert.Equal("2", result.Wastage.ToString());
            Assert.Equal("2", result.Qty.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void ObjectMixer_Returns_Default_Comparison_If_Not_Specified()
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

            dynamic result = ObjectMixer.MergeObjects(obj1, obj2);

            Assert.Equal("LineName_obj_1", result.LineName.ToString());
            Assert.Equal("1", result.Wastage.ToString());
            Assert.Equal("Left", result.AdditionalLeft.ToString());
            Assert.Equal("1", result.Qty.ToString());
            Assert.Equal("Right", result.AdditionalRight.ToString());
        }

        [Fact]
        public void Mix_JObjects_Default_Left_With_Right_Additional_Prop()
        {
            var json_1 =
                $"{{\r\n\t\"Wastage\": \"1\",\r\n\t\"Qty\": \"1.5\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\"\r\n}}";
            var json_2 =
                $"{{\r\n\t\"Wastage\": \"2\",\r\n\t\"Qty\": \"2.5\",\r\n\t\"ROUNDUP_Result\": \"ROUNDUP(({{Qty}}*(1+{{Wastage}})),0)\",\r\n\t\"QtyView\": \"\'Not Required\'\",\r\n}}";

            var jObj_1 = JObject.Parse(json_1);
            var jObj_2 = JObject.Parse(json_2);

            dynamic result = ObjectMixer.MergeObjects(jObj_1, jObj_2);

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

            dynamic result = ObjectMixer.MergeObjects(jObj_1, jObj_2);

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
                Subordinates = new List<int> { 2, 5, 8 }
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

            dynamic result = ObjectMixer.MergeObjects(obj_1, obj_2);

            Assert.Equal("Name_1", result.Name.ToString());
            Assert.Equal("2", result.Id.ToString());
        }

    }

}