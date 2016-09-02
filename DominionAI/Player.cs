using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	public class Player
	{
		public int num;
		PlayerStats stats;
		QTable qtable;
		List<Card> hand;
		List<Card> deck;
		List<Card> discard;
		List<Purchase> purchases;

		public bool print;
		static bool printDiscard = true;	//Debug value
		static int uniqueCards = 16;

		//Construct Player with the cardlist, and whether to use debug printing
		public Player(Card[] cardList, bool _print)
		{
			qtable = new QTable();
			hand = new List<Card>();
			deck = new List<Card>();
			discard = new List<Card>();
			purchases = new List<Purchase>();

			print = _print;
			//Starting hand
			for (int i = 0; i < 7; i++)		
				deck.Add(cardList[10]);		//Coppers
			for (int i = 0; i < 3; i++)		
				deck.Add(cardList[13]);		//Estates
				//deck.Add(cardList[0]);		//Village
			//for (int i = 0; i < 1; i++)		
				//deck.Add(cardList[1]);		//Laboratory
				//deck.Add(cardList[3]);		//Market
		}

		//Draw n cards
		public void drawCards(int n)
		{
			for (int i = 0; i < n; i++)
			{
				if (deck.Count == 0)
					shuffleDiscardIntoDeck();
				if (deck.Count == 0)
					break;
				hand.Add(deck[0]);
				deck.RemoveAt(0);
			}
		}

		//Discard the nth card in hand
		public void discardCard(int n)
		{
			discard.Add(hand[n]);
			hand.RemoveAt(n);
		}

		//Discard the whole hand
		public void discardHand()
		{
			while (hand.Count > 0)
				discardCard(0);
		}

		//Shuffles the discard into your deck
		public void shuffleDiscardIntoDeck()
		{
			Random r = new Random();
			int rand;
			while (deck.Count > 0)	//Put deck into discard
			{
				discard.Add(deck[0]);
				deck.RemoveAt(0);
			}
			while (discard.Count > 0)	//Randomly put discard into deck
			{
				rand = r.Next(discard.Count);
				deck.Add(discard[rand]);
				discard.RemoveAt(rand);
			}
		}

		//Debug print out the hand
		public void printHand()
		{
			for (int i = 0; i < hand.Count; i++)
			{
				Console.WriteLine("{0}: {1}", i, hand[i].name);
			}
		}

		//Debug print out the deck
		public void printDeck()
		{
			for (int i = 0; i < deck.Count; i++)
			{
				Console.WriteLine("{0}: {1}", i, deck[i].name);
			}
		}

		//Debug print out the current hand, deck, and if disc also discard, as well as current state
		public void printHandDeck(bool disc)
		{
			if(!disc)
			for (int i = 0; (i < hand.Count || i < deck.Count); i++)
			{
				if (i < hand.Count)
					Console.Write("{0}: {1}", i, hand[i].name);
				else
					Console.Write("   \t");
				if (i < deck.Count)
					Console.WriteLine("\t\t\t{0}: {1}", i, deck[i].name);
				else
					Console.WriteLine();
			}
			else
			for (int i = 0; (i < hand.Count || i < deck.Count || i < discard.Count); i++)
			{
				if (i < hand.Count)
					Console.Write("{0}: {1}", i, hand[i].name);
				else
					Console.Write("   \t");
				if (i < deck.Count)
					Console.Write("\t\t{0}: {1}", i, deck[i].name);
				else
					Console.Write("   \t\t\t");
				if (i < discard.Count)
					Console.WriteLine("\t\t\t{0}: {1}", i, discard[i].name);
				else
					Console.WriteLine();
			}

			Console.WriteLine("\tActions: {0}\t\tBuys: {1}\tGOLD: {2}", stats.actions, stats.buys, stats.gold);
		}

		//Take 1 turn, given the turn number for the Ai and the list of cards
		public void takeTurn(int turn, Card[] cardList)
		{
			stats = new PlayerStats();
			if (print) Console.WriteLine("Hand at start of turn {0}:", turn);
			if (print) printHandDeck(printDiscard);

			//Take actions
			playHand();

			//Buy things
			buy(turn, cardList);

			discardHand();
			drawCards(5);

			//Console.WriteLine("Hand at end of turn {0}:", turn);
			//printHandDeck(printDiscard);

			if (print) Console.Write("\n\n");
		}

		//Plays the hand to get the optimal gold amount for buying, or altering the vp status quo as much as possible
		public void playHand()
		{
			//hand = hand.OrderBy(o => o.actions).ToList();	//Sorts by ascending actions, 0 first

			int i;
			do
			{
				if (stats.gold > 100)
					break;	//If we somehow get this high, we can almost certainly get infinite gold, but lets not go that far
				i = 0;
				while ((i < hand.Count) && (stats.actions > 0))	//Play multi-action cards first
				{
					if (stats.actions > 10)
						break;

					if (hand[i].actions >= 1)
						playCard(i);
					else
						i++;
				}

				i = 0;
				//Select the best remaining action card to play with our actions
				if ((stats.actions > 1) && (deck.Count + discard.Count > 0))
				{
					int bestCard = findBestCard("cards");
					if (bestCard != -1)
						if (playCard(bestCard) == true)
							continue;
				}
				
				//Play the card that gives the most buys
				if ((stats.gold > 5) && (stats.actions > 0))	//CAN CHANGE THE NUMBER 5
				{
					int bestCard = findBestCard("buys");
					if (bestCard != -1)
						if (playCard(bestCard) == true)
							continue;
				}
				//Play the card that gives the most gold
				else if (stats.actions > 0)
				{
					int bestCard = findBestCard("gold");
					if (bestCard != -1)
						if (playCard(bestCard) == true)
							continue;
				}
				//Play the card that gives the most draws
				if ((stats.actions > 0) && (deck.Count + discard.Count > 0))
				{
					int bestCard = findBestCard("cards");
					if (bestCard != -1)
						if (playCard(bestCard) == true)
							continue;
				}

				if(stats.actions==0)
					break;
				int hasAction = findBestCard("firstAction");
				if (hasAction > -1)
					if (playCard(hasAction) == true)
						continue;
				if (hasAction == -1)
					break;
				if (stats.actions == 0)
					break;
			} while (true);
		}

		//Plays the nth card in hand
		public bool playCard(int n)
		{
			bool isMoney = (hand[n].cardType == "money");
			if (!isMoney)
				if (print) Console.WriteLine("PLAYING CARD: {0}", hand[n].name);
			if (!isMoney)
				stats.actions--;
			stats.actions += hand[n].actions;
			stats.buys += hand[n].buys;
			stats.effect += hand[n].vpEffect;
			stats.gold += hand[n].gold;
			int cardsToDraw = hand[n].cards;

			discardCard(n);

			drawCards(cardsToDraw);

			//And other special effects

			if (!isMoney)
			{
				if (print) Console.WriteLine("Hand now:");
				if (print) printHandDeck(printDiscard);
			}
			return cardsToDraw > 0;
		}

		//Find the card that is the strongest in hand of attribute att
		//att=="firstAction" returns the first action card found
		public int findBestCard(string att)
		{
			int best = 0;
			if (att == "cards")
			{
				for (int i = 1; i < hand.Count; i++)
					if (hand[i].cards > hand[best].cards)	//Doesn't use the best cards, i.e. if two are tied but one has lower stats it may be used anyway
						best = i;
				if (hand[best].cards == 0)
					return -1;
				return best;
			}
			if (att == "actions")
			{
				for (int i = 1; i < hand.Count; i++)
					if (hand[i].actions > hand[best].actions)
						best = i;
				if (hand[best].actions == 0)
					return -1;
				return best;
			}
			if (att == "buys")
			{
				for (int i = 1; i < hand.Count; i++)
					if (hand[i].buys > hand[best].buys)
						best = i;
				if (hand[best].buys == 0)
					return -1;
				return best;
			}
			if (att == "gold")
			{
				for (int i = 1; i < hand.Count; i++)
					if ((hand[i].gold > hand[best].gold) && (hand[i].cardType != "money"))
						best = i;
				if ((hand[best].gold == 0) || (hand[best].cardType == "money"))
					return -1;
				return best;
			}
			if (att == "firstAction")
			{
				for (int i = 0; i < hand.Count; i++)
					if (hand[i].cardType == "action")
						return i;
				return -1;
			}

			return -1;
		}

		//Buys a card given the turn and cardList, using the AI to do so
		public void buy(int turn, Card[] cardList)
		{
			while (hand.Count > 0)
			{
				if (hand[0].cardType == "money")
					playCard(0);
				else
					discardCard(0);
			}

			//Buy things
			do
			{
				if (print) Console.WriteLine("GOLD TO SPEND: {0}", stats.gold);
				bool[] validActions = new bool[uniqueCards + 1];
				for (int i = 0; i < uniqueCards; i++)
					validActions[i] = (stats.gold >= cardList[i].cost);
				validActions[uniqueCards] = true;

				int cardToBuy = qtable.pickAction(turn, stats.gold, validActions);
				if (cardToBuy == uniqueCards)
				{
					if (print) Console.WriteLine("NOT BUYING");
					break;
				}
				//Buy that card
				if (print) Console.WriteLine("BUYING CARD: {0}", cardList[cardToBuy].name);
				purchases.Add(new Purchase(cardList[cardToBuy], turn, stats.gold));
				discard.Add(cardList[cardToBuy]);
				stats.gold -= cardList[cardToBuy].cost;
				stats.buys--;
			} while ((stats.gold >= 0) && (stats.buys > 0));
		}
		
		//Tell the qTable to adjust itself using cardlist and average vp value so far
		public void adjustQTable(Card[] cardList, int avgval)
		{
			int vp = getVP();
			qtable.adjustQTable(cardList, purchases, vp, avgval);
		}

		//Count up the current victory points
		public int getVP()
		{
			int vp = 0;
			foreach (Card c in deck)
				vp += c.vp;
			foreach (Card c in hand)
				vp += c.vp;
			foreach (Card c in discard)
				vp += c.vp;
			return vp;
		}

		//Print victory points
		public void printVP()
		{
			int vp = getVP();
			Console.WriteLine("VICTORY POINTS: {0}", vp);
		}

		//Save the qTable to file path
		public void saveQTable(string path)
		{
			qtable.saveQTable(path);
		}

		//Load the qTable from path
		public void loadQTable(string path)
		{
			qtable.loadQTable(path);
		}

		/*static Comparer<Card> largerAction(Card c1, Card c2)
		{
			if (c1.actions >= c2.actions)
				return c1;
			else
				return c2;
		}*/
	}
}
