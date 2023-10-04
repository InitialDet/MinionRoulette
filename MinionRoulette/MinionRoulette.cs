﻿using Dalamud.Game.Command;
using Dalamud.Plugin;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace MinionRoulette;

public class Plugin : IDalamudPlugin
{
    public static string Name => Service.PluginName;

    private const string CmdMrCfg = "/minionroulette";
    private const string CmdMrCfgShort = "/mrcfg";
    private const string CmdMrToggle  = "/mrtoggle";

    private readonly SwapManager _currentZone;
    private static PluginUi _pluginUi = null!;

    public Plugin(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.Configuration = Configuration.Configuration.Load();
        _pluginUi = new PluginUi();
        Service.PluginInterface.UiBuilder.Draw += Service.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;

        Service.Commands.AddHandler(CmdMrToggle, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggles MinionRoulette"
        });

        Service.Commands.AddHandler(CmdMrCfgShort, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Config Window"

        });

        Service.Commands.AddHandler(CmdMrCfg, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Config Window"
        });
        _currentZone = new SwapManager();
        _currentZone.Init();
    }

    private static void OnCommand(string command, string args)
    {
        switch (command.Trim())
        {
            case CmdMrCfg:
            case CmdMrCfgShort:
                OnOpenConfigUi();
                return;
            case CmdMrToggle when Service.Configuration.PluginEnabled:
                Service.Chat.Print("MinionRoulette Disabled");
                Service.Configuration.PluginEnabled = false;
                break;
            case CmdMrToggle:
                Service.Chat.Print("MinionRoulette Enabled");
                Service.Configuration.PluginEnabled = true;
                break;
        }
    }

    private static void OnOpenConfigUi() => _pluginUi.Toggle();

    public void Dispose()
    {
        _pluginUi.Dispose();
        _currentZone.Dispose();

        Service.Configuration.Save();
        Service.PluginInterface.UiBuilder.Draw -= Service.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Commands.RemoveHandler(CmdMrCfg);
        Service.Commands.RemoveHandler(CmdMrCfgShort);
        Service.Commands.RemoveHandler(CmdMrToggle);
    }
}