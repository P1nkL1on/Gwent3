using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth * 2 / 3, Console.LargestWindowHeight * 2 / 3);

            SPlay play = new SPlay(
                new SPlayers(new SPlayer(), new SPlayer()),
                new SCards(SCard.testArcher, SCard.testArcher, SCard.testSapper, SCard.testArcher, SCard.testArcher, SCard.testSapper),
                new SCards(SCard.testArcher, SCard.testArcher, SCard.testSkirmisher, SCard.testSkirmisher));

            play.start();
            Console.ReadKey();
        }
    }
}
