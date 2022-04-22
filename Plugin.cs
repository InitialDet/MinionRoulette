using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using Dalamud.Game.Gui;
using Lumina.Excel.GeneratedSheets;
using Dalamud.Game.ClientState.Conditions;
using System.Linq;
using Dalamud.Game.ClientState;
using FFXIVClientStructs.FFXIV.Common;

namespace MinionRoulette {
    // Based on the FishNotify plugin
    public sealed class Plugin : IDalamudPlugin {
        public string Name => "Minion Roulette";

        [PluginService] [RequiredVersion("1.0")] private DalamudPluginInterface PluginInterface { get; set; }
        [PluginService] [RequiredVersion("1.0")] public static SigScanner SigScanner { get; private set; } = null!;
        [PluginService] [RequiredVersion("1.0")] public static ChatGui Chat { get; private set; } = null!;
        [PluginService] [RequiredVersion("1.0")] public static ClientState ClientState { get; private set; } = null!;
        [PluginService] [RequiredVersion("1.0")] public static DataManager GameData { get; private set; } = null!;
        [PluginService] [RequiredVersion("1.0")] public static Framework Framework { get; private set; } = null!;
        [PluginService] [RequiredVersion("1.0")] public static Dalamud.Game.ClientState.Conditions.Condition Condition { get; private set; } = null!;
        [PluginService][RequiredVersion("1.0")] public static Dalamud.Game.ClientState.Buddy.BuddyList Buddy { get; private set; } = null!;


        public CommandManager _commandManager;


        private readonly Lumina.Excel.ExcelSheet<GeneralAction> _generalActionSheet;
        private readonly Lumina.Excel.ExcelSheet<TerritoryType> _territoriesList;

   

        
        uint idMinionRoulette = 10;   //GeneralAction

        public Plugin() {
            _commandManager = new CommandManager(SigScanner);
            PluginInterface!.UiBuilder.Draw += OnDrawUI;
            PluginInterface!.UiBuilder.OpenConfigUi += OnOpenConfigUi;
            Framework.Update += UpdateLocation;
            _territoriesList = GameData.GetExcelSheet<TerritoryType>()!;
            _generalActionSheet = GameData.GetExcelSheet<GeneralAction>()!;
        }

        public void Dispose() {
            Framework.Update -= this.UpdateLocation;
            PluginInterface!.UiBuilder.Draw -= OnDrawUI;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        }

        public string playerZone = "";
        public string lastTerritoryName = "";
        public string lastTerritoryRegion = "";
      
        private void UpdateLocation(Framework framework) {
           
            try {

                if (ClientState.LocalContentId != 0 && ClientState.LocalPlayer != null && BetweenAreas()) {
                    return;
                }

                var territoryId = ClientState.TerritoryType;
                string territoryName;
                string territoryRegion;
               
                if (territoryId != 0) {
                    var territory = _territoriesList.First(Row => Row.RowId == territoryId);
                    territoryName = new string(territory.PlaceName.Value?.Name ?? "???");
                    territoryRegion = new string(territory.PlaceNameRegion.Value?.Name ?? "The Void");

                } else {
                    return;
                }

                if (this.lastTerritoryName.Equals(territoryName) && this.lastTerritoryRegion.Equals(territoryRegion)) {         
                    return;
                }
                
                if (!BetweenAreas()) {
                    SwapMinion();

                    this.lastTerritoryName = new string(territoryName);
                    this.lastTerritoryRegion = new string(territoryRegion);
                   
                }

            } catch (Exception ex) {
            }
        }

        public static bool BetweenAreas()
        => Condition[ConditionFlag.BetweenAreas]
         || Condition[ConditionFlag.BetweenAreas51]
            || Condition[ConditionFlag.BoundByDuty];

        private void SwapMinion() {
            try {
                var minionRoulette = _generalActionSheet?.GetRow(idMinionRoulette)?.Name;
                _commandManager.Execute($"/ac \"{minionRoulette}\"");
               
            } catch (Exception) { }  
        }

        private void OnDrawUI() {
            
            if (true)
                return;
        }
        private void OnOpenConfigUi() {
            
        }
    }
}
