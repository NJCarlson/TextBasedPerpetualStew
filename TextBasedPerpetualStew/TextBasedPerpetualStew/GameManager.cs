using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TextBasedPerpetualStew
{
    //todos:
    // - input validation & basic game loop
    // - figure out how to determine frequency of customers / customer mechanics 
    // - Text graphics and sound FX
    // - write flavor text
    // - create and balance default vars


    internal class GameManager
    {
        public bool gameloopRunning = false;
        private string saveDir = AppDomain.CurrentDomain.BaseDirectory;
        private string savePath = "";

        //saved/loaded vars:
        public string tavernName;
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

        public void Start()
        {
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
            string mTavernName = Console.ReadLine(); //do input validation
            tavernName = mTavernName;

            gameloopRunning = true;
            GameLoop();
        }

        private void ContinueGame()
        {
            //load all vars from file;



            gameloopRunning = true;
            GameLoop();
        }

        private void GameLoop()
        {
            while (gameloopRunning)
            {

                //clear screen / print time, day, current gold
                //current stew servings left and the set price
                Console.Clear();

                //customer events

                //check for player input:

                Console.Write(@"
                Options:
                [0] Set Stew Ingredients
                [1] Buy Ingredients
                [2] See Stats
                [3] Save & Quit
                [4] Restart" );

                try
                {
                    string val;
                    val = Console.ReadLine();

                    int res;
                    res = Convert.ToInt32(val);
                }
                catch (Exception)
                {
                    
                }


                //check if player has enough stew ingredients for another bowl, if not Game over!

                //repeat
            }
          
        }

        private void SaveGame()
        {
            //save vars to file;

        }
      
        private void CustomerEvents()
        {
            //check if a customer has entered

            //if so determine if they buy stew

            //if they dont, tell the player why

            //if they do, tell the player if they liked it or not
        }



    }
}
