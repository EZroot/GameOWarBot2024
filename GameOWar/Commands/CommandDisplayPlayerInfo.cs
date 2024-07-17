using DiscordBot;
using GameOWar.Entities;
using GameOWar.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Commands
{
    internal class CommandDisplayPlayerInfo : Command
    {
        private readonly WorldMap _map;

        public CommandDisplayPlayerInfo(int duration, WorldMap map) : base(duration)
        {
            _map = map;
        }
        public override void StartMethod()
        {
            base.StartMethod();

            var message = "";
            foreach (var playerBase in _map.WorldBases)
            {
                //foreach (var playerBase in player.PlayerBases)
                //{
                    if (playerBase == null)
                    {
                        message += ($"\nBASE DOESNT EXIST!!\n");
                        continue;
                    }

                if (playerBase.Owner == null)
                {
                    message += ($"\nBase {playerBase.BaseName} has no owner!!!\n");
                    continue;
                }
                playerBase.Owner.Currency.Amount += new Random().Next((int)playerBase.Population);
                    var houseCount = playerBase.Buildings.FindAll(x => x.Name == "House");
                    long predictedPopulation = 0;
                    foreach (House house in houseCount) predictedPopulation += house.Population;

                    message += ($"{playerBase.Owner.UserName} |{playerBase.BaseName}| ${playerBase.Owner.Currency.Amount} Pop:{playerBase.Population}/{predictedPopulation} Troops:{playerBase.TotalTroopCount()}\n");
                //}
            }
            BotManager.Instance.QueueMessage($"{message}");
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