using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace MinionRoulette;

public class Service
{
    public const string PluginName = "MinionRoulette";

    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ICondition Condition { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static ICommandManager Commands { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;

    public static Configuration.Configuration Configuration { get; set; } = null!;
    public static WindowSystem WindowSystem { get; } = new(PluginName);

    public static void Initialize(DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
    }
}