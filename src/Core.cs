using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.Common;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using HarmonyLib;
using PickupMessages.Client.UI;

[assembly: ModInfo( "PickupMessages",
	Description = "A small mod that displays notifications whenever you pick up an item or block.",
	Authors     = new []{ "PeculiarSana" } )]

namespace PickupMessages
{
    public class Core : ModSystem
    {
        protected static ICoreClientAPI capi;
        protected static GuiPickupMessage guiPickupMessage;

        Harmony harmony = new Harmony("pickupmessages");

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            capi = api;

            //capi.Event.IsPlayerReady += Event_IsPlayerReady;
            harmony.PatchAll();
        }

        public override void Dispose()
        {
            harmony.UnpatchAll("pickupmessages");
            base.Dispose();
        }

        private bool Event_IsPlayerReady(ref EnumHandling handling)
        {

            //hotbar.SlotNotified += Event_HotbarSlotNotified;
            //backpack.SlotNotified += Event_BackpackSlotNotified;
            return default;
        }

        static void TryPickupMessage(ItemStack stack, int stackSize)
        {
            stack.StackSize = stackSize;
            IEnumerable<GuiPickupMessage> pickupMessagesList = capi.Gui.OpenedGuis.OfType<GuiPickupMessage>();
            GuiPickupMessage messageExists = pickupMessagesList.FirstOrDefault(item => item.Stack.Id == stack.Id);
            if (messageExists != null)
                messageExists.UpdatePickupText(stackSize);
            else
                new GuiPickupMessage(capi, stack);
        }

        [HarmonyPatch(typeof(PlayerInventoryManager), nameof (PlayerInventoryManager.TryGiveItemstack))]
        class TryGiveItemStack_Patch
        {
            static void Prefix(ref int __state, ItemStack itemstack, bool slotNotifyEffect = false)
            {
                __state = itemstack.StackSize;
            }

            static void Postfix(ref bool __result, int __state, ItemStack itemstack, bool slotNotifyEffect = false)
            {
                if (__result)
                {
                    capi.Event.EnqueueMainThreadTask(() => TryPickupMessage(itemstack, __state), "test");
                }
            }
        }
    }
}
