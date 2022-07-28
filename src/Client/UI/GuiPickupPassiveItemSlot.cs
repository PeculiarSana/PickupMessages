using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Cairo;

namespace PickupMessages.Client.UI
{
    internal class GuiPickupPassiveItemSlot : GuiElementPassiveItemSlot
    {
        public static double unscaledItemSize = 22;

        public static double unscaledSlotSize = 48.0;

        private ItemSlot slot;

        private IInventory inventory;

        private bool drawBackground;

        private GuiElementStaticText textComposer;

        public GuiPickupPassiveItemSlot(ICoreClientAPI capi, ElementBounds bounds, IInventory inventory, ItemSlot slot, bool drawBackground = true) : base(capi, bounds, inventory, slot, drawBackground)
        {
            this.slot = slot;
            this.inventory = inventory;
            this.drawBackground = drawBackground;
            bounds.fixedWidth = unscaledSlotSize;
            bounds.fixedHeight = unscaledSlotSize;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            //IL_00c4: Unknown result type (might be due to invalid IL or missing references)
            Bounds.CalcWorldBounds();
            if (drawBackground)
            {
                ctx.SetSourceRGBA(1.0, 1.0, 1.0, 0.6);
                ElementRoundRectangle(ctx, Bounds);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, Bounds, inverse: true);
            }

            double absSlotSize = scaled(unscaledSlotSize);

            ElementBounds textBounds = ElementBounds
              .Fixed(0, unscaledSlotSize - GuiStyle.SmallFontSize - 2, unscaledSlotSize - 5, unscaledSlotSize - 5)
              .WithEmptyParent();

            CairoFont font = CairoFont.WhiteSmallText();
            font.FontWeight = FontWeight.Bold;
            textComposer = new GuiElementStaticText(api, "", EnumTextOrientation.Right, textBounds, font);

        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (slot.Itemstack != null)
            {
                double num = GuiElement.scaled(unscaledSlotSize) / 2.0;
                api.Render.PushScissor(Bounds, stacking: true);
                api.Render.RenderItemstackToGui(slot, Bounds.renderX + num, Bounds.renderY + num, 450.0, (float)GuiElement.scaled(unscaledItemSize), -1, true, false, false);
                api.Render.PopScissor();
            }
        }
    }
}
