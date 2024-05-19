using Eremite.Buildings;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace PlaceBuildingsInstantly;

public class BuildingHooks
{
    [HarmonyPatch(typeof(Building), "PlaceBuilding")]
    [HarmonyPostfix]
    private static void HookPlaceBuilding(Building __instance)
    {
        if (!Keyboard.current.ctrlKey.isPressed)
            return;

        Plugin.Log.LogInfo($"Skippping construction process for building {__instance}");
            
        _ = new BuildingCreator().CreateCompletedBuilding(
            __instance.BuildingModel,
            __instance.BuildingState.field, 
            __instance.BuildingState.rotation);
        __instance.Remove(false);
    }
}
