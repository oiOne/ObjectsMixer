using System.Collections.Generic;
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
        public void Detect_NestedDictionary_In_Dictionary_Config()
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

            var result = _mapperSvc.GetNamesOfPropertiesWhichAreDictionary(config);

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
        public void MapFrom_Config_Into_Obj_With_ParticularType_Prop()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "Value2"},
                {"Property3", new
                    {
                        Id= 11,
                        Name= "Property3"
                    }
                }
            };

            var result = ObjectsMapper.MapInto<OtherClassInnerTyped>(config);

            Assert.Equal("Value1", result.Property1);
            Assert.Equal("Value2", result.Property2);
            Assert.Equal(11, result.Property3.Id);
            Assert.Equal("Property3", result.Property3.Name);
        }

        

        [Fact]
        public void MapFrom_Config_Into_Obj_With_List_of_ParticularType_Prop()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "Value2"},
                {"Property3", new []  
                    {
                        new { Id= 11, Name= "Property31" }, 
                        new { Id= 12, Name= "Property32" },
                    }
                }
            };

            var result = ObjectsMapper.MapInto<OtherClassEnumTyped>(config);

            // TODO: like below
            //var result = _mapper.MapInto<OtherClassEnumTyped>(config);

            Assert.Equal("Value1", result.Property1);
            Assert.Equal("Value2", result.Property2);
            Assert.Equal(11, result.Property3.First().Id);
            Assert.Equal(12, result.Property3.Last().Id);
            Assert.Equal("Property31", result.Property3.First().Name);
            Assert.Equal("Property32", result.Property3.Last().Name);
        }

        [Fact]
        public void Assign_Complex_With_Two_Items_In_List()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "2"},
                {"Property3", new List<object>
                    {
                        new { Id= 31, Name= "Property3_1" },
                        new { Id= 32, Name= "Property3_2" },
                    } 
                }
            };

            var result = ObjectsMapper.MapInto<OtherClassEnumTyped>(config);

            // TODO: like below
            //var result = _mapper.MapInto<OtherClassEnumTyped>(config);

            Assert.Equal("Value1", result.Property1);
            Assert.Equal("2", result.Property2);

            Assert.Equal("Property3_1", result.Property3.First().Name);
            Assert.Equal(31, result.Property3.First().Id);

            Assert.Equal("Property3_2", result.Property3.Last().Name);
            Assert.Equal(32, result.Property3.Last().Id);
        }

        /* Dynamic as source */

        [Fact]
        public void MapInto_Particular_Obj_From_Dynamic()
        {
            var resource = new
            {
                Id = 11,
                Name = "Property3"
            };

            var result = ObjectsMapper.MapInto<OtherInner>(resource);

            Assert.Equal(11, result.Id);
            Assert.Equal("Property3", result.Name);
        }

        [Fact]
        public void MapInto_Particular_Obj_With_Nested_One_From_Dynamic()
        {
            var resource = new {
                Property1 = "Value1",
                Property2 = "Value2",
                Property3 = new {
                    Id = 11,
                    Name = "Property3"
                }
            };

            var result = ObjectsMapper.MapInto<OtherClassInnerTyped>(resource);

            // TODO: like below
            //var result = _mapper.MapInto<OtherClassInnerTyped>(config);

            Assert.Equal("Value1", result.Property1);
            Assert.Equal("Value2", result.Property2);
            Assert.Equal(11, result.Property3.Id);
            Assert.Equal("Property3", result.Property3.Name);
        }

        /* MapIntoList */
        [Fact]
        public void MapInto_List_dynamic()
        {
            var source = new []
            {
                new { Id= 11, Name= "Property31" },
                new { Id= 12, Name= "Property32" }
            };

            var result = ObjectsMapper.MapIntoListOf<OtherInner>(source);

            Assert.True(result.Count() == 2);
            Assert.True(result.First().Id == 11);
            Assert.True(result.Last().Id == 12);
        }

        [Fact]
        public void MapInto_List_Dictionary()
        {
            var source = new Dictionary<string, object>
            {
                { "First", new Dictionary<string, object> { {"Id", 11 }, { "Name", "Property31" } } },
                { "Second", new Dictionary<string, object> { {"Id", 12 }, { "Name", "Property32" } } },
            };

            var result = ObjectsMapper.MapIntoListOf<OtherInner>(source);

            Assert.True(result.Count() == 2);
            Assert.True(result.First().Id == 11);
            Assert.Equal("Property31", result.First().Name);
            Assert.True(result.Last().Id == 12);
            Assert.Equal("Property32", result.Last().Name);
        }

        [Fact]
        public void MapInto_List_Dictionary_Anonymous()
        {
            var source = new Dictionary<string, object>
            {
                { "First", new { Id= 11, Name= "Property31" } },
                { "Second", new { Id= 12, Name= "Property32" } }
            };

            var result = ObjectsMapper.MapIntoListOf<OtherInner>(source);

            Assert.True(result.Count() == 2);
            Assert.True(result.First().Id == 11);
            Assert.Equal("Property31", result.First().Name);
            Assert.True(result.Last().Id == 12);
            Assert.Equal("Property32", result.Last().Name);
        }

        /* Ignoring some */
        [Fact]
        public void Map_And_Ignore_First()
        {
            var config = new Dictionary<string, object>
            {
                {"Property1", "Value1"},
                {"Property Two", "Value2"},
                {"Property3", new []
                    {
                        new { Id= 11, Name= "Property31" },
                        new { Id= 12, Name= "Property32" },
                    }
                }
            };
            // we'd like to have such functionality
            // var result = ObjectsMapper.Ignore(() => target.Property1).MapInto<OtherClassEnumTyped>(config);

            //Assert.Null(result.Property1);
            Assert.True(false);
        }

        [Fact]
        public void Check_Default_4_String()
        {
            var tString = typeof(string);
            var defaultVal = _mapperSvc.GetDefault(tString);

            Assert.Equal(string.Empty, defaultVal);
        }

        
    }
}
