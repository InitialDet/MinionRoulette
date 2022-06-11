using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using Dalamud.Interface.Colors;

namespace MinionRoulette;
public class PluginUI : Window, IDisposable
{
    public PluginUI() : base($"{Service.PluginName} Settings")
    {
        Service.WindowSystem.AddWindow(this);

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    public void Dispose()
    {
        Service.Configuration.Save();
        Service.WindowSystem.RemoveWindow(this);
    }

    public override void Draw()
    {
        if (!IsOpen)
            return;

        ImGui.Checkbox("Enable MinionRoulette", ref Service.Configuration.PluginEnabled);

        if (Service.Configuration.PluginEnabled)
            ImGui.TextColored(ImGuiColors.HealerGreen, "MinionRoulette Enabled");
        else
            ImGui.TextColored(ImGuiColors.DalamudRed, "MinionRoulette Disabled");

        ImGui.Spacing();

        ImGui.End();
    }

    public override void OnClose()
    {
        Service.Configuration.Save();
    }
}

