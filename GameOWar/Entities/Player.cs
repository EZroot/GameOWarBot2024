using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar.Entities
{
    [Serializable]
    public class Player
    {
        public int ID;
        public string UserName;
        public long Level;
        public List<Base> PlayerBases;
        public Currency Currency;
        public PlayerKnowledge Knowledge;

        public Player()
        {
        }

        public Player(int iD, string userName, int level, List<Base> playerBase, Currency currency)
        {
            ID = iD;
            UserName = userName;
            Level = level;
            PlayerBases = playerBase;
            Currency = currency;
            if (PlayerBases != null)
            {
                foreach (var b in PlayerBases)
                    b.SetOwner(userName, false);
                Knowledge = new PlayerKnowledge(PlayerBases[0]);
            }
        }
    }
    [Serializable]
    public class Currency
    {
        public string Name;
        public long Amount;

        public Currency()
        {
        }

        public Currency(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }

    [Serializable]
    public class PlayerKnowledge
    {
        public List<Base> BaseKnowledge;

        public PlayerKnowledge()
        {
        }

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
