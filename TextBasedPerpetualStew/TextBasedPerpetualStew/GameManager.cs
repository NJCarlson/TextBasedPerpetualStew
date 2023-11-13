using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Timers;

namespace TextBasedPerpetualStew
{
    //todos:
    // - input validation & basic game loop
    // - figure out how to determine frequency of customers / customer mechanics 
    // - Text graphics and sound FX
    // - write flavor text
    // - create and balance default vars

    [Serializable]

    /// <summary>
    /// Game save file data
    /// </summary>
    public struct StewSaveFile
    {
        public string tavernName;
        public List<string> eventLog;
        public int curTime;
        public int curDay;

        //Stew recipe 
        public int curStewPrice; // player set price of the stew   
        public int curMeatCount; // how much meat the player has
        public int curVeggieCount; // how many veggies the player has

        //player inventory
        public int playerGold;
        public int curStewMeat; //how many meats are in the players stew recipe
        public int curStewVeggies; // how many veggies are in the player stew recipe
        public int curStewWater; // how much water is in the player stew recipe - player has infinite water.

        //Player stats
        public int stewSold;
        public int maxGold;
    }



    internal class GameManager
    {
        public bool gameloopRunning = false;
        public bool paused = false;
        private string saveDir = AppDomain.CurrentDomain.BaseDirectory;
        private string savePath = "";
        private StewSaveFile data = new StewSaveFile();

        public string title = @"
#   _______                                           __                         __       
#  |       \                                         |  \                       |  \      
#  | $$$$$$$\  ______    ______    ______    ______ _| $$_   __    __   ______  | $$      
#  | $$__/ $$ /      \  /      \  /      \  /      \   $$ \ |  \  |  \ |      \ | $$      
#  | $$    $$|  $$$$$$\|  $$$$$$\|  $$$$$$\|  $$$$$$\$$$$$$ | $$  | $$  \$$$$$$\| $$      
#  | $$$$$$$ | $$    $$| $$   \$$| $$  | $$| $$    $$| $$ __| $$  | $$ /      $$| $$      
#  | $$      | $$$$$$$$| $$      | $$__/ $$| $$$$$$$$| $$|  \ $$__/ $$|  $$$$$$$| $$      
#  | $$       \$$     \| $$      | $$    $$ \$$     \ \$$  $$\$$    $$ \$$    $$| $$      
#   \$$        \$$$$$$$ \$$      | $$$$$$$   \$$$$$$$  \$$$$  \$$$$$$   \$$$$$$$ \$$      
#    ______     __               | $$                                                     
#   /      \   |  \              | $$                                                     
#  |  $$$$$$\ _| $$_    ______   _\$$ __   __                                             
#  | $$___\$$|   $$ \  /      \ |  \ |  \ |  \                                            
#   \$$    \  \$$$$$$ |  $$$$$$\| $$ | $$ | $$                                            
#   _\$$$$$$\  | $$ __| $$    $$| $$ | $$ | $$                                            
#  |  \__| $$  | $$|  \ $$$$$$$$| $$_/ $$_/ $$                                            
#   \$$    $$   \$$  $$\$$     \ \$$   $$   $$                                            
#    \$$$$$$     \$$$$  \$$$$$$$  \$$$$$\$$$$                                                                                                                                      

";
        public string mainOptions = @"
Options:
[0] Set Stew Ingredients
[1] Buy Ingredients
[2] See Stats
[3] Save & Quit
[4] Restart
";
        public string BuyIngredientOptions = @"
Options:
[1] Buy Meat
[2] Buy Veggies
[3] Exit
";
        public string SetIngredientOptions = @"
Options:
[1] Set Meat Quantity
[2] Set Veggie Quantity
[3] Set Water Quantity
[4] Exit
";

        private static System.Timers.Timer eventTimer = new System.Timers.Timer();

