using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOWar
{
    internal static class GameSettings
    {
        public const int TICK_INTERVAL = 60000; //30 Seconds
        public const bool FORCE_CREATE_NEW_USERS = false;  //Instead of loading players, create new accs + bases for them
        public const bool FORCE_CREATE_NEW_MAP = false; //Instead of loading map, generate a new one
    }
}
