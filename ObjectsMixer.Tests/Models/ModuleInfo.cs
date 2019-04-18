using System;

namespace ObjectsMixer.Tests.Models
{
    public class ModuleInfo
    {
        //private Guid _id;

        //public Guid Id {
        //    set => _id = Guid.Parse(value.ToString());
        //    get => _id;
        //}
        public Guid Id { get; set; }
        public virtual string Name { get; set; }
    }

}
