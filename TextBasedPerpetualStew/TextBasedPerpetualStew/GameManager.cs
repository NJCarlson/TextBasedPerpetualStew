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

    public struct StewSaveFile
    {
        //saved/loaded vars:
        public string tavernName;
        public List<string> eventLog;
        private int curTime;
        private int curDay;
        private int playerGold;
        private int curStewPrice; // player set price of the stew   
        private int curMeatCount; // how much meat the player has
        private int curVeggieCount; // how many veggies the player has
        private int curStewMeat; //how many meats are in the players stew recipe
        private int curStewVeggies; // how many veggies are in the player stew recipe
        private int curStewWater; // how much water is in the player stew recipe - player has infinite water.

        //Player stats
        private int stewSold;
        private int maxGold;
    }



    internal class GameManager
    {
        public bool gameloopRunning = false;
        private string saveDir = AppDomain.CurrentDomain.BaseDirectory;
        private string savePath = "";
        private StewSaveFile data = new StewSaveFile();

        public string title =@"
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

        private static System.Timers.Timer eventTimer;


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
            // Create a timer with a two second interval.
            eventTimer = new System.Timers.Timer(30000);
            eventTimer.Elapsed += EventCheck;
            eventTimer.AutoReset = true;
            eventTimer.Enabled = true;

            while (gameloopRunning)
            {

                DrawScreen(); //draws the main screen, event logs, and commands.


                //main input check
                Console.WriteLine("Input : ");
              
                try
                {
                    string? val = Console.ReadLine();

                    int res;
                    res = Convert.ToInt32(val);

                    if (res > -1 && res <5)
                    {
                        //valid
                        data.eventLog.Add("Command Recieved " + res);
                    }
                    else
                    {
                        data.eventLog.Add("Invalid Input");
                        continue;
                    }

                    switch (res)
                    {
                        case 0:
                            {
                                // Set Stew Ingredients

                            }
                            break;
                        case 1:
                            {
                                //Buy Ingredients

                            }
                            break;
                        case 2:
                            {
                                //See Stats

                            }
                            break;
                        case 3:
                            {
                                //Save & Quit

                            }
                            break;
                        case 4:
                            {
                                //Restart

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


               
                //check if player has enough stew ingredients for another bowl, if not Game over!

                //repeat
            }

        }

        private void SaveGame()
        {
            //save vars to file;

        }

        /// <summary>
        /// Clears the console and writes out the title, event logs, and commands.
        /// </summary>
        private void DrawScreen()
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

        private void EventCheck(Object source, ElapsedEventArgs e)
        {
            //todo determine the event and log the information

            data.eventLog.Add("Event Check hit");

            DrawScreen();

        }


      
    }
}
