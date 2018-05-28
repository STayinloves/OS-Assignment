using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BankersAlgorithm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ShowIntro();

            var resources = GetTotalResources(out var resourcesNumber);
            var claimedResources = GetClaimedResources(resourcesNumber);
            var allocatedResources = GetAllocatedResources(resourcesNumber, claimedResources);
            var isSafe = Judge(resources, claimedResources, allocatedResources);

            Console.WriteLine(isSafe? "Safe" : "Unsafe"); 

            //Suspend the screen
            Console.ReadLine();
        }

        private static bool Judge(List<int> resources, List<List<int>> claimedResources, List<List<int>> allocatedResources)
        {
            throw new NotImplementedException();
        }

        private static void GetResourcesNumber(out int resourcesNumber)
        {
            Console.WriteLine("How many resources do you want to have?");
            var input = Console.ReadLine();
            while (true)
            {
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
                var valid = true;
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
                        valid = false;
                        break;
                    }

                    totalResourcesArray.Add(numResult);
                }

                if (totalResourcesArray.Count != resourcesNumber)
                {
                    valid = false;
                }

                if (valid)
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
            Console.WriteLine("\tProcesses (claimed resources):");
            var sb = new StringBuilder();
            for (var i = 0; i < resourcesNumber; i++)
            {
                sb.Append(Convert.ToChar('A' + i));
                sb.Append(' ');
            }

            Console.WriteLine("\t" + sb);

            var ret = new List<List<int>>();
            var count = 1;
            while (true)
            {
                var isEnd = false;
                while (true)
                {
                    Console.Write("P" + count + " :");
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

        private static List<List<int>> GetAllocatedResources(int resourcesNumber, List<List<int>> claimed)
        {
            Console.WriteLine("Please input processes' allocated resources,");
            Console.WriteLine("an empty line to skip the input.");
            Console.WriteLine("NOTE: currently allocated resources can't exceed the claimed amount");
            Console.WriteLine("\tProcesses:");
            var sb = new StringBuilder();
            for (var i = 0; i < resourcesNumber; i++)
            {
                sb.Append(Convert.ToChar('A' + i));
                sb.Append(' ');
            }

            Console.WriteLine("\t" + sb);

            var ret = new List<List<int>>();
            var count = 1;
            while (true)
            {
                while (true)
                {
                    Console.Write("P" + count + " :");
                    var input = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(input))
                    {
                        continue;
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
                            if (claimed[count - 1][i] < resources[i])
                            {
                                valid = false;
                                break;
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
            Console.WriteLine("Welcome to the Banker's Algorithm console application!");
            Console.WriteLine();
            Console.WriteLine("Here are some examples you could try:");
            Console.WriteLine("");
            ShowHorizonRule();
        }

        private static void ShowHorizonRule()
        {
            Console.WriteLine(new string('-', 60));
            Console.WriteLine(new string('-', 60));
            Console.WriteLine();
        }

        private static void HandleInvalidInput()
        {
            Console.WriteLine("The input is invalid, please input again");
        }
    }
}