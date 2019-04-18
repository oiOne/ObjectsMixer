using System.ComponentModel;
using ObjectsMixer.Tests.Services;

namespace ObjectsMixer.Tests.Models
{
    public class BrickBrickModuleConfiguration : ModuleConfiguration
    {
        public override string Name { get; set; }

        public static BrickBrickModuleConfiguration InitDefaults()
        {
            return new BrickBrickModuleConfiguration
            {
                Name = "Wall Brick/Brick"
            };
        }
        
        [DisplayName("Wall Length")]
        public string WallLength { get; set; }

        [DisplayName("Wall Height")]
        public string WallHeight { get; set; }

        [DisplayName("Wall Openings")]
        public string Openings { get; set; }

        [DisplayName("Total Area of Wall Including Openings")]
        public string TotalArea { get; set; }

        [DisplayName("Area of Wall With Openings Deducted")]
        public string AreaOfWall { get; set; }

        [DisplayName("Total Cost")]
        public string TotalCost { get; set; }

        public string Total { get; set; }

        public string MAT { get; set; }

    }
}
