using System.Collections.Generic;

namespace ObjectsMixer.Tests.Models
{
    public class ModuleConfiguration : ModuleInfo
    {
        public IEnumerable<Line> Lines { get; set; }
    }

}
