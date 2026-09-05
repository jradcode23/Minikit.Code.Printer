using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;

namespace LPotC.Minikit.Codes;

public class Game
{
    private uint MapID = 0;
    private string MapName = string.Empty;
    // Used to check if the game menu is loaded before connecting and trying to set up hooks
    public static unsafe void IsGameLoaded()
    {
        PrintToLog("Checking to see if save file is loaded");
        int rewriteNumber = 0;
        while (!IsPlayerLoaded())
        {
            if (rewriteNumber % 10 == 0)
                PrintToLog("Waiting for save file to load");
            rewriteNumber++;
            Thread.Sleep(500);
        }
        PrintToLog("Game Loaded");
        SetupHooks(Mod._hooks!);
        byte* n0CutFlag = *(byte**)(Mod.BaseAddress + 0xA39FB8) + 0xA0;
        if (n0CutFlag == null)
        {
            PrintToLog("NoCut Flag is null");
            return;
        }
        *n0CutFlag = 1;
        byte* mapID = (byte*)(Mod.BaseAddress + 0xB791B4);
        if (mapID == null)
        {
            PrintToLog($"Cannot Update Map ID. Map ID is null");
            return;
        }
        Mod.GameInstance!.MapID = *mapID;
        PrintToLog($"Initial Map ID is: {Mod.GameInstance!.MapID}");
    }

    // Checks to see if the Player has loaded in
    private static unsafe bool IsPlayerLoaded()
    {
        try
        {
            byte** P1Health = (byte**)(Mod.BaseAddress + 0xB6DE18);
            if (P1Health == null || *P1Health == null)
                return false;

            byte* finalPtr = *P1Health + 0xE26;
            if (finalPtr == null)
                return false;

            return finalPtr != null; // Sometimes this is loading early
        }
        catch (Exception ex)
        {
            PrintToLog($"Error in checking if menu is loaded: {ex.Message}");
            return false;
        }
    }

    private static void PrintToLog(string message)
    {
        Mod.Logger?.WriteLineAsync("[LPotC.Minikit.Codes] " + message);
    }

    private static unsafe byte* MinikitCodeBaseAddress => (byte*)(Mod.BaseAddress + 0xB7B598);

    private static unsafe void ReadMinikitValue(uint minikitNum)
    {
        if (MinikitCodeBaseAddress == null)
        {
            PrintToLog("Please report to a Dev. Minikit Address is Null");
            return;
        }
        byte* mostRecentMinikit = MinikitCodeBaseAddress + (minikitNum * 0x24);
        string currentCode = new((sbyte*)mostRecentMinikit); // Read the current message from memory
        int MaxLength = 24;

        if (string.IsNullOrEmpty(currentCode))
        {
            PrintToLog("Minikit Code has a null value");
            return;
        }

        if (currentCode.Length > MaxLength)
        {
            PrintToLog("Unexpected Behavior, code exceeded max length");
            return;
        }
        PrintToLog($"Minikit Code: {currentCode}. Map ID is: {Mod.GameInstance!.MapID}. Map Name is: {Mod.GameInstance!.MapName}");
    }

    private static unsafe void ClearMinikitCodes()
    {
        if (MinikitCodeBaseAddress == null)
        {
            PrintToLog("Please report to a Dev. Minikit Address is Null");
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            byte* minikitCode = MinikitCodeBaseAddress + (i * 0x24);
            *minikitCode = 0x0;
        }
    }

    private static List<IAsmHook> AsmHooks = [];
    private static IReverseWrapper<IncreaseMinikitCount> _reverseWrapOnIncreaseMinikitCount = default!;
    private static IReverseWrapper<UpdateMapID> _reverseWrapOnUpdatedMapID = default!;
    private static IReverseWrapper<UpdateMapName> _reverseWrapOnUpdatedMapName = default!;
    public static void SetupHooks(IReloadedHooks hooks)
    {
        string[] minikitCountIncreaseHook =
        {
            "use32",
            "pushfd",
            "pushad",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnIncreaseMinikitCount, out _reverseWrapOnIncreaseMinikitCount)}",
            "popad",
            "popfd",
        };
        AsmHooks.Add(hooks.CreateAsmHook(minikitCountIncreaseHook, (int)(Mod.BaseAddress + 0x41BC69), AsmHookBehaviour.ExecuteAfter).Activate()); // Note to self since there are multiple minikit counters: This writes to LEGOPirates.exe+B6B198

        string[] updateMapIDHook =
        {
            "use32",
            "pushfd",
            "pushad",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnUpdateMapID, out _reverseWrapOnUpdatedMapID)}",
            "popad",
            "popfd",
        };
        AsmHooks.Add(hooks.CreateAsmHook(updateMapIDHook, (int)(Mod.BaseAddress + 0x3B1E3A), AsmHookBehaviour.ExecuteFirst).Activate()); // cause of the call, doesn't work if you run after

        string[] updateMapNameHook =
        {
            "use32",
            "pushfd",
            "pushad",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnUpdateMapName, out _reverseWrapOnUpdatedMapName)}",
            "popad",
            "popfd",
        };
        AsmHooks.Add(hooks.CreateAsmHook(updateMapNameHook, (int)(Mod.BaseAddress + 0x4AD683), AsmHookBehaviour.ExecuteFirst).Activate()); // cause of the call, doesn't work if you run after
    }

    [Function([FunctionAttribute.Register.eax],
    FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
    public delegate void IncreaseMinikitCount(uint eax);

    private static void OnIncreaseMinikitCount(uint eax)
    {
        uint minikitCount = eax;
        PrintToLog($"Total Minikits collected: {minikitCount}");
        minikitCount -= 1; // Adjust for pointer arithmetic
        ReadMinikitValue(minikitCount);
    }

    [Function([FunctionAttribute.Register.eax, FunctionAttribute.Register.ecx],
    FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
    public delegate void UpdateMapID(uint eax, uint ecx);

    private static void OnUpdateMapID(uint eax, uint ecx)
    {
        PrintToLog($"Level ID Updated to: {eax}. Map ID Updated to: {ecx}");
        Mod.GameInstance!.MapID = ecx;
        if (ecx == 0x2)
        {
            PrintToLog("Hub Detected, clearing minikit codes");
            ClearMinikitCodes();
        }
    }

    [Function([FunctionAttribute.Register.eax],
    FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
    public delegate void UpdateMapName(uint eax);

    private static unsafe void OnUpdateMapName(uint eax)
    {
        byte* FirstPointer = *(byte**)(Mod.BaseAddress + 0xB79178);
        if (FirstPointer == null)
        {
            PrintToLog("Map Name Pointer 1 is null");
            return;
        }
        byte* SecondPointer = *(byte**)(FirstPointer + 0x48 + eax);
        // PrintToLog($"Update Map Name Address is 0x{(uint)SecondPointer:X}");
        if (SecondPointer == null)
        {
            PrintToLog("Map Name Pointer 2 is null");
            return;
        }
        string mapName = new((sbyte*)SecondPointer);
        if (string.IsNullOrEmpty(mapName))
        {
            PrintToLog("Map Name is null or empty");
            return;
        }
        if (mapName.Length > 32)
        {
            PrintToLog("Unexpected Behavior, map name exceeded max length");
            return;
        }
        PrintToLog($"Map Name Updated to: {mapName}");
        Mod.GameInstance!.MapName = mapName;
    }
}