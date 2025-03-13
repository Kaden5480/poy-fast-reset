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
        private class MenuState {
            public bool didOverwrite { get; }
            public bool restoreState { get; }

            public MenuState(bool didOverwrite, bool restoreState) {
                this.didOverwrite = didOverwrite;
                this.restoreState = restoreState;
            }
        }

        private static UI.Window ui {
            get => Plugin.instance.ui;
        }

        static void Prefix(Inventory __instance, ref MenuState __state) {
            if (ui.showingUI == true) {
                __state = new MenuState(true, InGameMenu.isCurrentlyNavigationMenu);
                InGameMenu.isCurrentlyNavigationMenu = true;
            }
            else {
                __state = new MenuState(false, InGameMenu.isCurrentlyNavigationMenu);
            }
        }

        static void Postfix(MenuState __state) {
            if (__state.didOverwrite == true) {
                InGameMenu.isCurrentlyNavigationMenu = __state.restoreState;
            }
        }
    }
}
