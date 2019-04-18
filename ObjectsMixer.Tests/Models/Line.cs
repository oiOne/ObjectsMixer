namespace ObjectsMixer.Tests.Models
{
    public class Line
    {
        public string RowName { get; set; }
        public string Enabled { get; set; }
        public string Class { get; set; }
        public string Category { get; set; }
        public string Material { get; set; }
        public string MaterialPriceView { get; set; }
        public string Unit { get; set; }
        public string UnitType { get; set; }
        public string CategoryId { get; set; }
        public string QtyView { get; set; }
        public string Qty { get; set; }
        public string PlantCost { get; set; }
        public string PlantCostView { get; set; }
        public string MaterialPrice { get; set; }
        public string MaterialCostView { get; set; }
        public string Hours { get; set; }
        public string HPU { get; set; }
        public string CalculatedFixedLabourPrice { get; set; }
        public string LabourCost { get; set; }
        public string TotalCost { get; set; }
        public string Trade { get; set; }
        public string WorkSection { get; set; }
        public string CalculatedTrade { get; set; }

        public virtual string tmp_AA { get; set; } = "";
        public virtual string tmp_AB { get; set; } = "";
        public virtual string tmp_AC { get; set; } = "";
        public virtual string tmp_AD { get; set; } = "";
        public virtual string tmp_AE { get; set; } = "";
        public virtual string tmp_AF { get; set; } = "";
        
    }
}
