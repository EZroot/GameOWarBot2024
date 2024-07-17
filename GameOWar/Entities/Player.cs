using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Entities
{
    public class Player
    {
        public int ID;
        public string UserName;
        public long Level;
        public List<Base> PlayerBases;
        public Currency Currency;
        public PlayerKnowledge Knowledge;
        public Player(int iD, string userName, int level, List<Base> playerBase, Currency currency)
        {
            ID = iD;
            UserName = userName;
            Level = level;
            PlayerBases = playerBase;
            Currency = currency;
            foreach(var b in PlayerBases)
                b.SetOwner(this, false);
            Knowledge = new PlayerKnowledge(PlayerBases[0]);
        }
    }

    public class Currency
    {
        public string Name;
        public long Amount;

        public Currency(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }

    public class PlayerKnowledge
    {
        public List<Base> BaseKnowledge;

        public PlayerKnowledge(Base playerBase)
        {
            BaseKnowledge = new List<Base> { playerBase };
        }

        public void AddBaseKnowledge(Base playerBase)
        {
            BaseKnowledge.Add(playerBase);
        }
    }
}
