﻿using System.Collections.Generic;
using System.Linq;
using ObjectsMixer.UnitTests.Models;
using Xunit;
using Xunit.Abstractions;

namespace ObjectsMixer.UnitTests
{
    public class ObjectsMapperTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ObjectsMapperService _mapperSvc;
        public ObjectsMapperTests(ITestOutputHelper output)
        {
            _output = output;
            _mapperSvc =  new ObjectsMapperService();
        }

        [Fact]
        public void Returns_Proper_DisplayName_If_They_Presented()
        {
            var otherClass = new OtherClass
            {
                Property1 = "Value1",
                Property2 = "Value2",
                Property3 = "Value3"
            };

            var dict = _mapperSvc.GetPairsOfPropsAndValues(otherClass);

            Assert.True(dict.ContainsKey("Property One"));
            Assert.True(dict.ContainsKey("Property Two"));
            Assert.True(dict.ContainsKey("Property3"));
        }

        [Fact]
        public void Populate_Object_With_Dictionary_Configuration()
        {
            var configuration = new Dictionary<string, string>
            {
                {"Property One", "Value1"},
                {"Property Two", "Value2"},
                {"Property3", "Value3"},
            };

            var result = _mapperSvc.AssignConfigurationWithObject<OtherClass>(configuration);

            Assert.Equal("Value1", result.Property1);
            Assert.Equal("Value2", result.Property2);
            Assert.Equal("Value3", result.Property3);
        }

        [Fact]
        public void Check_For_Enumerable_Property()
        {
            var obj = new OtherClassEnum
            {
                Property1 = "Value1",
                Property2 = new List<string> { "Value2" }
            };
            var result = _mapperSvc.GetNamesOfPropertiesWhichAreEnumerable(obj);

            Assert.Single(result);
            Assert.Equal("Property2", result.First());
        }

        [Fact]
        public void Check_For_Enumerable_Property_NotExists()
        {
            var obj = new
            {
                Property1 = "Value1",
            };
            var result = _mapperSvc.GetNamesOfPropertiesWhichAreEnumerable(obj);

            Assert.Empty(result);
        }

        [Fact]
        public void Check_For_Enumerable_Properties()
        {
            var obj = new
            {
                Property1 = "Value1",
                Property2 = new List<string> { "Value2" },
                Property3 = new List<string> { "Value3" }
            };
            var result = _mapperSvc.GetNamesOfPropertiesWhichAreEnumerable(obj);

            Assert.Equal(2, result.Count());
            Assert.Equal("Property2", result.First());
            Assert.Equal("Property3", result.Last());
        }

        [Fact]
        public void Detect_AnonymousType_In_Dictionary_Config()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "2"},
                {"Property3", new Dictionary<string, object>
                            {
                                {"Id", "11"}, 
                                {"Name", "Property3"}
                            }
                }
            };

            var result = _mapperSvc.GetNamesOfPropertiesWhichAreAnonymous(config);

            Assert.Single(result);
            Assert.Equal("Property3", result.First());
        }

        [Fact]
        public void Detect_AnonymousType_In_Dynamic_Config()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "2"},
                {"Property3", new 
                                {
                                    Id= "11",
                                    Name= "Property3"
                                }
                }
            };

            var result = _mapperSvc.GetNamesOfPropertiesWhichAreAnonymous(config);

            Assert.Single(result);
            Assert.Equal("Property3", result.First());
        }
        
        [Fact]
        public void Returns_List_Of_AnonymousTypes_BasedOn_Config()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "2"},
                {"Property3", new
                    {
                        Id= 11,
                        Name= "Property3"
                    }
                }
            };

            var result = _mapperSvc.CreateListOfAnonymousTypesFromConfig<OtherInner>(config);

            Assert.Single(result);
            Assert.Equal("Property3", result.First().Name);
        }

        [Fact]
        public void Assign_Complex()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "2"},
                {"Property3", new
                    {
                        Id= 11,
                        Name= "Property3"
                    }
                }
            };

            var ssd = _mapperSvc.AssignConfigurationWithComplexObject<OtherClassEnumTyped>(config);

            Assert.Equal("Value1", ssd.Property1);
            Assert.Equal("2", ssd.Property2);
            Assert.Equal("Property3", ssd.Property3.First().Name);
        }

    }
}
