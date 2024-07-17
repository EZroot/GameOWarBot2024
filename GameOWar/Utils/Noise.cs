using System;

public static class Noise
{
    private static readonly int[] Permutation = new int[512];
    private static readonly (float, float)[] Gradients = new (float, float)[512];

    static Noise()
    {
        InitializePermutationTable();
        InitializeGradientTable();
    }

    private static void InitializePermutationTable()
    {
        Random random = new Random();

        for (int i = 0; i < 256; i++)
        {
            Permutation[i] = i;
        }

        for (int i = 0; i < 256; i++)
        {
            int j = random.Next(256);
            (Permutation[i], Permutation[j]) = (Permutation[j], Permutation[i]);
        }

        for (int i = 0; i < 256; i++)
        {
            Permutation[256 + i] = Permutation[i];
        }
    }

    private static void InitializeGradientTable()
    {
        Random random = new Random();

        for (int i = 0; i < 256; i++)
        {
            double angle = random.NextDouble() * Math.PI * 2;
            Gradients[i] = ((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        for (int i = 0; i < 256; i++)
        {
            Gradients[256 + i] = Gradients[i];
        }
    }

    public static float PerlinNoise(float x, float y)
    {
        int xi = (int)x & 255;
        int yi = (int)y & 255;

        float xf = x - (int)x;
        float yf = y - (int)y;

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = Permutation[xi + Permutation[yi]];
        int ab = Permutation[xi + Permutation[yi + 1]];
        int ba = Permutation[xi + 1 + Permutation[yi]];
        int bb = Permutation[xi + 1 + Permutation[yi + 1]];

        float x1 = Lerp(Dot(Gradients[aa], xf, yf), Dot(Gradients[ba], xf - 1, yf), u);
        float x2 = Lerp(Dot(Gradients[ab], xf, yf - 1), Dot(Gradients[bb], xf - 1, yf - 1), u);

        return (Lerp(x1, x2, v) + 1) / 2; // Normalize to [0, 1]
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static float Dot((float, float) gradient, float x, float y) => gradient.Item1 * x + gradient.Item2 * y;
}
