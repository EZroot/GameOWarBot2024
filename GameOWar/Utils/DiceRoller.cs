public static class DiceRoller
{
    private static Random random = new Random();

    public static int RollD4()
    {
        return RollDie(4);
    }

    public static int RollD6()
    {
        return RollDie(6);
    }

    public static int RollD8()
    {
        return RollDie(8);
    }

    public static int RollD10()
    {
        return RollDie(10);
    }

    public static int RollD12()
    {
        return RollDie(12);
    }

    public static int RollD20()
    {
        return RollDie(20);
    }

    private static int RollDie(int sides)
    {
        return random.Next(1, sides + 1);
    }

    public static int RollMultipleDice(int numberOfDice, int sides)
    {
        int total = 0;
        for (int i = 0; i < numberOfDice; i++)
        {
            total += RollDie(sides);
        }
        return total;
    }
}