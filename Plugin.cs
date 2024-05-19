using BepInEx;
using Eremite.Buildings;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace PlaceBuildingsInstantly
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        private Harmony harmony;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));  
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(Building), "PlaceBuilding")]
        [HarmonyPostfix]
        private static void HookPlaceBuilding(Building __instance)
        {
            if (!Keyboard.current.ctrlKey.isPressed)
                return;

            Instance.Logger.LogInfo($"Skippping construction process for building {__instance}");
            
            _ = new BuildingCreator().CreateCompletedBuilding(
                __instance.BuildingModel,
                __instance.BuildingState.field, 
                __instance.BuildingState.rotation);
            __instance.Remove(false);
        }
    }
}
