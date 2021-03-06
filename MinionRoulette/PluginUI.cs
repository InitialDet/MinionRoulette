using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using Dalamud.Interface.Colors;
using System.Diagnostics;

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

        ShowKofi();
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

    public static void ShowKofi()
    {
        string buttonText = "Support on Ko-fi";
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);

        if (ImGui.Button(buttonText))
        {
            Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/initialdet", UseShellExecute = true });
        }

        ImGui.PopStyleColor(3);
    }
}

