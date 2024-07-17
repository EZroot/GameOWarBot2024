using DiscordBot;
using GameOWar.Entities;
using GameOWar.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace GameOWar.Commands
{
    internal class CommandDisplayWorldInfo : Command
    {
        private readonly Player _player;
        private readonly WorldMap _map;

        public CommandDisplayWorldInfo(int duration, Player player, WorldMap map) : base(duration)
        {
            _player = player;
            _map = map;
        }
        public override void StartMethod()
        {
            base.StartMethod();

            var message = "";
            for (int i = 0; i < _map.WorldBases.Count; i++)
            {
                Base? b = _map.WorldBases[i];
                if (_player.Knowledge.BaseKnowledge.Contains(b))
                {
                    var name = b.BaseName;
                    var troops = b.TotalTroopCount();
                    var population = b.Population;

                    var houses = b.Buildings.FindAll(x => x.Name == "House").Count;
                    var mine = b.Buildings.FindAll(x => x.Name == "Mine").Count;
                    var farm = b.Buildings.FindAll(x => x.Name == "Farm").Count;
                    var barracks = b.Buildings.FindAll(x => x.Name == "Barracks").Count;
                    var markets = b.Buildings.FindAll(x => x.Name == "MarketPlace").Count;

                    var food = b.Resources.FindAll(x => x.Name == "Food").Sum(x => x.Quantity);
                    var ore = b.Resources.FindAll(x => x.Name == "Ore").Sum(x => x.Quantity);
                    var stone = b.Resources.FindAll(x => x.Name == "Stone").Sum(x => x.Quantity);
                    var tree = b.Resources.FindAll(x => x.Name == "Tree").Sum(x => x.Quantity);

                    message += ($"#{i} {b.Owner.UserName} **[{name}]** ${b.Owner.Currency.Amount}.00 Pop:{population}/{b.PredictedPopulation} Troops:{troops} | Markets:({markets}) Houses({houses}) Mine({mine}) Farm({farm}) Barracks({barracks}) Food({food}) Ore({ore}) Stone({stone}) Tree({tree})\n");
                }
                //else
                //{
                //    var dist = _map.CalculateDistance(_player.PlayerBases[0].WorldTile, b.WorldTile);
                //    message += ($"#{i} {b.Owner.UserName} [{b.BaseName}] < UNDESCOVERED > {(int)dist*10} miles away\n");
                //}
            }
            _ = Task.Run(async () => await BotManager.Instance.SendMessage(message));

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