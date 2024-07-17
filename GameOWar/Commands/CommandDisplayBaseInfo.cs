using GameOWar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandDisplayBaseInfo : Command
    {
        private readonly Player _player;

        public CommandDisplayBaseInfo(int duration, Player player) : base(duration) 
        {
            _player = player;
        }
        public override void StartMethod()
        {
            base.StartMethod();
            //Console.WriteLine("Population: " + _player.PlayerBase.Population);
            //foreach (var b in _player.PlayerBase.Buildings) Console.Write($"Building [{b.Name} {b.Level}] \n");
            //foreach (var b in _player.PlayerBase.Troops) Console.Write($"Troop [{b.Name} {b.Level} {b.Amount}] \n");
            //foreach (var b in _player.PlayerBase.Resources) Console.Write($"Resources [{b.Name} {b.Quantity}] \n");
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