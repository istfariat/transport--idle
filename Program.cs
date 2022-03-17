using System;

namespace transport_idle
{
    class Program
    {
        
        static Vehicle[] transPark = new [] {
            new Vehicle {name = "taxi", price = 90, passangersMax = 1, incomePerPassanger = 5, incomeTimeSeconds = 3},
            new Vehicle {name = "bus", price = 20, passangersMax = 20, incomePerPassanger = 2, incomeTimeSeconds = 5},
            new Vehicle {name = "trolley", price = 30, passangersMax = 30, incomePerPassanger = 2, incomeTimeSeconds = 7}
            };
        static void Main()
        {
            var timeUpdated = DateTime.Now;
            string actionInput;
            
            while (true)
            {
                World.Update(transPark);
                World.PrintStatus(transPark);
                Console.WriteLine("What to do?");
                actionInput = Console.ReadLine()??"";

                if (actionInput == "buy taxi" && World.CheckMoney(transPark[0]))
                    {
                        
                        transPark[0].Buy();
                        World.PrintStatus(transPark);
                    }
                else if(actionInput == "exit")
                    break;
                else
                    Console.WriteLine("Unknown command, to buy taxi type 'buy taxi', to exit type 'exit'");
            }

            Console.WriteLine("Press ENTER to exit...");
            actionInput = Console.ReadLine()??"";
        }
    }

    static class World
    {
        
        static public ulong playerMoney = 100;
        
        public static void Update(Vehicle[] transPark)
        {
            ulong transIncome = 0;

            foreach (var veh in transPark)
            {
                if (veh.amount > 0)
                {
                    ulong timePass = (ulong)DateTime.Now.Subtract(new DateTime()).TotalSeconds - veh.timeMoneyGained;
                    ulong rounds = timePass / veh.incomeTimeSeconds;
                    transIncome += rounds *
                                    veh.amount *
                                    veh.incomePerPassanger *
                                    veh.passangersMax;
                    veh.timeMoneyGained += rounds * veh.incomeTimeSeconds;
                }
            }
                

            playerMoney += transIncome;
        } 

        public static void PrintStatus(Vehicle[] transPark)
        {
            Console.WriteLine("Your money: {0}\nYou have {1} taxi.", playerMoney, transPark[0].amount);
        }

        public static bool CheckMoney(Vehicle veh)
        {
            if (playerMoney >= veh.price)
                return true;
            return false;
        }
    }

    public class Vehicle
    {
        public string name = "";
        public  uint amount = 0;
        public uint price;
        public uint passangersMax;
        public uint incomePerPassanger;
        public uint incomeTimeSeconds;
        public ulong timeMoneyGained; 
        

        public void Buy ()
        {
            if (amount == 0)
                this.timeMoneyGained = (ulong)DateTime.Now.Subtract(new DateTime()).TotalSeconds;
            
            World.playerMoney -= this.price;
            this.amount ++;
        }
    }

}