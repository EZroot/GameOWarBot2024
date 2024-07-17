using DiscordBot;
using GameOWar.Entities;
using GameOWar.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandDistance : Command
    {
        private readonly WorldMap _map;
        private readonly Base _attackingBase;
        private readonly Base _targetBase;
        public CommandDistance(int duration, Base attackingBase, Base targetBase, WorldMap map) : base(duration)
        {
            _attackingBase = attackingBase;
            _targetBase = targetBase;
            _map = map;
        }
        public override void StartMethod()
        {
            base.StartMethod();
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{_attackingBase.BaseName} is {(int)_map.CalculateDistance(_attackingBase.WorldTile, _targetBase.WorldTile)} miles from[{_targetBase.BaseName}]!"));
        }

        public override void OnTick()
        {
            base.OnTick();
            if (ticksRemaining == 0) return;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            
        }
    }
}