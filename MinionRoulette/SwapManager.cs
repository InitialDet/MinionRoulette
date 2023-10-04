using System;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace MinionRoulette;
public sealed partial class Plugin
{
    public class SwapManager : IDisposable
    {
        public ushort lastZone;

        const uint idMinionRoulette = 10;   //GeneralAction

        public void Init()
        {
            Service.ClientState!.TerritoryChanged += TerritoryChanged;
        }

        public void TerritoryChanged(ushort currentZone)
        {

            if (!Service.Configuration.PluginEnabled) {
                lastZone = currentZone;
                return;
            }

            if (InvalidPlaces(currentZone))
            {
                lastZone = currentZone;
                return;
            }

            if (currentZone != lastZone)
                SwapMinion(currentZone);
        }

        private static bool InvalidPlaces(int zoneID)
        {
            return zoneID switch
            {
                1055 => true, // Island Sanctuary
                _ => false,
            };
        }

        private async void SwapMinion(ushort zoneID)
        {
            
            int trys = 0;
            
            while (this.lastZone != zoneID)
            {
                await Task.Delay(500); //Theres no need to loop like crazy while waiting for the game to load.
                if ((Service.ClientState.LocalContentId == 0 && Service.ClientState.LocalPlayer == null) || BetweenAreas())
                    continue;

                if (BoundByDuty()) // Dont proceed if player is Bound By Duty
                {
                    this.lastZone = zoneID;
                    return;
                }

                if (BetweenAreas()) // Recheking because the BetweenAreas() above can be inconsistent, not sure why.
                    continue;

                if (!IsMinionSummoned())
                {
                    this.lastZone = zoneID;
                    break;
                }

                if (ActionAvailable(idMinionRoulette))
                {
                    if (CastAction(idMinionRoulette)) {
                        this.lastZone = zoneID;
                        break;
                    }
                }      

                trys++;
                if (trys > 10)
                    break;
            }
        }

        public static unsafe bool CastAction(uint id, ActionType actionType = ActionType.GeneralAction)
        {
            try
            {
                return ActionManager.Instance()->UseAction(actionType, id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static unsafe bool ActionAvailable(uint id)
        {
            try
            {
                return ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, id) == 0 && !ActionManager.Instance()->IsRecastTimerActive(ActionType.Action, id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool BetweenAreas() =>
            Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51];

        public static bool BoundByDuty() =>
            Service.Condition[ConditionFlag.BoundByDuty];

        public void Dispose()
        {
            Service.ClientState!.TerritoryChanged -= TerritoryChanged;
        }

        // Ty Pohky for helping me with this
        private static bool IsMinionSummoned()
        {
            try
            {
                var isSummoned = false;

                var list = Service.ObjectTable;
                
                GameObject? player = Service.ObjectTable.OfType<Character>().FirstOrDefault();

                if (player == null || list == null)
                    return isSummoned;
                
                for (var i = 0; i < list.Length - 1; i++)
                {
                    if (list[i]?.Address != player?.Address)
                        continue;

                    var obj = list[i + 1]; // If the player adress is found, the next obj should be the minion
                    isSummoned = (obj != null && obj.ObjectKind == ObjectKind.Companion);
                    break;
                }

                return isSummoned;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
