using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;

namespace MinionRoulette;
public class Service
{
    public static void Initialize(DalamudPluginInterface pluginInterface)
        => pluginInterface.Create<Service>();

    public const string PluginName = "MinionRoulette";

    [PluginService][RequiredVersion("1.0")] public static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static ClientState ClientState { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static Dalamud.Game.ClientState.Conditions.Condition Condition { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static ObjectTable ObjectTable { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static CommandManager Commands { get; private set; } = null!;
    [PluginService][RequiredVersion("1.0")] public static ChatGui Chat { get; private set; } = null!;

    public static Configuration Configuration { get; set; } = null!;
    public static WindowSystem WindowSystem { get; } = new WindowSystem(PluginName);
}

