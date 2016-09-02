using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace DominionAI
{
	//Table of q-learning values from which to grab probabilities of using certain purchases in the future
	//q[turn][gold] is the state
	//q[turn][gold][card] is the probability of an action given that state
	public class QTable
	{
		double[][][] q;
		static int maxTurns = 20;
		static int maxGold = 100;
		static int uniqueCards = 17;

		public QTable()
		{
			q = new double[maxTurns][][];
			for (int i = 0; i < maxTurns; i++)	
			{
				q[i] = new double[maxGold+1][];
				for (int j = 0; j < maxGold + 1; j++)
				{
					q[i][j] = new double[uniqueCards];
					for (int k = 0; k < uniqueCards; k++)
					{
						q[i][j][k] = 1;
					}
				}
			}
		}

		//Picks an action randomly based on the possibilities of state q[turn][gold], and which actions are valid
		public int pickAction(int turn, int gold, bool[] validAction)
		{
			Random rand = new Random();
			if (gold > 100)
				gold = 100;

			double total = 0;
			double r;
			//Since the sum of all probabilities need not add up to 1, get what they do add up to and make that the total for rand
			for (int i = 0; i < uniqueCards; i++)
				if(validAction[i])
					total += q[turn][gold][i];

			r = rand.NextDouble() * total;
			double origR = r;

			//Run through the possibilities til we have the one we randomly selected
			for (int i = 0; i < uniqueCards; i++)
			{
				if (validAction[i])
				{
					if (r<=q[turn][gold][i])
						return i;
					r -= q[turn][gold][i];
				}
			}

			//Error
			Console.WriteLine("FAILED TO PROPERLY PICK AN ACTION!!!");
			return uniqueCards - 1;
		}

		//Adjust the qTable based on purchases, given performance vp, and past performance avgval
		public void adjustQTable(Card[] cardList, List<Purchase> purchases, int vp, int avgval)
		{
			double reward = 0;
			//Really old algorithm
			/*int baseVal = 10;
			if (vp >= baseVal)
				value = (vp - baseVal) * Math.Abs(vp - baseVal) * Math.Abs(vp - baseVal);
			else
				value = (vp - baseVal);//*/

			//Old algorithm
			//if (vp > avgval)
			//	reward = 2.0 * (vp - avgval) * ((double)vp / 10.0) * ((double)vp / 10.0);
			//else
			//	reward = 0.9 + ((double)vp / (double)avgval) * 0.1;

			double alpha = .2;
			//double lambda = 1;
			//double next = reward;
			//double rs = 0;
			//double minimum = .5;
			
			//Finding a good formula for reward is hard
			//Lets use roughly how much better we did than average squared, but negative if we did poorly
			reward = (vp - avgval / 2) * (vp - avgval / 2) * Math.Abs(vp - avgval) / (vp - avgval + .5);
			//reward = vp * vp;
			double discount = .2;

			//Foreach purchase, modify that action's probability in the future based on performance
			foreach (Purchase p in purchases)
			{
				int cardNumber = findCardNumber(cardList, p.purchase);
				double qvi = q[p.turn][p.goldBefore][cardNumber];
				double maxNextState = 0;
				for (int i = 0; i < uniqueCards; i++)
				{
					if (q[p.turn][p.goldBefore][i] > maxNextState)
						maxNextState = q[p.turn][p.goldBefore][i];
				}
				//Old modified algorithm
				//if (vp > avgval)
				//	qvo += alpha * (rs + lambda * next - qvi);
				//else
				//	qvo *= reward;
				//if (qvo < minimum)
				//	qvo = minimum;
				double qnext = qvi + alpha * (reward + discount * maxNextState - qvi);
				if (qnext < 0) qnext = 0;
				q[p.turn][p.goldBefore][cardNumber] = qnext;
			}
		}

		//Returns the index of find in cardList
		public int findCardNumber(Card[] cardList, Card find)
		{
			for (int i = 0; i < uniqueCards - 1; i++)
				if (cardList[i] == find)
					return i;

			//CAN'T FIND THE CARD
			return 0;
		}

		//Save the updated qTable to file path
		public void saveQTable(string path)
		{
			StreamWriter file = new StreamWriter(path);
			for (int i = 0; i < maxTurns; i++)
			{
				for (int j = 0; j <= maxGold; j++)
				{
					for (int k = 0; k < uniqueCards; k++)
					{
						file.Write("{0} ", q[i][j][k]);
					}
					file.WriteLine();
				}
				file.WriteLine();
			}
			file.Close();
		}

		//Load the qTable from file path
		public void loadQTable(string path)
		{
			try
			{
				StreamReader file = new StreamReader(path);
				string s;
				for (int i = 0; i < maxTurns; i++)
				{
					for (int j = 0; j <= maxGold; j++)
					{
						s = file.ReadLine();
						string[] parts = s.Split(' ');
						for (int k = 0; k < uniqueCards; k++)
						{
							q[i][j][k] = Convert.ToDouble(parts[k]);
						}
					}
					s = file.ReadLine();
				}
				file.Close();
			}
			catch { return;  }
		}
	}
}