using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;

namespace LMSH.Minikit.Codes;

public class Game
{
    // Used to check if the game menu is loaded before connecting and trying to set up hooks
    public static void IsGameLoaded()
    {
        PrintToLog("Checking to see if save file is loaded");
        int rewriteNumber = 0;
        while (!IsMenuLoaded())
        {
            if (rewriteNumber % 10 == 0)
                PrintToLog("Waiting for save file to load");
            rewriteNumber++;
            System.Threading.Thread.Sleep(500);
        }
        PrintToLog("Game Loaded");
        Mod.GameInstance!.SetupHooks(Mod._hooks!);
    }

    // Helper Function to check if menu is loaded or player is controllable
    public static unsafe bool IsMenuLoaded()
    {
        try
        {
            byte** P1Health = (byte**)(Mod.BaseAddress + 0x15B087C);
            if (P1Health == null || *P1Health == null)
                return false;

            byte* finalPtr = *P1Health + 0x609;
            if (finalPtr == null)
                return false;

            return *finalPtr != 0;
        }
        catch (Exception ex)
        {
            PrintToLog($"Error in checking if menu is loaded: {ex.Message}");
            return false;
        }
    }

    public static void PrintToLog(string message)
    {
        Mod.Logger?.WriteLineAsync("[LMSH.Minikit.Codes] " + message);
    }

    private static unsafe byte* MinikitCodeBaseAddress => (byte*)(Mod.BaseAddress + 0x15AD988);

    public static unsafe void ReadMinikitValue(int minikitNum)
    {
        byte* mostRecentMinikit = MinikitCodeBaseAddress + (minikitNum * 0x24);
        string currentCode = new((sbyte*)mostRecentMinikit); // Read the current message from memory
        int MaxLength = 30;

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
        PrintToLog($"Minikit Code: {currentCode}");
    }

    public static List<IAsmHook> _asmHooks = [];
    private static IReverseWrapper<IncreaseMinikitCount> _reverseWrapOnIncreaseMinikitCount = default!;
    public void SetupHooks(IReloadedHooks hooks)
    {
        string[] completeLevelHook =
        {
            "use32",
            "pushfd",
            "pushad",
            $"{hooks.Utilities.GetAbsoluteCallMnemonics(OnIncreaseMinikitCount, out _reverseWrapOnIncreaseMinikitCount)}",
            "popad",
            "popfd",
        };
        _asmHooks.Add(hooks.CreateAsmHook(completeLevelHook, (int)(Mod.BaseAddress + 0x916774), AsmHookBehaviour.ExecuteAfter).Activate());
    }

    [Function([FunctionAttribute.Register.eax],
    FunctionAttribute.Register.eax, FunctionAttribute.StackCleanup.Callee)]
    public delegate void IncreaseMinikitCount(nuint eax);

    private static unsafe void OnIncreaseMinikitCount(nuint eax)
    {
        int minikitCount = *(byte*)(eax + 0x14);
        PrintToLog($"Total Minikits collected: {minikitCount}");
        minikitCount -= 1; // Adjust for pointer arthimatic
        ReadMinikitValue(minikitCount);
    }
}