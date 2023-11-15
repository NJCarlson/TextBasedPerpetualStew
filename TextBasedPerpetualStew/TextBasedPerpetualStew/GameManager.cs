using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace TextBasedPerpetualStew
{
    //todos:
    // - input validation & basic game loop
    // - figure out how to determine frequency of customers / customer mechanics 
    // - Text graphics and sound FX
    // - write flavor text
    // - create and balance default vars

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
[0] Buy Meat
[1] Buy Veggies
[2] Exit
";
        public string SetIngredientOptions = @"
Options:
[0] Set Meat Quantity
[1] Set Veggie Quantity
[2] Set Water Quantity
[3] Exit
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
                ContinueGame(savePath);
            }
            else
            {
                StartNewGame();
            }

        }

        private void StartNewGame()
        {

            //initalize all new vars to default;
            data = new StewSaveFile();
            data.eventLog = new List<string>();
            data.curDay = 1;
            data.curTime = 0;
            data.curStewWater = 1;
            data.curStewVeggies = 1;
            data.curStewMeat = 1;
            data.curMeatCount = 10;
            data.curVeggieCount = 10;
            data.playerGold = 10;

            bool validNameFound = false;
            bool invalidInput = false;

            while (!validNameFound)
            {
                Console.Clear();
                Console.WriteLine(title);
                Console.WriteLine("Hello, Welcome to Perpetual Stew! A Text based inn keeper simulation game!");

                if (invalidInput)
                {
                    Console.WriteLine("Invalid name entered!");
                }

                Console.WriteLine("Please Enter the name of your new Tavern : ");

                //todo generate a default tavern name?
                var mTavernName = Console.ReadLine(); //do input validation

                if (!string.IsNullOrEmpty(mTavernName))
                {
                    data.tavernName = mTavernName;
                    validNameFound = true;
                }
                else
                {
                    invalidInput = true;
                }
            }

            gameloopRunning = true;
            GameLoop();
        }

        private void ContinueGame(string savePath)
        {
            //todo load all vars from file;
            LoadStewFile(savePath);

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

                if (data.curMeatCount < data.curStewMeat || data.curVeggieCount < data.curStewVeggies)
                {
                    Console.WriteLine("Game Over!");
                }

                
                SaveGame();

            }

        }

        /// <summary>
        /// Save all game vars to text file
        /// </summary>
        private void SaveGame()
        {
            savePath = AppDomain.CurrentDomain.BaseDirectory + data.tavernName + ".stew";

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            //todo save vars to file;
            using (var stream = File.Open(savePath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(data.tavernName);
                    writer.Write(data.playerGold);
                    writer.Write(data.curDay);
                    writer.Write(data.curTime);
                    writer.Write(data.curStewMeat);
                    writer.Write(data.curStewVeggies);
                    writer.Write(data.curStewWater);
                    writer.Write(data.curMeatCount);
                    writer.Write(data.curVeggieCount);
                    writer.Write(data.curStewPrice);
                }

            }

            
        }

        private void LoadStewFile(string savePath)
        {
            data = new StewSaveFile();

            if (File.Exists(savePath))
            {
                using (var stream = File.Open(savePath, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        //reader.ReadSingle();
                        //reader.ReadString();
                        //reader.ReadInt32();
                        //reader.ReadBoolean();

                        data.tavernName = reader.ReadString();
                        data.playerGold = reader.ReadInt32();
                        data.curDay = reader.ReadInt32();
                        data.curTime = reader.ReadInt32();
                        data.curStewMeat = reader.ReadInt32();
                        data.curStewVeggies = reader.ReadInt32();
                        data.curStewWater = reader.ReadInt32();
                        data.curMeatCount = reader.ReadInt32();
                        data.curVeggieCount = reader.ReadInt32();
                        data.curStewPrice = reader.ReadInt32();

                    }
                }
            }
        }

        private void SaveAndQuit()
        {

            //Save & Quit
            Console.Clear();
            Console.Write(title);
            Console.WriteLine("Are you sure you want to save and Exit?");
            Console.WriteLine("[0] No");
            Console.WriteLine("[1] Yes");
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
            Console.WriteLine("[0] NO ");
            Console.WriteLine("[1] YES ");
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
                    {
                        return;
                    }
                    break;
                case 1:
                    {
                        if (File.Exists(savePath))
                        {
                            File.Delete(savePath);
                        }

                        // Starts a new instance of the program itself
                        var fileName = Assembly.GetExecutingAssembly().Location;
                        System.Diagnostics.Process.Start(fileName);

                        // Closes the current process
                        Environment.Exit(0);
                    }
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
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine(data.tavernName + " Stew Recipe: ");
            Console.WriteLine("Meat: " + data.curStewMeat);
            Console.WriteLine("Veggies: " + data.curStewVeggies);
            Console.WriteLine("Water: " + data.curStewWater);
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Day " + data.curDay + " Time " + data.curTime);
            Console.WriteLine(data.playerGold + " Gold");
            Console.WriteLine("Log: ");

            //write out all logs
            //todo limit this?
            if (data.eventLog != null && data.eventLog.Count > 0)
            {
                foreach (var log in data.eventLog)
                {
                    Console.WriteLine(log);
                }
            }
            Console.WriteLine("-----------------------------------------");

            //Write out main menu options
            Console.Write(mainOptions);
        }

        private void MainMenuInput()
        {
            //main input check
            Console.Write("Input : ");

            try
            {
                string? val = Console.ReadLine();

                int res;
                res = Convert.ToInt32(val);

                if (res > -1 && res < 5)
                {
                    //input was valid

                    //uncomment this for debug logging of commands:
                    // data.eventLog.Add("Command Recieved " + res);
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
                            SetIngrediens();
                        }
                        break;
                    case 1:
                        {
                            BuyIngredients();
                        
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

        private void BuyIngredients()
        {
            bool shopping = true;
            List<string> shopNotifications = new List<string>();
            while (shopping)
            {
                //Buy Ingredients
                Console.Clear();
                Console.Write(title);
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("Inventory : ");
                Console.WriteLine(data.playerGold + " Gold");
                Console.WriteLine("Meat: " + data.curMeatCount);
                Console.WriteLine("Veggies: " + data.curVeggieCount);
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("Meat costs 1 gold");
                Console.WriteLine("Vegtables costs 1 gold");
                Console.WriteLine("----------------------------------------------------");
                Console.Write(BuyIngredientOptions);

                //write notifications:
                foreach (var item in shopNotifications)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine("Input : ");

                var val = Console.ReadLine();
                int res = Convert.ToInt32(val);

                if (res > -1 && res < 3)
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
                                shopNotifications.Add("You don't have enough gold!!");
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
                                shopNotifications.Add("You don't have enough gold!!");
                            }
                        }
                        break;
                    case 2:
                        shopping = false;
                        break;
                }

                SaveGame();
            }

        }

        private void SetIngrediens()
        {
            bool cooking = true;
            List<string> cookingNotifications = new List<string>();
            while (cooking)
            {
                // Set Stew Ingredients
                Console.Clear();
                Console.Write(title);
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine( data.tavernName + " Stew Recipe: ");
                Console.WriteLine("Meat: " + data.curStewMeat);
                Console.WriteLine("Veggies: " + data.curStewVeggies);
                Console.WriteLine("Water: " + data.curStewWater);
                Console.WriteLine("-----------------------------------------");
                Console.Write(SetIngredientOptions);

                foreach (var item in cookingNotifications)
                {
                    Console.WriteLine(item);
                }

                var val = Console.ReadLine();
                int res = Convert.ToInt32(val);

                if (res > -1 && res < 4)
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
                            if (quantity > data.curMeatCount)
                            {
                                cookingNotifications.Add("You don't even have that many!");
                            }
                            else
                            {
                                data.curStewMeat = quantity;
                            }

                        }
                        break;
                    case 1:
                        {
                            //set veggies

                            Console.WriteLine("Veggies in stew :");

                            var quantityStr = Console.ReadLine();
                            int quantity = Convert.ToInt32(quantityStr);

                            if (quantity > data.curVeggieCount)
                            {
                                cookingNotifications.Add("You don't even have that many!");
                            }
                            else
                            {
                                data.curStewVeggies = quantity;
                            }

                          
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

                SaveGame();
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
