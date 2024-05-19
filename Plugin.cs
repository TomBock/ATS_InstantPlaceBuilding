using BepInEx;
using BepInEx.Logging;
using Cysharp.Threading.Tasks;
using Eremite;
using Eremite.Buildings;
using Eremite.View.HUD;
using Eremite.View.HUD.Construction;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace PlaceBuildingsInstantly
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log => Instance.Logger;

        public static Plugin Instance;
        private Harmony harmony;

        private bool _removeBuilding;
        private Building _cachedBuildingToRemove;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));  
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        
        [HarmonyPatch(typeof(BuildingsPanel), nameof(BuildingsPanel.PlacingFinished))]
        [HarmonyPrefix]
        private static bool HookPlacingFinishedPrefix(BuildingsPanel __instance)
        {
            Instance._removeBuilding = false;
            
            if (__instance.currentRequest is not BuildingRequest buildingRequest)
                return true;
            
            var building = buildingRequest.currentBuilding;

            if (!Keyboard.current.ctrlKey.isPressed)
                return true;
            
            Log.LogInfo($"Skipping construction process for building {building}");
            
            _ = new BuildingCreator().CreateCompletedBuilding(
                building.BuildingModel,
                building.BuildingState.field, 
                building.BuildingState.rotation);

            Instance._removeBuilding = true;
            Instance._cachedBuildingToRemove = building;
            return true;
        }
        
        [HarmonyPatch(typeof(BuildingsPanel), nameof(BuildingsPanel.PlacingFinished))]
        [HarmonyPostfix]
        private static void HookPlacingFinishedPostfix()
        {
            if (Instance._removeBuilding)
            {
                Instance._cachedBuildingToRemove?.Remove(false);
            }
        }
    }
}
