using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace PickupMessages.Client.UI
{
    class PickupFont : CairoFont
    {
        public static PickupFont StrokedWhiteText()
        {
            return new PickupFont
            {
                Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
                Fontname = GuiStyle.StandardFontName,
                UnscaledFontsize = GuiStyle.SmallishFontSize,
                StrokeWidth = 2,
                StrokeColor = ColorUtil.Hex2Doubles("#000000", 1.0)
            };
        }
    }
}
