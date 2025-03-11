using HarmonyLib;

namespace FastReset.Patches {
    /**
     * <summary>
     * Prevents the use of inventory when the UI is open.
     * This disables stuff like coffee, bird seeds, and chalk.
     * </summary>
     */
    [HarmonyPatch(typeof(Inventory), "Update")]
    static class InventoryFix {
        //private static UI.Window ui {
        //    get => Plugin.instance.ui;
        //}

        static void Prefix(Inventory __instance, ref bool __state) {
            //__state = InGameMenu.isCurrentlyNavigationMenu;
            //InGameMenu.isCurrentlyNavigationMenu = ui.showingUI;
        }

        static void Postfix(bool __state) {
            //InGameMenu.isCurrentlyNavigationMenu = __state;
        }
    }
}
