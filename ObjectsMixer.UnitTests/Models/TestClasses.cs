using System;
using System.Collections.Generic;

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

}
