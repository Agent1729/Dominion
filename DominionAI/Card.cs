using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
	public class Card
	{
		public string name;
		public int cost;
		public int actions;
		public int cards;
		public int buys;
		public int gold;
		public double effect;
		public string other;
		public int vp;
		public string cardType;

		public Card(string _name, int _cost, int _actions, int _cards, int _buys, int _gold, double _effect, string _other)
		{
			cost = _cost;
			name = _name;
			actions = _actions;
			cards = _cards;
			buys = _buys;
			gold = _gold;
			effect = _effect;
			other = _other;
			vp = 0;
		}
		public Card(string _name, int _vp, int _cost)
		{
			cost = _cost;
			name = _name;
			actions = 0;
			cards = 0;
			buys = 0;
			gold = 0;
			effect = 0;
			other = "";
			vp = _vp;
		}

	}

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
		public CardActivity(string _name, int _cost, int _actions, int _cards, int _buys, int _gold, double _effect, string _other)
			: base(_name, _cost, _actions, _cards, _buys, _gold, _effect, _other) { cardType = "activity"; }
	}
}
