using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace MinionRoulette;

public class SwapManager : IDisposable
{
    private const uint IdMinionRoulette = 10; //GeneralAction
    private ushort _lastZone;

    private const int TickCooldown = 60;
    private int _frameTick = 0;
    private int _lastTick = 0;

    public void Dispose()
    {
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    public void Init()
    {
        Service.ClientState.TerritoryChanged += TerritoryChanged;
    }

    private void TerritoryChanged(ushort currentZone)
    {
        Service.Framework.Update -= OnFrameworkUpdate;
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

        if (currentZone != _lastZone && _frameTick == 0)
        {
            Service.Framework.Update += OnFrameworkUpdate;
        }
    }

    private bool InvalidPlaces(int zoneId)
    {
        return zoneId switch
        {
            1055 => true, // Island Sanctuary
            _ => false
        };
    }

    private void OnFrameworkUpdate(IFramework _)
    {
        _frameTick++;

        if (_frameTick < _lastTick + TickCooldown)
            return;

        if (_frameTick > 2000)
            SwapComplete();

        _lastTick = _frameTick;

        if (Service.ClientState.LocalContentId == 0 && Service.ClientState.LocalPlayer == null)
            return;

        if (BoundByDuty())
            return;

        if (BetweenAreas())
            return;

        if (!IsMinionSummoned())
        {
            if (Service.Configuration.DontSwapDismissed)
            {
                Service.PluginLog.Debug("Dismissed Minion, not swapping");
                SwapComplete();
                return;
            }
        }

        if (!ActionAvailable(IdMinionRoulette))
            return;

        if (!CastAction(IdMinionRoulette))
            return;

        SwapComplete();
    }

    private void SwapComplete()
    {
        _lastZone = Service.ClientState.TerritoryType;
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.PluginLog.Debug($"Minion Swapped: Ticks = {_frameTick}");
        _frameTick = 0;
        _lastTick = 0;
    }

    private unsafe bool CastAction(uint id, ActionType actionType = ActionType.GeneralAction)
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

    private unsafe bool ActionAvailable(uint id)
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

    private bool BetweenAreas()
    {
        return Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51];
    }

    private bool BoundByDuty()
    {
        return Service.Condition[ConditionFlag.BoundByDuty];
    }

    private bool IsMinionSummoned()
    {
        try
        {
            return Service.ClientState.LocalPlayer?.CurrentMinion?.RowId != 0;
        }
        catch (Exception e)
        {
            Service.PluginLog.Error($"{e.StackTrace}");
            return false;
        }
    }
}