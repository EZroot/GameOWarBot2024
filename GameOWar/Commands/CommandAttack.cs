﻿using DiscordBot;
using GameOWar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandAttack : Command
    {
        private readonly Base _attackingBase;
        private readonly Base _targetBase;
        private readonly string _args;
        private readonly int _startDuration;
        private bool _isHalfwayBuilt;
        private bool _isAlmostBuilt;
        private bool _isPerformingBecauseOfMe;
        public CommandAttack(int duration, Base attackinBase, Base targetBase, string args) : base(duration)
        {
            _attackingBase = attackinBase;
            _targetBase = targetBase;
            _args = args.ToLower();
            _startDuration = duration;


        }
        public override void StartMethod()
        {
            if (!_attackingBase.IsPerformingAction)
            {
                _isPerformingBecauseOfMe = true;
                _attackingBase.IsPerformingAction = true;
            }
            base.StartMethod();
            var player = BotManager.Instance.Game.WorldMap.FindPlayer(_attackingBase.Owner).UserName;
            var target = BotManager.Instance.Game.WorldMap.FindPlayer(_targetBase.Owner).UserName;

            if (_attackingBase.IsPerformingAction && !_isPerformingBecauseOfMe)
            {
                _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{player}'s [{_attackingBase.BaseName}] is already attack/scouting!"));
                return;
            }
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{player}'s [{_attackingBase.BaseName}] is attacking {target}'s [{_targetBase.BaseName}] in {ticksRemaining} days."));
        }

        public override void OnTick()
        {
            base.OnTick();
            if (_attackingBase.IsPerformingAction && !_isPerformingBecauseOfMe) return;

            if (ticksRemaining == 0) return;

            var player = BotManager.Instance.Game.WorldMap.FindPlayer(_attackingBase.Owner).UserName;
            var target = BotManager.Instance.Game.WorldMap.FindPlayer(_targetBase.Owner).UserName;
            if (ticksRemaining <= _startDuration / 2 && !_isHalfwayBuilt)
            {
                BotManager.Instance.QueueMessage($"{player}'s [{_attackingBase.BaseName}] troops are halfway toward {target}'s [{_targetBase.BaseName}].");
                _isHalfwayBuilt = true;
            }
            else if (ticksRemaining <= _startDuration / 5 && !_isAlmostBuilt)
            {
                BotManager.Instance.QueueMessage($"{player}'s [{_attackingBase.BaseName}] troops are almost at {target}'s [{_targetBase.BaseName}].");
                _isAlmostBuilt = true;
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (_attackingBase.IsPerformingAction && !_isPerformingBecauseOfMe) return;
            _isPerformingBecauseOfMe = false;
            _attackingBase.IsPerformingAction = false;
            var player = BotManager.Instance.Game.WorldMap.FindPlayer(_attackingBase.Owner).UserName;
            var target = BotManager.Instance.Game.WorldMap.FindPlayer(_targetBase.Owner).UserName;
            BotManager.Instance.QueueMessage($"{player}'s [{_attackingBase.BaseName}] troops arrived at {target}'s [{_targetBase.BaseName}]!");
            CommandHub.RegisterCommand(new CommandSimulateCombat(DiceRoller.RollD20()+10, _attackingBase, _targetBase, _args));
        }

    }
}