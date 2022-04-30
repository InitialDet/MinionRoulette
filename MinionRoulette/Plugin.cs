using Dalamud.Game;
using Dalamud.Plugin;
using System;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Game.ClientState.Conditions;
using System.Linq;
using Dalamud.Game.ClientState;
using Dalamud.Logging;
using System.Threading.Tasks;

namespace MinionRoulette {
    // Based on the FishNotify plugin
    public sealed class Plugin : IDalamudPlugin {
        public string Name => "Minion Roulette";

        readonly CurrentZone currentZone;

        public Plugin(DalamudPluginInterface pluginInterface) {
            Service.Initialize(pluginInterface);

            currentZone = new();
            Service.ClientState!.TerritoryChanged -= currentZone.TerritoryChanged;
            currentZone.Init();
        }

        public void Dispose() {
            PluginLog.Debug("Disposed");
            currentZone.Dispose();
        }

        public class CurrentZone : IDisposable {
            public ushort lastZoneID;

            readonly CommandManager _commandManager;
            readonly Lumina.Excel.ExcelSheet<GeneralAction> _generalActionSheet;
            readonly Lumina.Excel.ExcelSheet<TerritoryType> _territoriesList;

            const uint idMinionRoulette = 10;   //GeneralAction

            public CurrentZone() {
                _commandManager = new CommandManager(Service.SigScanner);
                _territoriesList = Service.GameData.GetExcelSheet<TerritoryType>()!;
                _generalActionSheet = Service.GameData.GetExcelSheet<GeneralAction>()!;
            }

            public void Init() {
                Service.ClientState!.TerritoryChanged += TerritoryChanged;
            }
            public void TerritoryChanged(object? sender, ushort zoneID) {
                if (zoneID != lastZoneID) {
                    PluginLog.Debug("Zone Change, New Zone");
                    SwapMinion(zoneID);
                } else
                    PluginLog.Debug("Zone Change, Same Zone");
            }

            private async void SwapMinion(ushort zoneID) {
                while (this.lastZoneID != zoneID) {
                    await Task.Delay(500); //Theres no need to loop like crazy while waiting for the game to load.
                    if ((Service.ClientState.LocalContentId == 0 && Service.ClientState.LocalPlayer == null) || BetweenAreas()) {
                        PluginLog.Debug("Loading not finished...");
                        continue;
                    }

                    if (BoundByDuty()) { // Dont proceed if player is Bound By Duty
                        PluginLog.Debug("Player in duty: dont proceed");
                        this.lastZoneID = zoneID;
                        return;
                    }

                    if (BetweenAreas()) { // Recheking because the BetweenAreas() above can be inconsistent, not sure why.
                        PluginLog.Debug("Loading not finished...");
                        continue;
                    }

                    var minionRoulette = _generalActionSheet?.GetRow(idMinionRoulette)?.Name;
                    _commandManager.Execute($"/ac \"{minionRoulette}\"");

                    PluginLog.Debug("SwapMinion, Executed");
                    this.lastZoneID = zoneID;
                };
            }

            public static bool BetweenAreas() =>
                Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51];

            public static bool BoundByDuty() =>
                Service.Condition[ConditionFlag.BoundByDuty];

            public void Dispose() {
                Service.ClientState!.TerritoryChanged -= TerritoryChanged;
            }
        }
    }
}
