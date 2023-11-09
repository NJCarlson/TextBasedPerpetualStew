using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedPerpetualStew
{
    internal class GameManager
    {
        public bool gameloopRunning = false;
        private string saveDir;
        public string tavernName;

        //saved/loaded vars:
        private int curTime;
        private int curDay;
        private int playerGold;
        
        //todo determine stew ingredients/system

       
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
                //clear screen / print time, day, current gold and current stew price

                //check if a customer has entered

                //if so determine if they buy stew

                //if they dont, tell the player why

                //if they do, tell the player if they liked it or not

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
      


    }
}
