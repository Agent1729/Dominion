﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace DominionAI
{
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

		public int pickAction(int turn, int gold, bool[] validAction)
		{
			Random rand = new Random();
			if (gold > 100)
				gold = 100;

			double total = 0;
			double r;
			for (int i = 0; i < uniqueCards; i++)
				if(validAction[i])
					total += q[turn][gold][i];

			r = rand.NextDouble() * total;
			double origR = r;

			for (int i = 0; i < uniqueCards; i++)
			{
				if (validAction[i])
				{
					if (r<=q[turn][gold][i])
						return i;
					r -= q[turn][gold][i];
				}
			}

			Console.WriteLine("FAILED TO PROPERLY PICK AN ACTION!!!");
			return uniqueCards - 1;
		}

		public void adjustQTable(Card[] cardList, List<Purchase> purchases, int vp, int avgval)
		{
			double value = 0;
			/*int baseVal = 10;
			if (vp >= baseVal)
				value = (vp - baseVal) * Math.Abs(vp - baseVal) * Math.Abs(vp - baseVal);
			else
				value = (vp - baseVal);//*/

			if (vp > avgval)
				value = 2.0 * (vp - avgval) * ((double)vp / 10.0) * ((double)vp / 10.0);
			else
				value = 0.9 + ((double)vp / (double)avgval) * 0.1;

			double alpha = .8;
			double lambda = 1;
			double next = value;
			double rs = 0;
			double minimum = .5;

			foreach (Purchase p in purchases)
			{
				int cardNumber = findCardNumber(cardList, p.purchase);
				double qvi = q[p.turn][p.goldBefore][cardNumber];
				double qvo = qvi;
				if (vp > avgval)
					qvo += alpha * (rs + lambda * next - qvi);
				else
					qvo *= value;
				if (qvo < minimum)
					qvo = minimum;
				q[p.turn][p.goldBefore][cardNumber] = qvo;
			}
		}

		public int findCardNumber(Card[] cardList, Card find)
		{
			for (int i = 0; i < uniqueCards - 1; i++)
				if (cardList[i] == find)
					return i;

			//CAN'T FIND THE CARD
			return 0;
		}

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