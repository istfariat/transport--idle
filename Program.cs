using System;

namespace transport_idle
{
    class Program
    {
        
        static Vehicles[] transPark;
        static void Main()
        {
            // int baseIncome = 2;
            // double income = baseIncome;
            var timeUpdated = DateTime.Now;
            
            transPark = new [] {
            new Vehicles {name = "taxi", price = 90, passangersMax = 1, incomePerPassanger = 5, incomeTimeSeconds = 3},
            new Vehicles {name = "bus", price = 20, passangersMax = 20, incomePerPassanger = 2, incomeTimeSeconds = 5},
            new Vehicles {name = "trolley", price = 30, passangersMax = 30, incomePerPassanger = 2, incomeTimeSeconds = 7}
            };
            
            string actionInput;
            
            while (true)
            {
                World.Update(transPark);
                World.Status(transPark);
                Console.WriteLine("What to do?");
                actionInput = Console.ReadLine();
                
                if (actionInput == "buy taxi" && World.CheckMoney(transPark[0]))
                    {
                        
                        transPark[0].Buy();
                        World.Status(transPark);
                    }
                // else if (actionInput == "buy taxi" && money < taxiPrice)
                    // Console.WriteLine("Not enough money\nYou have {0} coins\nTaxi costs {1} coins", money, taxiPrice);
                else if(actionInput == "exit")
                    break;
                else
                    Console.WriteLine("Unknown command, to buy taxi type 'buy taxi', to exit type 'exit'");
                // actionInput = Console.ReadLine();
            }

            Console.WriteLine("Press ENTER to exit...");
            actionInput = Console.ReadLine();
        }
    }

    static class World
    {
        static DateTime timeUpdated = DateTime.Now;
        static int timePass;
        static int timeDeltaLast;
        static public int playerMoney = 100;

        public static void Update(Vehicles[] transPark)
        {
            timeDeltaLast = timePass;            
            timePass = (int)DateTime.Now.Subtract(timeUpdated).TotalSeconds;
            timeUpdated = DateTime.Now;
            int transIncome = 0;


            foreach (var veh in transPark)
            {
                transIncome += (timePass + veh.timeOnRoute) * veh.amount * veh.incomePerPassanger * veh.passangersMax / veh.incomeTimeSeconds;
                if (veh.amount > 0)
                    veh.timeOnRoute = (veh.timeOnRoute + timePass) % veh.incomeTimeSeconds;
            }

            playerMoney += transIncome;
        } 

        public static void Status(Vehicles[] transPark)
        {
            Console.WriteLine("Your money: {0}\nYou have {1} taxi.", playerMoney, transPark[0].amount);
        }

        public static bool CheckMoney(Vehicles veh)
        {
            if (playerMoney >= veh.price)
                return true;
            return false;
        }
    }

    public class Vehicles
    {
        public string name = "";
        public  int amount = 0;
        public int price;
        public int passangersMax;
        public int incomePerPassanger;
        public int incomeTimeSeconds;
        public int timeOnRoute = 0;
        

        public int Buy ()
        {
            World.playerMoney -= this.price;
            return this.amount ++;
        }
    }

}