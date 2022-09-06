using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace MinionRoulette;
// Based on the FishNotify plugin
public sealed partial class Plugin : IDalamudPlugin
{
    public string Name => Service.PluginName;

    private const string cmdMrCfg = "/mrcfg";
    private const string cmdMrOn  = "/mron";
    private const string cmdMrOff = "/mroff";

    readonly MinionSwap currentZone;
    private static PluginUI PluginUI = null!;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.Configuration = Configuration.Load();
        PluginUI = new PluginUI();
        Service.PluginInterface!.UiBuilder.Draw += Service.WindowSystem.Draw;
        Service.PluginInterface!.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        Service.Commands.AddHandler(cmdMrOff, new CommandInfo(OnCommand)
        {
            HelpMessage = "Disables MinionRoulette",
        });

        Service.Commands.AddHandler(cmdMrOn, new CommandInfo(OnCommand)
        {
            HelpMessage = "Enables MinionRoulette"
        });

        Service.Commands.AddHandler(cmdMrCfg, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Config Window"
        });
        currentZone = new();
        currentZone.Init();
    }

    private void OnCommand(string command, string args)
    {
        if (command.Trim().Equals(cmdMrCfg)) {
            OnOpenConfigUi();
        }

        if (command.Trim().Equals(cmdMrOn))
        {
            Service.Chat.Print("MinionRoulette Enabled");
            Service.Configuration.PluginEnabled = true;
        }

        if (command.Trim().Equals(cmdMrOff))
        {
            Service.Chat.Print("MinionRoulette Disabled");
            Service.Configuration.PluginEnabled = false;
        }
    }

    private void OnOpenConfigUi() => PluginUI.Toggle();

    public void Dispose()
    {
        PluginUI.Dispose();
        currentZone.Dispose();

        Service.Configuration.Save();
        Service.PluginInterface!.UiBuilder.Draw -= Service.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Commands.RemoveHandler(cmdMrCfg);
        Service.Commands.RemoveHandler(cmdMrOn);
        Service.Commands.RemoveHandler(cmdMrOff);
    }

   
}