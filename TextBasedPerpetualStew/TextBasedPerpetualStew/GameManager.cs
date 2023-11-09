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
        private string saveDir;


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
            //load from save if it exists
            //ContinueGame();
            //else
            //start new game
            //StartNewGame();
        }

        private void StartNewGame()
        {
            //load vars from file

            //if no save file exists:
            Console.WriteLine("Hello, Welcome to Perpetual Stew! A Text based inn keeper simulation game!");
            Console.WriteLine("Please Enter the name of your new Tavern : ");
            string mTavernName = Console.ReadLine(); //do input validation
            tavernName = mTavernName;


            gameloopRunning = true;
            GameLoop();
        }

        private void ContinueGame()
        {
            //initalize all new vars to default;


            gameloopRunning = true;
            GameLoop();
        }

        private void GameLoop()
        {
            while (gameloopRunning)
            {
                //clear screen / print time, day, current gold
                //current stew servings left and the set price

               //customer events

                //check for player input:
                //Player Command Options:
                //[0] Set Stew Ingredients
                //[1] Buy Ingredients
                //[2] See Stats
                //[3] Save & Quit
                //[4] Restart

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
