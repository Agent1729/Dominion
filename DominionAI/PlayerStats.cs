using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	//A supplementary class that keeps track of the players current stats
	public class PlayerStats
	{
		public int actions;
		public int buys;
		public int gold;
		public double effect;

		public PlayerStats()
		{
			actions = 1;
			buys = 1;
			gold = 0;
			effect = 0;
		}
	}
}
