using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	//Keep track of the stats of a purchase
	//Helps the AI determine how impactful the purchase was
	public class Purchase
	{
		public Card purchase;
		public int turn;
		public int goldBefore;

		public Purchase(Card _p, int _t, int _g)
		{
			purchase = _p;
			turn = _t;
			if (_g <= 100)
				goldBefore = _g;
			else
				goldBefore = 100;
		}
	}
}
