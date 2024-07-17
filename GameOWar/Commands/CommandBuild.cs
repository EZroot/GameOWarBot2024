using DiscordBot;
using GameOWar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandBuild : Command
    {
        private readonly Base _base;
        private readonly string _args;
        private readonly int _startDuration;
        private bool _isHalfwayBuilt;
        private bool _isAlmostBuilt;
        private bool _canBuild;
        public CommandBuild(int duration, Base playerBase, string args) : base(duration)
        {
            _base = playerBase;
            _args = args.ToLower();
            _startDuration = duration;
            _canBuild = true;
        }
        public override void StartMethod()
        {
            base.StartMethod();
            if(_base.Owner.Currency.Amount < 10)
            {
                _ = Task.Run(async () => await BotManager.Instance.SendMessage($"You can't afford {_args}. All buildings cost $10 each"));
                _canBuild = false;
                return;
            }
            _base.Owner.Currency.Amount -= 10;
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"[{_base.BaseName}] started building {_args}. Done in {ticksRemaining} days."));
        }

        public override void OnTick()
        {
            base.OnTick();
            if (ticksRemaining == 0 || !_canBuild) return;

            if (ticksRemaining < _startDuration / 2 && !_isHalfwayBuilt)
            {
                BotManager.Instance.QueueMessage($"[{_base.BaseName}] {_args} is halfway built. {ticksRemaining} days remaining.");// {ticksRemaining} ticks");
                _isHalfwayBuilt = true;
            }

            if (ticksRemaining < _startDuration / 5 && !_isAlmostBuilt)
            {
                BotManager.Instance.QueueMessage($"[{_base.BaseName}] {_args} is almost built. {ticksRemaining} days remaining.");// {ticksRemaining} ticks");
                _isAlmostBuilt = true;
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (!_canBuild) return;
            //TODO: Parse args for building type
            switch (_args)
            {
                case "farm": _base.AddBuilding(new Farm()); break;
                case "mine": _base.AddBuilding(new Mine()); break;
                case "house": _base.AddBuilding(new House()); break;
                case "barracks": _base.AddBuilding(new Barracks()); break;
                case "marketplace": _base.AddBuilding(new MarketPlace()); break;
            }
           BotManager.Instance.QueueMessage($"[{_base.BaseName}] finished building {_args}");
        }

    }
}