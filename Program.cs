using System;
using System.Collections.Generic;

namespace transport_idle
{
    class Program
    {

        static void Main()
        {
            var timeUpdated = DateTime.Now;
            string actionInput;
            Console.Clear();

            while (true)
            {
                
                World.Update();
                World.PrintStatus();
                Console.WriteLine("What to do?");
                actionInput = Console.ReadLine()??"";
                var actionInputWords = actionInput.Split();

                switch (actionInput)
                {
                    case string s when s.StartsWith("buy"):
                        Console.Clear();
                        World.Buy(actionInputWords);
                        break;
                    
                    case string s when s.StartsWith("sell"):
                        Console.Clear();
                        World.Sell(actionInputWords);
                        break;

                    case string s when s.StartsWith("help"):
                        Console.Clear();
                        foreach(KeyValuePair<string, string> kvp in World.commandList)
                            Console.WriteLine(kvp.Key + kvp.Value);
                        break;
                    
                    case string s when s.StartsWith("exit"):
                        Console.Clear();
                        while (true)
                        {
                            Console.WriteLine("Do you want to leave? \ny / n");
                            string userInput = Console.ReadLine()??"";
                            if (userInput == "y")
                                Environment.Exit(0);
                            else if (userInput == "n")
                            {
                                Console.Clear();
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.Write("Invalid input. ");
                            }
                        }
                        break;
                    
                    default:
                        Console.Clear();
                        Console.WriteLine("Unknown command, type 'help', to see availible commands");
                        break;

                }
            }
        }
    

        static class World
        {
            
            static public ulong playerMoney = 10000;
            static public double priceModifier = 1.1;
            static public double sellModifier = 0.5;

            static Vehicle[] transPark = new [] {
                new Vehicle {name = "taxi", price = 50, passangersMax = 1, incomePerPassanger = 5, incomeTimeSeconds = 3, availible = true, fuelCost = 1.5},
                new Vehicle {name = "bus", price = 100, passangersMax = 20, incomePerPassanger = 2, incomeTimeSeconds = 5, fuelCost = 5},
                new Vehicle {name = "trolley", price = 150, passangersMax = 30, incomePerPassanger = 2, incomeTimeSeconds = 7, fuelCost = 3}
                };

            static string[] vehTypes = new string [] {"taxi", "bus", "trolley"};

            public static Dictionary<string, string> commandList = new Dictionary<string, string>() 
            {
                {"buy <name>", "\t - \t buy specified vehicle"},
                {"help\t", "\t - \t show list of commands"},
                {"exit\t", "\t - \t close the game window"}
            };
            
            public static void Update()
            {
                ulong transIncome = 0;

                foreach (var veh in transPark)
                {
                    if (veh.amount > 0)
                    {
                        ulong timePass = (ulong)DateTime.Now.Subtract(new DateTime()).TotalSeconds - veh.timeMoneyGained;
                        ulong rounds = timePass / veh.incomeTimeSeconds;
                        Random rnd = new Random();
                        double passangersAmount = rnd.Next(1, (int)veh.passangersMax);
                        transIncome += (ulong)(rounds *
                                        veh.amount *
                                        veh.incomePerPassanger *
                                        passangersAmount -
                                        veh.fuelCost * 
                                        rounds);
                        veh.timeMoneyGained += rounds * veh.incomeTimeSeconds;
                    }
                }

                playerMoney += transIncome;
            } 

            public static void PrintStatus()
            {
                Console.WriteLine("Your money: {0}.", playerMoney);
                foreach (var veh in transPark)
                {
                    if (veh.availible)
                        Console.WriteLine("You have {0} {1}. To buy 1 more you need {2} money", veh.amount, veh.name, veh.price);
                }
            }

            public static bool CheckMoney(Vehicle veh)
            {
                if (playerMoney >= veh.price)
                    return true;
                Console.WriteLine("Not enough money to buy {0}.", veh.name);
                return false;
            }

            public static void Buy (string[] inputStringWords)
            {
                int index = Array.IndexOf(vehTypes, inputStringWords[inputStringWords.Length-1]);
                Update();
                if (index >= 0 && transPark[index].availible)
                    {
                        if (CheckMoney(transPark[index]))
                            transPark[index].Buy();
                        if (index + 1 < transPark.Length && transPark[index].amount >= transPark[index].unlockNextAmount)
                            transPark[index + 1].availible = true;
                    }
                else Console.WriteLine("You can't buy '{0}'.", inputStringWords[inputStringWords.Length-1]);
            }

            public static void Sell (string[] inputStringWords)
            {
                int index = Array.IndexOf(vehTypes, inputStringWords[inputStringWords.Length-1]);
                Update();
                if (index >= 0 && transPark[index].availible)
                    {
                        transPark[index].Sell();
                        if (index + 1 < transPark.Length && transPark[index].amount <= transPark[index].unlockNextAmount)
                            transPark[index + 1].availible = false;
                    }
                else Console.WriteLine("You can't sell '{0}'.", inputStringWords[inputStringWords.Length-1]);
            }

            public static void Buff ()
            {
                
            }
        }

        public class Vehicle
        {
            public string name = "";
            public uint amount = 0;
            public uint price;
            public uint passangersMax;
            public uint incomePerPassanger;
            public uint incomeTimeSeconds;
            public ulong timeMoneyGained; 
            public bool availible = false;
            public uint unlockNextAmount = 10;
            public double fuelCost;
            

            public void Buy ()
            {
                if (amount == 0)
                    this.timeMoneyGained = (ulong)DateTime.Now.Subtract(new DateTime()).TotalSeconds;
                
                World.playerMoney -= this.price;
                this.amount ++;
                price = (uint)(World.priceModifier * price);
            }

            public void Sell ()
            {
                if (amount == 0)
                {
                    Console.WriteLine("You have 0 units of this vehicle");
                    return;
                }

                amount --;
                World.playerMoney = (uint)(World.playerMoney + price * World.sellModifier);
                price = (uint)(price / World.priceModifier);
            }
        }

        public class Buff
        {
            public uint durationSeconds;
            public DateTime startTime = DateTime.Now;

        }
    }
}
