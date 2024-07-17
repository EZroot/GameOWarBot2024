using DiscordBot;
using GameOWar.Entities;
using GameOWar.World;

internal class CommandScout : Command
{
    private readonly Base _playerBase;
    private readonly WorldMap _map;
    private readonly string _args;
    private readonly int _startDuration;
    private bool _isHalfwayBuilt;
    private bool _isAlmostBuilt;
    private bool _isPerformingBecauseOfMe;
    public CommandScout(int duration, Base playerBase, WorldMap map, string args) : base(duration)
    {
        _playerBase = playerBase;
        _map = map;
        _args = args.ToLower();
        _startDuration = duration;

    }

    public override void StartMethod()
    {
        if (!_playerBase.IsPerformingAction)
        {
            _isPerformingBecauseOfMe = true;
            _playerBase.IsPerformingAction = true;
        }

        base.StartMethod();
        if (_playerBase.IsPerformingAction && !_isPerformingBecauseOfMe)
        {
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{_playerBase.Owner.UserName}'s {_playerBase.BaseName} is already scouting/attacking!"));
            return;
        }

        if (string.IsNullOrEmpty(_args))
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{_playerBase.Owner.UserName}'s {_playerBase.BaseName} is scouting area in {ticksRemaining} days."));
        else
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{_playerBase.Owner.UserName}'s {_playerBase.BaseName} is scouting {_args} in {ticksRemaining} days."));
    }

    public override void OnTick()
    {
        base.OnTick();
        if (_playerBase.IsPerformingAction && !_isPerformingBecauseOfMe) return;

        if (ticksRemaining == 0) return;
        if (ticksRemaining < _startDuration / 2 && !_isHalfwayBuilt)
        {
            BotManager.Instance.QueueMessage($"{_playerBase.Owner.UserName}'s {_playerBase.BaseName} is halfway done scouting {_args}... {ticksRemaining} days remaining.");
            _isHalfwayBuilt = true;
        }

        if (ticksRemaining < _startDuration / 5 && !_isAlmostBuilt)
        {
            BotManager.Instance.QueueMessage($"{_playerBase.Owner.UserName}'s {_playerBase.BaseName} is almost done scouting {_args}... {ticksRemaining} days remaining.");
            _isAlmostBuilt = true;
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if (_playerBase.IsPerformingAction && !_isPerformingBecauseOfMe) return;
        _isPerformingBecauseOfMe = false;
        _playerBase.IsPerformingAction = false;
        //Scouting specific base
        if (!string.IsNullOrEmpty(_args))
        {
            var foundResult = false;
            foreach (var b in _map.WorldBases)
            {
                if(b.BaseName.ToLower().Contains(_args))
                {
                    BotManager.Instance.QueueMessage($"Found {b.BaseName}! **Use !knowledge to show knowledge**");
                    _playerBase.Owner.Knowledge.AddBaseKnowledge(b);
                    foundResult = true;
                    break;
                }
            }

            if(foundResult) return;
        }

        //Scouting based on distance
        var foundLocations = new List<Base>();
        var playerBase = _playerBase;
        foreach (var b in _map.WorldBases)
        {
            var dist = _map.CalculateDistance(playerBase.WorldTile, b.WorldTile);
            if(dist < 40)
            {
                foundLocations.Add(b);
                _playerBase.Owner.Knowledge.AddBaseKnowledge(b);
            }
        }

        foreach (var location in foundLocations)
        {
            BotManager.Instance.QueueMessage($"Found {location.BaseName} @ ({location.WorldTile.X},{location.WorldTile.Y})");
        }
    }

}
