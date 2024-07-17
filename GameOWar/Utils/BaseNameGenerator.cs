public static class BaseNameGenerator
{
    private static readonly string[] Prefixes = new[]
      {
        "North", "South", "East", "West", "New", "Old", "Fort", "Castle", "Lake", "River",
        "Bright", "Shadow", "Golden", "Silver", "Iron", "Stone", "Green", "Blue", "Red", "Black",
        "White", "Grey", "Spring", "Autumn", "Winter", "Summer", "Dragon", "Wolf", "Bear", "Eagle",
        "Lion", "Falcon", "Thunder", "Storm", "Sun", "Moon", "Star", "Wind", "Fire", "Ice", "Frost",
        "Crystal", "Shadow", "Shadow", "Dark", "High", "Low", "Deep", "Clear", "Swift", "Silent",
        "Misty", "Glade", "Glen", "Hollow", "Vale", "Hill", "Peak", "Bay", "Isle", "Marsh"
    };

    private static readonly string[] Suffixes = new[]
    {
        "wood", "town", "ville", "haven", "side", "field", "fort", "keep", "burg", "shire",
        "dale", "grove", "ridge", "cross", "bridge", "water", "watch", "point", "cove", "rock",
        "fall", "reach", "march", "gate", "cliff", "strand", "port", "crest", "mount", "peak",
        "valley", "wold", "moor", "heath", "mead", "ford", "springs", "ridge", "falls", "bluff",
        "harbor", "grove", "hollow", "dell", "arch", "brook", "mill", "glade", "fields", "lands"
    };

    private static readonly string[] MaleNames = new[]
    {
    "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Charles", "Thomas",
    "Christopher", "Daniel", "Matthew", "Anthony", "Donald", "Paul", "Mark", "George", "Steven", "Edward",
    "Brian", "Ronald", "Kevin", "Jason", "Jeffrey", "Gary", "Timothy", "Jose", "Larry", "Kenneth",
    "Brandon", "Frank", "Scott", "Eric", "Stephen", "Andrew", "Raymond", "Gregory", "Joshua", "Jerry",
    "Dennis", "Walter", "Patrick", "Peter", "Harold", "Douglas", "Henry", "Carl", "Arthur", "Ryan"
};

    private static readonly string[] FemaleNames = new[]
    {
    "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen",
    "Nancy", "Lisa", "Betty", "Dorothy", "Sandra", "Ashley", "Kimberly", "Donna", "Emily", "Michelle",
    "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Laura", "Sharon", "Cynthia", "Kathleen",
    "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Victoria", "Christina",
    "Catherine", "Samantha", "Ruth", "Janet", "Virginia", "Maria", "Debra", "Diane", "Julie", "Joyce"
};


    private static readonly Random Random = new Random();
    public static string GenerateMaleName()
    {
        return MaleNames[Random.Next(MaleNames.Length)];
    }

    public static string GenerateFemaleName()
    {
        return FemaleNames[Random.Next(FemaleNames.Length)];
    }

    public static string GenerateBaseName()
    {
        string prefix = Prefixes[Random.Next(Prefixes.Length)];
        string suffix = Suffixes[Random.Next(Suffixes.Length)];
        return $"{prefix}{suffix}";
    }
}