using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;

namespace MinionRoulette;

public sealed partial class Plugin : IDalamudPlugin
{
    public string Name => Service.PluginName;

    private const string cmdMrCfg = "/minionroulette";
    private const string cmdMrCfgShort = "/mrcfg";
    private const string cmdMrToggle  = "/mrtoggle";

    private readonly SwapManager currentZone;
    private static PluginUI PluginUI = null!;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.Configuration = Configuration.Load();
        PluginUI = new PluginUI();
        Service.PluginInterface!.UiBuilder.Draw += Service.WindowSystem.Draw;
        Service.PluginInterface!.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        Service.Commands.AddHandler(cmdMrToggle, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles MinionRoulette"
        });

        Service.Commands.AddHandler(cmdMrCfgShort, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Config Window"

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
        if (command.Trim().Equals(cmdMrCfg) || command.Trim().Equals(cmdMrCfgShort)) {
            OnOpenConfigUi();
            return;
        }

        if (command.Trim().Equals(cmdMrToggle))
        {
            if (Service.Configuration.PluginEnabled)
            {
                Service.Chat.Print("MinionRoulette Disabled");
                Service.Configuration.PluginEnabled = false;
            }
            else
            {
                Service.Chat.Print("MinionRoulette Enabled");
                Service.Configuration.PluginEnabled = true;
            }

            return;
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
        Service.Commands.RemoveHandler(cmdMrCfgShort);
        Service.Commands.RemoveHandler(cmdMrToggle);
    }
}