        public void Start()
        {
            data.eventLog = new List<string>();
            Console.Write(title);
            System.Threading.Thread.Sleep(1000);

            string[] files = System.IO.Directory.GetFiles(saveDir, "*.stew");

            if (files.Length > 0)
            {
                savePath = files[0];
                ContinueGame();
            }
            else
            {
                StartNewGame();
            }

        }

        private void StartNewGame()
        {

            //initalize all new vars to default;

            Console.WriteLine("Hello, Welcome to Perpetual Stew! A Text based inn keeper simulation game!");
            Console.WriteLine("Please Enter the name of your new Tavern : ");

            //todo generate a default tavern name
            string mTavernName = Console.ReadLine(); //do input validation

            if (!string.IsNullOrEmpty(mTavernName))
            {
                data.tavernName = mTavernName;
            }


            gameloopRunning = true;
            GameLoop();
        }

        private void ContinueGame()
        {
            //todo load all vars from file;


            gameloopRunning = true;
            GameLoop();
        }

        private void GameLoop()
        {
            // Create a timer for events,
            // so player doesnt have to input for game to update
            eventTimer = new System.Timers.Timer(30000);
            eventTimer.Elapsed += CustomerEventCheck;
            eventTimer.AutoReset = true;
            eventTimer.Enabled = true;

            while (gameloopRunning)
            {
                DrawMainScreen(); //draws the main screen, event logs, and commands.

                MainMenuInput();

                //check if player has enough stew ingredients for another bowl, if not Game over!
            }

        }

        /// <summary>
        /// Save all game vars to text file
        /// </summary>
        private void SaveGame()
        {
            //todo save vars to file;


        }

        private void SaveAndQuit()
        {

            //Save & Quit
            Console.Clear();
            Console.Write(title);
            Console.WriteLine("Are you sure you want to save and Exit?");
            Console.WriteLine("Input : ");
            Console.ReadLine();

            var val = Console.ReadLine();
            int res = Convert.ToInt32(val);

            if (res > -1 && res < 5)
            {
                //valid
                data.eventLog.Add("Command Recieved " + res);
            }
            else
            {
                data.eventLog.Add("Invalid Input");
                return;
            }

            switch (res)
            {
                case 0: //no
                    {
                        SaveGame();
                    }
                    break;
                case 1: //yes
                    {
                        SaveGame();
                        Environment.Exit(0);
                    }
                    break;
            }

        }

        private void Restart()
        {

            //Restart - ask player if they are sure they want to delete file
            Console.Clear();
            Console.Write(title);
            Console.WriteLine("Are you sure you want to restart? This can not be undone.");
            Console.WriteLine("Input : ");
            Console.ReadLine();
            var val = Console.ReadLine();
            int res = Convert.ToInt32(val);
            if (res > -1 && res < 5)
            {
                //valid
                data.eventLog.Add("Command Recieved " + res);
            }
            else
            {
                data.eventLog.Add("Invalid Input");
                return;
            }
            switch (res)
            {
                case 0:
                    break;
                case 1:
                    break;
            }


        }


        /// <summary>
        /// Clears the console and writes out the title, event logs, and commands.
        /// </summary>
        private void DrawMainScreen()
        {
            Console.Clear();
            Console.Write(title);

            //write out all logs
            //todo limit this?
            if (data.eventLog != null && data.eventLog.Count > 0)
            {
                foreach (var log in data.eventLog)
                {
                    Console.WriteLine(log);
                }
            }

            //check for player input:
            Console.Write(mainOptions);
        }

        private void MainMenuInput()
        {
            //main input check
            Console.WriteLine("Input : ");

            try
            {
                string? val = Console.ReadLine();

                int res;
                res = Convert.ToInt32(val);

                if (res > -1 && res < 5)
                {
                    //valid
                    data.eventLog.Add("Command Recieved " + res);
                }
                else
                {
                    data.eventLog.Add("Invalid Input");
                    return;
                }

                // pause timed events to do player input;
                paused = true;

                switch (res)
                {
                    case 0:
                        {
                            BuyIngredientInput();
                        }
                        break;
                    case 1:
                        {
                            SetIngredientInput();
                        }
                        break;
                    case 2:
                        {
                            //See Stats
                            DrawStats();
                        }
                        break;
                    case 3:
                        {
                            SaveAndQuit();

                        }
                        break;
                    case 4:
                        {
                            Restart();
                        }
                        break;
                    default:
                        {
                            data.eventLog.Add("Invalid Input");
                        }
                        break;
                }


            }
            catch (Exception)
            {
                data.eventLog.Add("Invalid Input");
            }

            paused = false;

        }

