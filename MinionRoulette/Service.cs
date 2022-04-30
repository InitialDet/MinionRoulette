using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Data;
using Dalamud.Plugin;
using Dalamud.Game.Gui;
using Dalamud.Game.ClientState;

namespace MinionRoulette {
    public class Service {
        public static void Initialize(DalamudPluginInterface pluginInterface)
            => pluginInterface.Create<Service>();

        [PluginService][RequiredVersion("1.0")] public static DalamudPluginInterface PluginInterface { get; set; }
        [PluginService][RequiredVersion("1.0")] public static SigScanner SigScanner { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static ChatGui Chat { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static ClientState ClientState { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static DataManager GameData { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static Framework Framework { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static Dalamud.Game.ClientState.Conditions.Condition Condition { get; private set; } = null!;

    }
}
