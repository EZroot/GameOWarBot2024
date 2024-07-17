using GameOWar.Entities;
using GameOWar.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandDisplayCommands : Command
    {
        private readonly string _userName;

        public CommandDisplayCommands(int duration, string userName) : base(duration)
        {
            _userName = userName;
        }

        public override void StartMethod()
        {
            base.StartMethod();

            CommandParser.ShowCommands(_userName);
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