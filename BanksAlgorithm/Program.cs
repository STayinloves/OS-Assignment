using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace BankersAlgorithm
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ShowIntro();

            var resources = GetTotalResources(out var resourcesNumber);
            var claimedResources = GetClaimedResources(resourcesNumber);
            var allocatedResources =
                GetAllocatedResources(resourcesNumber, claimedResources);
            var isSafe = Judge(resources, claimedResources, allocatedResources);

            if (isSafe)
            {
                Console.WriteLine("Safe", Color.GreenYellow);
            }
            else
            {
                Console.WriteLine("Unsafe", Color.Red);
            }

            //Suspend the screen
            Console.ReadLine();
        }

        private static bool Judge(List<int> resources,
            List<List<int>> claimedResources,
            List<List<int>> allocatedResources)
        {
            var processCount = claimedResources.Count;
            var neededResources = claimedResources.Zip(allocatedResources,
                (a, b) => a.Zip(b, (i, j) => i - j)).ToList();
            var currentResources = resources.Zip(allocatedResources.Aggregate(
                    (a, b) => a.Zip(b, (int1, int2) => int1 + int2).ToList()),
                (int1, int2) => int1 - int2).ToList();

            if (currentResources.Any(o => o < 0))
            {
                Console.WriteLine(
                    "Currently allocated resources exceeds the limitation", Color.Red);
                return false;
            }

            var results = new List<int>();
            var runed = Enumerable.Repeat(false, claimedResources.Count)
                .ToList();

            for (var i = 0; i < processCount; i++)
            {
                if (neededResources[i].All(o => o.Equals(0)))
                {
                    runed[i] = true;
                    Console.WriteLine(i + 1 + " seems running, freed.", Color.Aqua);
                    currentResources = currentResources.Zip(
                        allocatedResources[i],
                        (i1, i2) => i1 + i2).ToList();
                    results.Add(i);
                }
            }

            while (runed.Any(fg => !fg))
            {
                var onceRun = false;
                for (var i = 0; i < processCount; i++)
                {
                    if (!runed[i] && currentResources
                            .Zip(neededResources[i], (i1, i2) => i1 - i2)
                            .All(o => o >= 0))
                    {
                        runed[i] = true;
                        Console.WriteLine(i + 1 + " is running, freed.", Color.Aqua);
                        currentResources = currentResources.Zip(
                            allocatedResources[i],
                            (i1, i2) => i1 + i2).ToList();
                        results.Add(i);

                        onceRun = true;
                    }
                }

                if (!onceRun)
                {
                    return false;
                }
            }

            Console.WriteLine(results.Select(o => (o + 1).ToString()).Aggregate((i1, i2) => $"{i1}->{i2}"), Color.Aqua);            

            return true;
        }

        private static void GetResourcesNumber(out int resourcesNumber)
        {
            Console.WriteLine("How many resources do you want to have?");
            while (true)
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out resourcesNumber))
                {
                    return;
                }

                HandleInvalidInput();
            }
        }

        private static List<int> GetTotalResources(out int resourcesNumber)
        {
            GetResourcesNumber(out resourcesNumber);
            var sb = new StringBuilder();
            for (var i = 0; i < resourcesNumber; i++)
            {
                sb.Append(Convert.ToChar('A' + i));
                sb.Append(' ');
            }

            Console.WriteLine(sb);
            while (true)
            {
                var isValid = true;
                var totalResourcesArray = new List<int>();

                var input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    HandleInvalidInput();
                    continue;
                }

                var resources = Regex.Split(input, @"\W+");

                foreach (var num in resources)
                {
                    if (!int.TryParse(num, out var numResult))
                    {
                        isValid = false;
                        break;
                    }

                    totalResourcesArray.Add(numResult);
                }

                if (totalResourcesArray.Count != resourcesNumber)
                {
                    isValid = false;
                }

                if (isValid)
                {
                    return totalResourcesArray;
                }

                HandleInvalidInput();
            }
        }

        private static List<List<int>> GetClaimedResources(int resourcesNumber)
        {
            Console.WriteLine("Please input processes' claimed resources,");
            Console.WriteLine("an empty line to end the input.");
            Console.WriteLine("    Processes (claimed resources):");
            var sb = new StringBuilder();
            for (var i = 0; i < resourcesNumber; i++)
            {
                sb.Append(Convert.ToChar('A' + i));
                sb.Append(' ');
            }

            Console.WriteLine("    " + sb);

            var ret = new List<List<int>>();
            var count = 1;
            while (true)
            {
                var isEnd = false;
                while (true)
                {
                    Console.Write("P" + count + ": ");
                    var input = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(input))
                    {
                        isEnd = true;
                        break;
                    }

                    var resources = Regex.Split(input, @"\W+").Select(o =>
                    {
                        int.TryParse(o, out var num);
                        return num;
                    }).ToList();

                    if (resources.Count != resourcesNumber)
                    {
                        HandleInvalidInput();
                        continue;
                    }

                    ret.Add(resources);
                    break;
                }

                count++;

                if (isEnd)
                {
                    break;
                }
            }

            ShowHorizonRule();
            return ret;
        }

        private static List<List<int>> GetAllocatedResources(
            int resourcesNumber, List<List<int>> claimed)
        {
            Console.WriteLine("Please input processes' allocated resources,");
            Console.WriteLine("an empty line to skip the input.");
            Console.WriteLine(
                "NOTE: currently allocated resources can't exceed the claimed amount", Color.Yellow);
            Console.WriteLine("Processes:");
            var sb = new StringBuilder();
            for (var i = 0; i < resourcesNumber; i++)
            {
                sb.Append(Convert.ToChar('A' + i));
                sb.Append(' ');
            }

            Console.WriteLine("    " + sb);

            var ret = new List<List<int>>();
            var count = 1;
            while (true)
            {
                while (true)
                {
                    Console.Write("P" + count + ": ");
                    var input = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(input))
                    {
                        ret.Add(Enumerable.Repeat(0, resourcesNumber).ToList());
                        break;
                    }

                    var resources = Regex.Split(input, @"\W+").Select(o =>
                    {
                        int.TryParse(o, out var num);
                        return num;
                    }).ToList();

                    var valid = true;
                    if (resources.Count != resourcesNumber)
                    {
                        valid = false;
                    }
                    else
                    {
                        for (var i = 0; i < resourcesNumber; i++)
                        {
                            if (claimed[count - 1][i] < resources[i])
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                    if (!valid)
                    {
                        HandleInvalidInput();
                        continue;
                    }

                    ret.Add(resources);
                    break;
                }

                count++;
                if (count > claimed.Count)
                {
                    break;
                }
            }

            ShowHorizonRule();
            return ret;
        }

        private static void ShowIntro()
        {
            Console.WriteAscii("Banker", Color.Azure);
            Console.WriteLine(
                "Welcome to the Banker's Algorithm console application!");
            Console.WriteLine();
            Console.WriteLine("Here is an example you could try:", Color.Aquamarine);
            Console.WriteLine("Resources number: 3");
            Console.WriteLine("    A B C");
            Console.WriteLine("    3 3 3");
            Console.WriteLine("    A B C");
            Console.WriteLine("P1: 1 0 0");
            Console.WriteLine("P2: 0 1 0");
            Console.WriteLine("P3: 0 0 1");
            Console.WriteLine("P4: 2 0 0");
            Console.Write("This example is ");
            Console.WriteLine("safe", Color.GreenYellow);
            ShowHorizonRule();
        }

        private static void ShowHorizonRule()
        {
            Console.WriteLine(new string('-', 60), Color.Green);
            Console.WriteLine(new string('-', 60), Color.Green);
            Console.WriteLine();
        }

        private static void HandleInvalidInput()
        {
            Console.WriteLine("The input is invalid, please input again", Color.Red);
        }
    }
}