        private void BuyIngredientInput()
        {
            bool shopping = true;
            while (shopping)
            {
                //Buy Ingredients
                Console.Clear();
                Console.Write(title);
                Console.WriteLine("Meat costs 1 gold");
                Console.WriteLine("Vegtables costs 1 gold");
                Console.Write(BuyIngredientOptions);
                Console.WriteLine("Input : ");

                var val = Console.ReadLine();
                int res = Convert.ToInt32(val);

                if (res > -1 && res < 5)
                {
                    //valid
                    data.eventLog.Add("Command Recieved " + res);
                }
                else
                {
                    data.eventLog.Add("Invalid Input");
                    return;
                }

                switch (res)
                {
                    case 0:
                        {
                            //buy meat
                            Console.WriteLine("How much meat would you like to purchase?");
                            var meatStr = Console.ReadLine();
                            int meatQuantity = Convert.ToInt32(meatStr);

                            if (data.playerGold >= meatQuantity)
                            {
                                data.playerGold -= meatQuantity;
                                data.curMeatCount += meatQuantity;
                            }
                            else
                            {
                                Console.WriteLine("You don't have enough gold!!");
                            }
                        }
                        break;
                    case 1:
                        {
                            //buy veggies
                            Console.WriteLine("How many veggies would you like to purchase?");
                            var veggieStr = Console.ReadLine();
                            int veggieQuantity = Convert.ToInt32(veggieStr);

                            if (data.playerGold >= veggieQuantity)
                            {
                                data.playerGold -= veggieQuantity;
                                data.curMeatCount += veggieQuantity;
                            }
                            else
                            {
                                Console.WriteLine("You don't have enough gold!!");
                            }
                        }
                        break;
                    case 2:
                        shopping = false;
                        break;
                }
            }

        }

        private void SetIngredientInput()
        {
            bool cooking = true;
            while (cooking)
            {

                // Set Stew Ingredients
                Console.Clear();
                Console.Write(title);
                Console.Write(SetIngredientOptions);

                var val = Console.ReadLine();
                int res = Convert.ToInt32(val);

                if (res > -1 && res < 5)
                {
                    //valid
                    data.eventLog.Add("Command Recieved " + res);
                }
                else
                {
                    data.eventLog.Add("Invalid Input");
                    return;
                }

                switch (res)
                {
                    case 0:
                        {
                            //set meat

                            Console.WriteLine("Meat in stew :");

                            var quantityStr = Console.ReadLine();
                            int quantity = Convert.ToInt32(quantityStr);

                            data.curStewMeat = quantity;

                        }
                        break;
                    case 1:
                        {
                            //set veggies

                            Console.WriteLine("Veggies in stew :");

                            var quantityStr = Console.ReadLine();
                            int quantity = Convert.ToInt32(quantityStr);

                            data.curStewVeggies = quantity;
                        }
                        break;
                    case 2:
                        {
                            //set water

                            Console.WriteLine("Water in stew :");

                            var quantityStr = Console.ReadLine();
                            int quantity = Convert.ToInt32(quantityStr);

                            data.curStewWater = quantity;
                        }
                        break;
                    case 3:
                        {
                            //exit
                            cooking = false;
                        }
                        break;
                    default:
                        break;
                }
            }


        }

        private void CustomerEventCheck(Object source, ElapsedEventArgs e)
        {
            //todo determine the event and log the information

            if (!paused)
            {
                data.eventLog.Add("Event Check hit");

                DrawMainScreen();
            }

        }

        private void DrawStats()
        {

        }


    }
}
