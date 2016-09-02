using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	public class Card
	{
		public string name;			//Cardname
		public int cost;			//Cost to buy the card
		public int actions;			//Amount of +Action
		public int cards;			//Amount of +Card
		public int buys;			//Amount of +Buy
		public int gold;			//Amount of +Gold
		public double vpEffect;		//Estimated effect on other players' victory points
		public string other;		//Extra effects of the card
		public int vp;				//Amount of +VP at end of game
		public string cardType;		//Money, victory, action, etc.

		protected Card(string _name, int _cost, int _actions, int _cards, int _buys, int _gold, double _vpEffect, string _other)
		{
			cost = _cost;
			name = _name;
			actions = _actions;
			cards = _cards;
			buys = _buys;
			gold = _gold;
			vpEffect = _vpEffect;
			other = _other;
			vp = 0;
		}
		protected Card(string _name, int _vp, int _cost)
		{
			cost = _cost;
			name = _name;
			actions = 0;
			cards = 0;
			buys = 0;
			gold = 0;
			vpEffect = 0;
			other = "";
			vp = _vp;
		}

	}

	//Different main classes of cards, these should be the only ones used outside of this file
	public class CardMoney : Card
	{
		public CardMoney(string _name, int _gold, int _cost) : base(_name, _cost, 0, 0, 0, _gold, 0, "") { cardType = "money"; }
	}
	public class CardVictory : Card
	{
		public CardVictory(string _name, int _vp, int _cost) : base(_name, _vp, _cost) { cardType = "victory"; }
	}
	public class CardActivity : Card
	{
		public CardActivity(string _name, int _cost, int _actions, int _cards, int _buys, int _gold, double _vpEffect, string _other)
			: base(_name, _cost, _actions, _cards, _buys, _gold, _vpEffect, _other) { cardType = "activity"; }
	}
}
