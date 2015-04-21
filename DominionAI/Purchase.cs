using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
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
