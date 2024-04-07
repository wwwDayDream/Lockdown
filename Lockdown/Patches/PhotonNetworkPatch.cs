using System;
using System.Reflection;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;

namespace Lockdown.Patches;

[HarmonyPatch(typeof(PhotonNetwork))]
public class PhotonNetworkPatch {
    [HarmonyPatch(nameof(PhotonNetwork.NetworkInstantiate), [ typeof(InstantiateParameters), typeof(bool), typeof(bool) ])]
    [HarmonyPrefix]
    private static bool InterceptInstantiatePrefix(ref InstantiateParameters parameters, bool roomObject, bool instantiateEvent)
    {
        #if DEBUG
        FileLog.Log($"[{(instantiateEvent ? "Incoming" : "Outgoing")}] Instantiate from {(parameters.creator.IsMasterClient ? "[M]" : "")}" +
                    $"{parameters.creator.NickName}: {parameters.prefabName}");
        #endif
        
        if (!instantiateEvent) return true; // We don't care about outgoing
        
        if (parameters.prefabName is "Player" or "PlayerData" or "ItemDataSyncer")
            return true; // Everyone has to use these 3
        
        if (parameters.prefabName is "PickupHolder" && parameters.creator.IsMasterClient) 
            return true; // Only the MasterClient is allowed to send this one to us
        
        if (parameters.creator.IsMasterClient) 
            return true; // Anything after this point should be stuff only master should spawn
        
        Lockdown.Logger.LogWarning($"Player {parameters.creator.NickName} is a big bad cheater! Blocking spawn for '{parameters.prefabName}'.");
        #if DEBUG
        FileLog.Log($"[X] Player {parameters.creator.NickName} is a big bad cheater! Blocking spawn for '{parameters.prefabName}'.");
        #endif
        return false;
    }
}