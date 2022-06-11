using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace MinionRoulette;
public sealed partial class Plugin
{
    public class MinionSwap : IDisposable
    {
        public ushort lastZoneID;

        private bool disposed = false;

        const uint idMinionRoulette = 10;   //GeneralAction

        public void Init()
        {
            Service.ClientState!.TerritoryChanged += TerritoryChanged;
            disposed = false;
        }

        public void TerritoryChanged(object? sender, ushort zoneID)
        {
            if (!Service.Configuration.PluginEnabled)
                return;

            if (zoneID != lastZoneID)
            {
                PluginLog.Debug("Zone Change, New Zone");
                SwapMinion(zoneID);
            }
            else
                PluginLog.Debug("Zone Change, Same Zone");
        }

        private async void SwapMinion(ushort zoneID)
        {
            int trys = 0;
            while ((this.lastZoneID != zoneID) && !disposed)
            {
                await Task.Delay(200); //Theres no need to loop like crazy while waiting for the game to load.
                if ((Service.ClientState.LocalContentId == 0 && Service.ClientState.LocalPlayer == null) || BetweenAreas())
                {
                    continue;
                }

                if (BoundByDuty()) // Dont proceed if player is Bound By Duty
                {
                    this.lastZoneID = zoneID;
                    return;
                }

                if (BetweenAreas()) // Recheking because the BetweenAreas() above can be inconsistent, not sure why.
                {
                    continue;
                }

                if (!IsMinionSummoned())
                {
                    this.lastZoneID = zoneID;
                    break;
                }

                if (ActionAvailable(idMinionRoulette))
                {
                    if (CastAction(idMinionRoulette)) {
                        this.lastZoneID = zoneID;
                        break;
                    }
                }      

                trys++;
                if (trys > 10)
                    break;
            }
        }

        public static unsafe bool CastAction(uint id, ActionType actionType = ActionType.General)
        {
            return ActionManager.Instance()->UseAction(actionType, id);
        }

        public static unsafe bool ActionAvailable(uint id)
        {
            // status 0 == available to cast? not sure but it seems to be
            // Also make sure its the skill is not on cooldown (maily for mooch2)
            return ActionManager.Instance()->GetActionStatus(ActionType.General, id) == 0 && !ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, id);
        }

        public static bool BetweenAreas() =>
            Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51];

        public static bool BoundByDuty() =>
            Service.Condition[ConditionFlag.BoundByDuty];

        public void Dispose()
        {
            disposed = true;
            Service.ClientState!.TerritoryChanged -= TerritoryChanged;
        }

        // Credits to Pohky (from goat place) for helping me with this
        private static bool IsMinionSummoned()
        {
            try
            {
                bool isSummoned = false;

                ObjectTable list = Service.ObjectTable;
                GameObject? player = null;

                foreach (var obj in Service.ObjectTable)
                {
                    if (obj is Character)
                    {
                        player = obj;
                        break;
                    }
                }

                if (player != null && list != null)
                {
                    for (var i = 0; i < list.Length - 1; i++)
                    {
                        if (list[i]?.Address != player?.Address)
                            continue;

                        var obj = list[i + 1]; // If the player adress is found, the next obj should be the minion
                        isSummoned = (obj != null && obj.ObjectKind == ObjectKind.Companion);
                        break;
                    }
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
