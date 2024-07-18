using GameOWar.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json.Serialization;

namespace GameOWar.World
{
    public class WorldMap
    {
        public int SizeX { get; }
        public int SizeY { get; }
        public WorldTile[,] Tiles { get; }
        public List<Base> WorldBases { get; } = new();
        public List<Player> WorldPlayers { get; } = new();

        private const float NoiseScale = 0.025f;
        private const float BaseThreshold = 0.55f;
        private const int MinimumDistance = 32;

        public WorldMap(int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Tiles = new WorldTile[SizeX, SizeY];

            var loadedTiles = DataManager.LoadWorldTiles("map.json");
            if (loadedTiles == null || GameSettings.FORCE_CREATE_NEW_MAP)
            {
                GenerateWorldMap();
                GenerateBases();
            }
            else
            {
                Tiles = loadedTiles;
            }

            DataManager.SaveWorldTiles(Tiles,"map.json");
            //DrawMap();
            //DrawMapToFile("WorldMap.png"); // Add this to save the map as a PNG file
        }

        public Player FindPlayer(string username)
        {
            return WorldPlayers.First(x => x.UserName.ToLower() == username.ToLower());
        }

        private void GenerateWorldMap()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    Tiles[x, y] = new WorldTile(x, y);
                    AssignBiome(x, y);
                }
            }
        }

        private void AssignBiome(int x, int y)
        {
            float noiseValue = Noise.PerlinNoise(x * NoiseScale, y * NoiseScale);
            Tiles[x, y].Biome = noiseValue switch
            {
                > 0.6f => Biome.Desert,
                > 0.49f => Biome.Forest,
                > 0.45f => Biome.Plain,
                _ => Biome.Water,
            };
        }


        private void GenerateBases()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    float noiseValue = Noise.PerlinNoise(x * NoiseScale, y * NoiseScale);
                    if (noiseValue > BaseThreshold && IsFarEnough(x, y))
                    {
                        var generatedBase = new Base(BaseNameGenerator.GenerateBaseName(), Tiles[x, y]);
                        for (var i = 0; i < new Random().Next(10) + 1; i++)
                        {
                            generatedBase.AddBuilding(new House());
                        }
                        for (var i = 0; i < new Random().Next(2) + 1; i++)
                        {
                            generatedBase.AddBuilding(new MarketPlace());
                        }
                        for (var i = 0; i < new Random().Next(2) + 1; i++)
                        {
                            generatedBase.AddBuilding(new Barracks());
                        }
                        for (var i = 0; i < new Random().Next(3) + 1; i++)
                        {
                            generatedBase.AddBuilding(new Farm());
                        }
                        for (var i = 0; i < new Random().Next(3) + 1; i++)
                        {
                            generatedBase.AddBuilding(new Mine());
                        }
                        var generatedNPC = new Player(x + y, BaseNameGenerator.GenerateMaleName(), 1, new List<Base> { generatedBase }, new Currency("Money", 20));
                        AddPlayer(generatedNPC);
                        AddBase(generatedBase);
                    }
                }
            }
        }

        private bool IsFarEnough(int x, int y)
        {
            foreach (var playerBase in WorldBases)
            {
                if (CalculateDistance(playerBase.WorldTile, Tiles[x, y]) < MinimumDistance)
                {
                    return false;
                }
            }
            return true;
        }

        private void DrawMap()
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    if (WorldBases.Exists(b => b.WorldTile.X == x && b.WorldTile.Y == y))
                    {
                        Console.ForegroundColor = ConsoleColor.Red; // Color for bases
                        Console.Write("X "); // Draw base
                        Console.ResetColor();
                    }
                    else
                    {
                        char symbol = GetTileSymbol(Tiles[x, y].Biome);
                        Console.ForegroundColor = GetBiomeColor(Tiles[x, y].Biome); // Set color based on biome
                        Console.Write($"{symbol} "); // Draw biome
                        Console.ResetColor();
                    }
                }
                Console.WriteLine(); // New line after each row
            }
        }
        private Color GetBiomeColorForBitmap(Biome biome)
        {
            return biome switch
            {
                Biome.Desert => Color.DarkGreen, // Color for Desert
                Biome.Forest => Color.Green, // Color for Forest
                Biome.Plain => Color.LightGray, // Color for Plain
                Biome.Water => Color.Blue, // Color for Water
                _ => Color.White // Default color
            };
        }

        public void DrawMapToFile(string filePath)
        {
            int tileSize = 10; // Size of each tile in pixels
            using var bitmap = new Bitmap(SizeX * tileSize, SizeY * tileSize);
            using var graphics = Graphics.FromImage(bitmap);
            var font = new Font("Arial", 16); // Font for drawing text
            var textBrush = new SolidBrush(Color.Black); // Brush for drawing text

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    Color color;
                    if (WorldBases.Exists(b => b.WorldTile.X == x && b.WorldTile.Y == y))
                    {
                        color = Color.Red; // Color for bases
                    }
                    else
                    {
                        color = GetBiomeColorForBitmap(Tiles[x, y].Biome); // Color based on biome
                    }

                    using var brush = new SolidBrush(color);
                    graphics.FillRectangle(brush, x * tileSize, y * tileSize, tileSize, tileSize);

                }
            }

            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    // Check if the current tile has a base and draw the base name if it does
                    var baseOnTile = WorldBases.Find(b => b.WorldTile.X == x && b.WorldTile.Y == y);
                    if (baseOnTile != null)
                    {
                        string baseName = baseOnTile.BaseName; // Assuming each base has a 'Name' property
                        var textPosition = new PointF(x * tileSize, y * tileSize);
                        graphics.DrawString(baseName, font, textBrush, textPosition);
                    }
                }
            }

            bitmap.Save(filePath, ImageFormat.Png);
        }


        private char GetTileSymbol(Biome biome)
        {
            return biome switch
            {
                Biome.Desert => 'D', // Desert symbol
                Biome.Forest => 'F', // Forest symbol
                Biome.Plain => '.', // Plain symbol
                Biome.Water => '~', // Water symbol
                _ => ' ' // Default symbol
            };
        }

        private ConsoleColor GetBiomeColor(Biome biome)
        {
            return biome switch
            {
                Biome.Desert => ConsoleColor.Yellow, // Color for Desert
                Biome.Forest => ConsoleColor.Green, // Color for Forest
                Biome.Plain => ConsoleColor.Gray, // Color for Plain
                Biome.Water => ConsoleColor.Blue, // Color for Water
                _ => ConsoleColor.White // Default color
            };
        }


        public double CalculateDistance(WorldTile tileA, WorldTile tileB)
        {
            int deltaX = tileB.X - tileA.X;
            int deltaY = tileB.Y - tileA.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public void AddBase(Base playerBase)
        {
            Console.WriteLine($"Adding base {playerBase.BaseName} owner {playerBase.Owner}");
            WorldBases.Add(playerBase);
        }

        public void AddPlayer(Player player)
        {
            Console.WriteLine($"Adding player {player.UserName}");
            WorldPlayers.Add(player);
        }
    }

    [Serializable]
    public class WorldTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Biome Biome { get; set; } // Add biome property

        public WorldTile() { }
        public WorldTile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum Biome // Define biomes
    {
        Desert,
        Forest,
        Plain,
        Water
    }
}
