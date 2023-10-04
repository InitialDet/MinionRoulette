using Dalamud.Game;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Plugin.Services;

namespace MinionRoulette;
public class Service
{
    public static void Initialize(DalamudPluginInterface pluginInterface)
        => pluginInterface.Create<Service>();

    public const string PluginName = "MinionRoulette";

    [PluginService] public static DalamudPluginInterface PluginInterface { get; set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICondition Condition { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static ICommandManager Commands { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;

    public static Configuration Configuration { get; set; } = null!;
    public static WindowSystem WindowSystem { get; } = new WindowSystem(PluginName);
}

