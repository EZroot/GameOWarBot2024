using DiscordBot;
using GameOWar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandSimulateCombat : Command
    {
        private readonly Base _attackingBase;
        private readonly Base _targetBase;
        private readonly string _args;
        private int _initialTick;
        private bool _isBattleFinished;
        private bool _isTroopsRecoveredHalf;
        private bool _isTroopsRecoveredAlmost;
        public CommandSimulateCombat(int duration, Base attackingBase, Base targetBase, string args) : base(duration)
        {
            _attackingBase = attackingBase;
            _targetBase = targetBase;
            _args = args.ToLower();
            _isBattleFinished = false;
            _initialTick = duration;
            _isTroopsRecoveredHalf = false;
            _isTroopsRecoveredAlmost = false;
        }
        public override void StartMethod()
        {
            base.StartMethod();
            _ = Task.Run(async () => await BotManager.Instance.SendMessage($"{_attackingBase.BaseName} vs [{_targetBase.BaseName}] Battle started!"));
        }

        public override void OnTick()
        {
            base.OnTick();
            if (ticksRemaining == 0) return;
            if (_isBattleFinished)
            {
                _attackingBase.IsTroopsRecovering = true;
                DisplayRecovery();
                return;
            }

            if (_attackingBase.TotalTroopCount() < 1 || _targetBase.TotalTroopCount() < 1) _isBattleFinished = true;
            if (_isBattleFinished) { DisplayWinner(); return; }

            var playerAttackRoll = DiceRoller.RollD20();
            var targetAttackRoll = DiceRoller.RollD20();

            if (playerAttackRoll > targetAttackRoll)
            {

                BotManager.Instance.QueueMessage($"{_attackingBase.BaseName} won the roll with {playerAttackRoll} against {targetAttackRoll}.");
                CalculateCasualties(_attackingBase, _targetBase, playerAttackRoll, targetAttackRoll);
            }
            else if (targetAttackRoll > playerAttackRoll)
            {
                BotManager.Instance.QueueMessage($"{_targetBase.BaseName} won the roll with {targetAttackRoll} against {playerAttackRoll}.");
                CalculateCasualties(_targetBase, _attackingBase, targetAttackRoll, playerAttackRoll);
            }
            else
            {
                BotManager.Instance.QueueMessage("It's a tie! No casualties this round.");
            }

            BotManager.Instance.QueueMessage($"[{_attackingBase.BaseName}] has {_attackingBase.TotalTroopCount()} troops left.");
            BotManager.Instance.QueueMessage($"[{_targetBase.BaseName}] has {_targetBase.TotalTroopCount()} troops left.");
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (_isBattleFinished) return;
            DisplayWinner();
            _attackingBase.IsTroopsRecovering = false;
        }

        void CalculateCasualties(Base winner, Base loser, int winnerRoll, int loserRoll)
        {
            // Calculate percentage killed based on the roll difference
            int rollDifference = winnerRoll - loserRoll;
            double casualtyRate = Math.Min(rollDifference / 20.0, 1.0); // Maximum casualty rate is 100%

            int casualties = (int)(loser.TotalTroopCount() * casualtyRate);
            loser.RemoveTroop("infantry", casualties);

            BotManager.Instance.QueueMessage($"***{winner.BaseName}*** inflicted **{casualties}** casualties on ***{loser.BaseName}***.");
        }

        void DisplayWinner()
        {
            BotManager.Instance.QueueMessage($"[{_attackingBase.BaseName}] vs {_targetBase.BaseName} has ended.");
            if (_attackingBase.TotalTroopCount() > _targetBase.TotalTroopCount())
            {
                BotManager.Instance.QueueMessage($"[{_attackingBase.BaseName}] won!! Troops will be recovered in {ticksRemaining} days.");
                _targetBase.SetOwner(_attackingBase.Owner); 
            }
            else
            {
                BotManager.Instance.QueueMessage($"[{_targetBase.BaseName}] won!! Troops will be recovered in {ticksRemaining} days.");
                _attackingBase.SetOwner(_targetBase.Owner); 
            }
        }

        void DisplayRecovery()
        {
            int elapsedTicks = _initialTick - ticksRemaining;
            if (elapsedTicks >= _initialTick / 2 && !_isTroopsRecoveredHalf)
            {
                // Explain the remaining ticks if the battle ends early
                BotManager.Instance.QueueMessage($"[{_attackingBase.BaseName}] troops are halfway recovered... {ticksRemaining} days remaining.");
                _isTroopsRecoveredHalf = true;
            }
            else if (elapsedTicks >= _initialTick * 4 / 5 && !_isTroopsRecoveredAlmost)
            {
                // Explain the remaining ticks if the battle ends early
                BotManager.Instance.QueueMessage($"[{_attackingBase.BaseName}] troops are almost recovered... {ticksRemaining} days remaining.");
                _isTroopsRecoveredAlmost = true;
            }
        }
    }
}