using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PickupMessages.Client.UI
{
    public static class GuiPickupComposerHelpers
    {
        public static GuiComposer AddPassiveItemSlotNoText(this GuiComposer composer, ElementBounds bounds, IInventory inventory, ItemSlot slot, bool drawBackground = true)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiPickupPassiveItemSlot(composer.Api, bounds, inventory, slot, drawBackground));
            }

            return composer;
        }
    }
}
