using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	class Program
	{
		static int games = 500;
		static int maxTurns = 20;

		//Play games # of games, using and updating the AI qTable
		//Currently 1Player
		static void Main(string[] args)
		{
			Card[] cardList = loadCards();
			int total = 0;
			double avg;
			for (int i = 0; i < games; i++)
			{
				if (i != 0) avg = (double)total / (double)i;
				else avg = 3;
				total += playGame(cardList, false, "try3.txt", true, (int)avg);
			}
			avg = (double)total / games;
			Console.WriteLine("\n\nAverage VP: {0}", avg);
		}

		//Creates a list of all the cards in the game
		static Card[] loadCards()
		{
			Card[] cardList = new Card[16];
			cardList[0] = new CardActivity("Village", 3, 2, 1, 0, 0, 0, "");
			cardList[1] = new CardActivity("Laboratory", 5, 1, 2, 0, 0, 0, "");
			cardList[2] = new CardActivity("Smithy", 4, 0, 3, 0, 0, 0, "");										//No extra actions
			cardList[3] = new CardActivity("Market", 5, 1, 1, 1, 1, 0, "");
			cardList[4] = new CardActivity("Festival", 5, 2, 0, 1, 2, 0, "");
			cardList[5] = new CardActivity("Woodcutter", 3, 0, 0, 1, 2, 0, "");									//No extra actions
			cardList[6] = new CardActivity("Council Room", 5, 0, 4, 1, 0, .25, "Each other player draws a card.");//No extra actions
			cardList[7] = new CardActivity("Spy", 4, 1, 1, 0, 0, -.25, "Each player (including you) reveals the top card of his deck and either discards it or puts it back, your choice.");
			cardList[8] = new CardActivity("Witch", 5, 0, 2, 0, 0, -1, "Each other player gains a Curse card."); //No extra actions
			cardList[9] = new CardActivity("Cellar", 2, 1, 0, 0, 0, 0, "Discard any number of cards. +1 Card per card discarded.");
			cardList[10] = new CardMoney("Copper", 1, 0);
			cardList[11] = new CardMoney("Silver", 2, 3);
			cardList[12] = new CardMoney("Gold", 3, 6);
			cardList[13] = new CardVictory("Estate", 1, 2);
			cardList[14] = new CardVictory("Duchy", 3, 5);
			cardList[15] = new CardVictory("Province", 6, 8);
			//cardList[10] = new Card("Adventurer", 6, 0, 0, 0, 0, 0, "Reveal cards from your deck until you reveal 2 Treasure cards. Put those Treasure cards into your hand and discard the other revealed cards.");
			//cardList[11] = new Card("Throne Room", 4, 0, 0, 0, 0, 0, "Choose an Action card in your hand. Play it twice.");
			return cardList;
		}

		//Plays one single player game using a given AI at path, and saving updates to it
		//If useAI==false, don't load the AI, just save to it
		//To update the AI better, give it it's prior average scores
		static int playGame(Card[] cardList, bool print, string path, bool useAI, int avg)
		{
			Player p1 = new Player(cardList, print);
			if (useAI) p1.loadQTable(path);

			p1.shuffleDiscardIntoDeck();
			if (print)
			{
				Console.WriteLine("Hand before:");
				p1.printHand();
				Console.WriteLine("Deck before:");
				p1.printHand();
				Console.WriteLine();
			}
			p1.drawCards(5);

			for (int i = 0; i < maxTurns; i++)
				p1.takeTurn(i, cardList);
			if (print)	p1.printHandDeck(true);
			if (print)	Console.WriteLine();
			p1.printVP();
			if (useAI) p1.adjustQTable(cardList, avg);
			p1.saveQTable(path);

			return p1.getVP();
		}
	}
}
