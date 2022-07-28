using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace PickupMessages.Client.UI
{
    public class GuiPickupMessage : HudElement
    {
        int quantity = 1;

        private int posInList;
        private GuiComposer composer;
        private long tickId;

        public InventoryBase unspoilableInventory;
        public DummySlot dummySlot;

        public ItemStack Stack;

        //TODO: Check if empty slot
        public GuiPickupMessage(ICoreClientAPI capi, ItemStack stack) : base(capi)
        {
            IClientPlayer player = capi.World.Player;
            Stack = stack;
            quantity = stack.StackSize;
            posInList = capi.Gui.OpenedGuis.OfType<GuiPickupMessage>().Count();

            unspoilableInventory = new CreativeInventoryTab(1, "not-used", null);
            dummySlot = new DummySlot(Stack, unspoilableInventory);

            Stack.ResolveBlockOrItem(capi.World);

            //update = capi.Event.RegisterGameTickListener(new Action<float>(Update), 5, 0);
            tickId = capi.Event.RegisterGameTickListener(new Action<float>(TimeOut), 3000, 0);


            ComposeGuis(Stack, posInList);
        }

        /*void Update(float dt)
        {
            if (MonitoredSlots.Count > 1)
            {
                if (MonitoredSlots.Any(item => item.Itemstack == null))
                    return;

                relevantItemStack = MonitoredSlots[0].Itemstack;

                totalQuantity = MonitoredSlots.Sum(item => item.StackSize);
                

                if (totalQuantity > oldQuantity)
                {
                    dummySlot.Itemstack = MonitoredSlots[0].Itemstack;
                    amountChanged += Math.Abs(totalQuantity - oldQuantity);
                    UpdatePickupText();
                    oldQuantity = totalQuantity;
                }
                if (totalQuantity < oldQuantity)
                {
                    oldQuantity = totalQuantity;
                }
            }

            else if (MonitoredSlots.Count == 1 && MonitoredSlots[0].Itemstack != null)
            {
                relevantItemStack = MonitoredSlots[0].Itemstack;

                if (MonitoredSlots[0].StackSize > oldQuantity)
                {
                    dummySlot.Itemstack = MonitoredSlots[0].Itemstack;
                    amountChanged += Math.Abs(MonitoredSlots[0].StackSize - oldQuantity);
                    UpdatePickupText();
                    oldQuantity = MonitoredSlots[0].StackSize;
                }
                if (MonitoredSlots[0].StackSize < oldQuantity)
                {
                    oldQuantity = MonitoredSlots[0].StackSize;
                }
            }
        }*/

        public void UpdatePickupText(int amount)
        { 
            quantity += amount;

                ResetTimer();
                ComposeGuis(Stack, posInList);

        }

        private void TimeOut(float dt)
        {
            TryClose();
            ClearComposers();
            capi.Event.UnregisterGameTickListener(tickId);
            //capi.Event.UnregisterGameTickListener(update);
            Dispose();
            foreach(GuiPickupMessage gui in capi.Gui.OpenedGuis.OfType<GuiPickupMessage>().Skip(posInList))
            {
                gui.ShiftDown();
            }
        }

        void ResetTimer()
        {
            capi.Event.UnregisterGameTickListener(tickId);
            tickId = capi.Event.RegisterGameTickListener(new Action<float>(TimeOut), 3000, 0);
        }

        public void ComposeGuis(ItemStack itemStack, float offsetMult)
        {
            bool needRecompose = false;
            string text = $"{quantity}x {itemStack.Collectible.GetHeldItemName(itemStack)}";

            ElementBounds iconBounds = ElementBounds.Fixed(EnumDialogArea.RightFixed, 0.0, -11.0, 10.0, 10.0);
            ElementBounds textBounds = ElementBounds.Fixed(EnumDialogArea.RightFixed, -44.0, 0.0, 500.0, 5.0);
			ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightBottom).WithFixedAlignmentOffset(0.0, offsetMult * -45.0f);

            GuiElementRichtext rtElem;
            LoadedTexture reuseRichTextTexture = null;
            if (composer == null)
            {
                composer = capi.Gui.CreateCompo($"pickupmessage{itemStack.Id}", dialogBounds);
            }
            else
            {
                rtElem = composer.GetRichtext("rt");
                reuseRichTextTexture = rtElem.richtTextTexture;
                rtElem.richtTextTexture = null;
                composer.Clear(dialogBounds);
                needRecompose = true;
            }

            composer.AddRichtext(text, PickupFont.StrokedWhiteText(), textBounds, "rt")
                .AddPassiveItemSlotNoText(iconBounds, null, dummySlot, false);

            Composers[$"pickupmessage{itemStack.Id}"] = composer;
            rtElem = composer.GetRichtext("rt");
            if (reuseRichTextTexture != null)
            {
                rtElem.richtTextTexture = reuseRichTextTexture;
            }
            rtElem.BeforeCalcBounds();
            textBounds.fixedWidth = Math.Min(500.0, rtElem.MaxLineWidth / (double)RuntimeEnv.GUIScale + 1.0);
            if (needRecompose)
                composer.ReCompose();
            else
                composer.Compose(true);
            TryOpen();
        }

        public void ShiftDown()
        {
            posInList--;
            ComposeGuis(Stack, posInList);
        }
    }
}
