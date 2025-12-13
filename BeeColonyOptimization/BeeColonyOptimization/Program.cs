using System;

class Program
{
    //célfüggvény
    static double Function(double x, double y)
    {
        return x * x + y * y;
    }

    //globáls Random
    static Random randomGenerator = new Random();

    //Inicializálás
    static double[,] InitializeBees(int beeCount)
    {
        double MIN_COORD = -5.0;
        double MAX_COORD = 5.0;
        double RANGE = MAX_COORD - MIN_COORD;

        double[,] bees = new double[beeCount, 2];

        for (int i = 0; i < beeCount; i++)
        {
            bees[i, 0] = randomGenerator.NextDouble() * RANGE + MIN_COORD;
            bees[i, 1] = randomGenerator.NextDouble() * RANGE + MIN_COORD;
        }
        return bees;
    }

    //Legjobb méh megtalálása
    static int GetBestBeeIndex(double[,] bees)
    {
        int beeCount = bees.GetLength(0);
        int bestIndex = 0;
        double bestValue = Function(bees[0, 0], bees[0, 1]);

        for (int i = 1; i < beeCount; i++)
        {
            double value = Function(bees[i, 0], bees[i, 1]);
            if (value < bestValue) 
            {
                bestValue = value;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    //segédfüggvény a pozíció javítására
    static void TryImprovePosition(ref double x, ref double y, ref int noImprove)
    {
        double oldValue = Function(x, y);

        double newX = x + (randomGenerator.NextDouble() * 2 - 1);
        double newY = y + (randomGenerator.NextDouble() * 2 - 1);

        if (newX > 5) newX = 5;
        if (newX < -5) newX = -5;
        if (newY > 5) newY = 5;
        if (newY < -5) newY = -5;

        double newValue = Function(newX, newY);

        if (newValue < oldValue)
        {
            x = newX;
            y = newY;
            noImprove = 0;
        }
        else
        {
            noImprove++;
        }
    }

    //dolgozó méhek
    static void EmployedBeePhase(double[,] bees, int[] noImprovement)
    {
        for (int i = 0; i < bees.GetLength(0); i++)
        {
            TryImprovePosition(ref bees[i, 0], ref bees[i, 1], ref noImprovement[i]);
        }
    }

    //Megfigyelő méhek
    static void OnlookerBeePhase(double[,] bees, int[] noImprovement)
    {
        int beeCount = bees.GetLength(0);
        double[] fitness = new double[beeCount];
        double fitnessSum = 0;

        for (int i = 0; i < beeCount; i++)
        {
            double value = Function(bees[i, 0], bees[i, 1]);
            fitness[i] = 1.0 / (1.0 + value);
            fitnessSum += fitness[i];
        }

        //Minden megfigyelő méh választ egy pozíciót és keres körülötte
        for (int k = 0; k < beeCount; k++)
        {
            double r = randomGenerator.NextDouble() * fitnessSum;
            double acc = 0.0;
            int chosenIndex = 0;

            for (int i = 0; i < beeCount; i++)
            {
                acc += fitness[i];
                if (acc >= r)
                {
                    chosenIndex = i;
                    break;
                }
            }

            TryImprovePosition(ref bees[chosenIndex, 0], ref bees[chosenIndex, 1], ref noImprovement[chosenIndex]);
        }
    }

    //felderítő méhek fázisa
    static void ScoutBeePhase(double[,] bees, int[] noImprovement, int limit)
    {
        double MIN_COORD = -5.0;
        double MAX_COORD = 5.0;
        double RANGE = MAX_COORD - MIN_COORD;

        for (int i = 0; i < bees.GetLength(0); i++)
        {
            if (noImprovement[i] >= limit)
            {
                bees[i, 0] = randomGenerator.NextDouble() * RANGE + MIN_COORD;
                bees[i, 1] = randomGenerator.NextDouble() * RANGE + MIN_COORD;
                noImprovement[i] = 0;
            }
        }
    }

    static void Main()
    {
        int BEE_COUNT = 20;
        int MAX_ITERATIONS = 50;
        int FAIL_LIMIT = 10;

        int[] noImprovement = new int[BEE_COUNT];
        var bees = InitializeBees(BEE_COUNT);

        double[] bestPosition = new double[2];
        int initialBestIndex = GetBestBeeIndex(bees);
        bestPosition[0] = bees[initialBestIndex, 0];
        bestPosition[1] = bees[initialBestIndex, 1];

        Console.WriteLine("--- Artificial Bee Colony (ABC) BEMUTATÓ ---");
        Console.WriteLine($"Indítás {BEE_COUNT} méhhel, {MAX_ITERATIONS} iterációra.\n");

        for (int iter = 1; iter <= MAX_ITERATIONS; iter++)
        {
            EmployedBeePhase(bees, noImprovement);
            OnlookerBeePhase(bees, noImprovement);
            ScoutBeePhase(bees, noImprovement, FAIL_LIMIT);

            int currentBestIndex = GetBestBeeIndex(bees);
            double currentBestF = Function(bees[currentBestIndex, 0], bees[currentBestIndex, 1]);

            if (currentBestF < Function(bestPosition[0], bestPosition[1]))
            {
                bestPosition[0] = bees[currentBestIndex, 0];
                bestPosition[1] = bees[currentBestIndex, 1];
            }

            if (iter % 10 == 0 || iter == 1)
            {
                Console.WriteLine($"Iteráció {iter:00}: Legjobb f={currentBestF:0.00000} (x={bees[currentBestIndex, 0]:0.00}, y={bees[currentBestIndex, 1]:0.00})");
            }
        }

        double finalBestF = Function(bestPosition[0], bestPosition[1]);

        Console.WriteLine($"\n--- A végeredmény ---");
        Console.WriteLine($"Legjobb talált minimum f(x,y)={finalBestF:0.00000}");
        Console.WriteLine($"Pozíció: x={bestPosition[0]:0.000}, y={bestPosition[1]:0.000}");
        Console.ReadLine();
    }
}
