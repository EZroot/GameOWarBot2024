using System;
using System.Collections.Generic;

namespace GameOWar.Commands
{
    public static class BuildingFactory
    {
        public static readonly Dictionary<string, Func<Building>> BuildingFactories = new()
        {
            { "farm", () => new Farm() },
            { "mine", () => new Mine() },
            { "house", () => new House() },
            { "barracks", () => new Barracks() },
            { "marketplace", () => new MarketPlace() },
            { "logging", () => new Logging() }
            // Add more building types here
        };
    }
}
