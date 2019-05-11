using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ObjectsMixer.UnitTests.Models
{
    public class TestClass : SomeBase
    {
        public string Total { get; set; } = "";
    }
    public class SomeBase : Some
    {
        public IEnumerable<Inner> Inners { get; set; }
    }
    public class Some
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
    public class Inner
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Amount { get; set; }
    }

    public class OtherClass
    {
        [DisplayName("Property One")]
        public string Property1 { get; set; }
        [DisplayName("Property Two")]
        public string Property2 { get; set; }
        public string Property3 { get; set; }

    }

    public class OtherClassEnum
    {
        public string Property1 { get; set; }
        public IEnumerable<string> Property2 { get; set; }
    }

    public class OtherClassEnumTyped
    {
        public string Property1 { get; set; }

        [DisplayName("Property Two")]
        public string Property2 { get; set; }

        public IEnumerable<OtherInner> Property3 { get; set; }
    }

    public class OtherClassInnerTyped
    {
        public string Property1 { get; set; }

        [DisplayName("Property Two")]
        public string Property2 { get; set; }

        public OtherInner Property3 { get; set; }
    }

    public class OtherInner
    {
        public int Id { get; set; } /* we don't handle different types of property */
        public string Name { get; set; }
    }

}
