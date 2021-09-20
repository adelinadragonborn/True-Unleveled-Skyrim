using System;
using System.Threading.Tasks;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

using TrueUnleveledSkyrim.Config;
using TrueUnleveledSkyrim.Patch;
using Mutagen.Bethesda.Plugins.Cache;

namespace TrueUnleveledSkyrim
{
    public class Patcher
    {
        public static Lazy<TUSConfig> ModSettings = null!;

        public static ILinkCache LinkCache { get; set; } = null!;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(nickname: "Mod Settings", path: "settings.json", out ModSettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "TUS.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            LinkCache = state.LoadOrder.PriorityOrder.ToImmutableLinkCache();

            if (ModSettings.Value.Unleveling.UnlevelGame)
            {
                LeveledItemsPatcher.PatchLVLI(state);
                OutfitsPatcher.PatchOutfits(state);
                NPCsPatcher.PatchNPCs(state);
            }

            if (ModSettings.Value.ItemAdjustments.MorrowlootifyItems)
                ItemsPatcher.PatchItems(state);
        }
    }
}