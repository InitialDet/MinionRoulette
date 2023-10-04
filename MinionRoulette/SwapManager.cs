using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace MinionRoulette;

public class SwapManager : IDisposable
{
    private const uint IdMinionRoulette = 10; //GeneralAction
    private ushort _lastZone;

    public void Dispose()
    {
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
    }

    public void Init()
    {
        Service.ClientState.TerritoryChanged += TerritoryChanged;
    }

    private void TerritoryChanged(ushort currentZone)
    {
        if (!Service.Configuration.PluginEnabled)
        {
            _lastZone = currentZone;
            return;
        }

        if (InvalidPlaces(currentZone))
        {
            _lastZone = currentZone;
            return;
        }

        if (currentZone != _lastZone)
            SwapMinion(currentZone);
    }

    private static bool InvalidPlaces(int zoneId)
    {
        return zoneId switch
        {
            1055 => true, // Island Sanctuary
            _ => false
        };
    }

    private async void SwapMinion(ushort zoneId)
    {
        var trys = 0;

        while (_lastZone != zoneId)
        {
            await Task.Delay(500); //Theres no need to loop like crazy while waiting for the game to load.
            if ((Service.ClientState.LocalContentId == 0 && Service.ClientState.LocalPlayer == null) || BetweenAreas())
                continue;

            if (BoundByDuty()) // Dont proceed if player is Bound By Duty
            {
                _lastZone = zoneId;
                return;
            }

            if (BetweenAreas()) // Rechecking because the BetweenAreas() above can be inconsistent, not sure why.
                continue;

            if (!IsMinionSummoned())
            {
                _lastZone = zoneId;
                break;
            }

            if (ActionAvailable(IdMinionRoulette))
                if (CastAction(IdMinionRoulette))
                {
                    _lastZone = zoneId;
                    break;
                }

            trys++;
            if (trys > 10)
                break;
        }
    }

    private static unsafe bool CastAction(uint id, ActionType actionType = ActionType.GeneralAction)
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

    private static unsafe bool ActionAvailable(uint id)
    {
        try
        {
            return ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, id) == 0 &&
                   !ActionManager.Instance()->IsRecastTimerActive(ActionType.Action, id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool BetweenAreas()
    {
        return Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51];
    }

    private static bool BoundByDuty()
    {
        return Service.Condition[ConditionFlag.BoundByDuty];
    }

    // Ty Pohky for helping me with this
    private static bool IsMinionSummoned()
    {
        var list = Service.ObjectTable;
        try
        {
            var isSummoned = false;

            GameObject? player = Service.ObjectTable.OfType<Character>().FirstOrDefault();

            if (player == null)
                return isSummoned;

            for (var i = 0; i < list.Length - 1; i++)
            {
                if (list[i]?.Address != player?.Address)
                    continue;

                var obj = list[i + 1]; // If the player address is found, the next obj should be the minion
                isSummoned = obj != null && obj.ObjectKind == ObjectKind.Companion;
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