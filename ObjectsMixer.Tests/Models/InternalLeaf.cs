namespace ObjectsMixer.Tests.Models
{
    public class InternalLeaf : Line
    {
        public static InternalLeaf GetWithDefaultAndFormulas()
        {
            return new InternalLeaf
            {
                RowName = "Internal Leaf",
                Enabled = "true",
                Class = "Material",
                Category = "Bricks",
                CategoryId = "83b738c5-5e77-42ab-b20d-87bd5af50b42",
                Material = "Allowance per Thousand Bricks £1000.00",
                MaterialPriceView = "1000",
                Unit = "Pounds",
                UnitType = "EA",
                QtyView = "ROUNDUP(({Qty}*(1+{MAT})),0)",
                Qty = "ROUNDUP(({Area of Wall With Openings Deducted}/{tmp_AF}),0)",
                PlantCost = "",
                PlantCostView = "",
                MaterialPrice = "1",
                MaterialCostView = "{QtyView}*{MaterialPrice}",
                Hours = "",
                HPU = "",
                CalculatedFixedLabourPrice = "",
                LabourCost = "",
                TotalCost = "",
                Trade = "",
                WorkSection = "",
                CalculatedTrade = "",

                tmp_AA = "215",
                tmp_AB = "102.5",
                tmp_AC = "65",
                tmp_AD = "10",
                tmp_AE = "10",
                tmp_AF = "(({tmp_AA}+{tmp_AD})/1000)*(({tmp_AC}+{tmp_AE})/1000)"
            };
        }

        public static InternalLeaf GetWithDefault()
        {
            return new InternalLeaf
            {
                RowName = "Internal Leaf",
                Enabled = "true",
                Class = "Material",
                Category = "Bricks",
                CategoryId = "83b738c5-5e77-42ab-b20d-87bd5af50b42",
                Material = "Allowance per Thousand Bricks £1000.00",
                MaterialPriceView = "1000",
                Unit = "Pounds",
                UnitType = "EA",
                QtyView = "",
                Qty = "",
                PlantCost = "",
                PlantCostView = "",
                MaterialPrice = "1",
                MaterialCostView = "",
                Hours = "",
                HPU = "",
                CalculatedFixedLabourPrice = "",
                LabourCost = "",
                TotalCost = "",
                Trade = "",
                WorkSection = "",
                CalculatedTrade = "",

                tmp_AA = "215",
                tmp_AB = "102.5",
                tmp_AC = "65",
                tmp_AD = "10",
                tmp_AE = "10",
                tmp_AF = ""
            };
        }

        public static InternalLeaf GetWithFormulas()
        {
            return new InternalLeaf
            {
                RowName = "",
                Enabled = "",
                Class = "",
                Category = "",
                CategoryId = "",
                Material = "",
                MaterialPriceView = "",
                Unit = "",
                QtyView = "ROUNDUP(({Qty}*(1+{MAT})),0)",
                Qty = "ROUNDUP(({Area of Wall With Openings Deducted}/{tmp_AF}),0)",
                PlantCost = "",
                PlantCostView = "",
                MaterialPrice = "",
                MaterialCostView = "{QtyView}*{MaterialPrice}",
                Hours = "",
                HPU = "",
                CalculatedFixedLabourPrice = "",
                LabourCost = "",
                TotalCost = "",
                Trade = "",
                WorkSection = "",
                CalculatedTrade = "",

                tmp_AA = "",
                tmp_AB = "",
                tmp_AC = "",
                tmp_AD = "",
                tmp_AE = "",
                tmp_AF = "(({tmp_AA}+{tmp_AD})/1000)*(({tmp_AC}+{tmp_AE})/1000)"
            };
        }

    }
}
