using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Score;
using SabberStoneCoreAi.Nodes;
using SabberStoneCore.Model;
using SabberStoneCore.Kettle;
using System.Linq;
using SabberStoneCore.Model.Zones;
using SabberStoneCore.Model.Entities;

namespace SabberStoneCoreAi.src.Agent
{



	class MyAgent : AbstractAgent
	{

		public static int result = 0;

		private Random Rnd = new Random();

		

		public override void FinalizeAgent()
		{
		
		}

		public override void FinalizeGame()
		{
			
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			/*
			List<PlayerTask> turnList = new List<PlayerTask>();
			PlayerTask option;
			do
			{
				List<PlayerTask> options = poGame.CurrentPlayer.Options();
				option = options[Rnd.Next(options.Count)];
				poGame.Process(option);
				turnList.Add(option);
			} while (option.PlayerTaskType != PlayerTaskType.END_TURN);

			return turnList;
			*/

			List<PlayerTask> sim = new List<PlayerTask>();

			PlayerTask option;
			PlayerTask BestMove;
			int BestScore=-10000000;
			int num= 0; //第幾種
			List<PlayerTask> options = poGame.CurrentPlayer.Options();


			//int r = Rnd.Next(options.Count);
			//option = options[r];
			//sim.Add(option);

			BestMove = options[0];//initialize

			/*
			foreach (PlayerTask option1 in options)
			{
				Console.WriteLine(option1);
			}
			*/

			for (int i=0 ; i<options.Count ; i++)
			{

				sim.Add(options[i]);
				
				Console.WriteLine("-----------^-----------");
				Console.WriteLine("-----------" + options[i] + "----------");	//列出該回合所有可能的行動

				poGame.Simulate(sim);
				Socre_control(options[i]);
				Console.WriteLine("得" + result + "分");
				Console.WriteLine("-----------ˇ------------");
				Console.WriteLine();

				

				if (options[i].PlayerTaskType != PlayerTaskType.END_TURN && result > BestScore)
				{
					BestMove = options[i];
					BestScore = result;
					num = i;
				}

				result = 0; //初始化result

				sim.Clear();
			}



			Console.WriteLine("共" + options.Count + "種選擇");
			
			option = BestMove;
			Console.WriteLine("選第"+ num + "個");

			BestScore = -10000000; //初始化BestScore
			BestMove = options[0]; //初始化BestMove

			return option;
			
			

		}

		public override void InitializeAgent()
		{
			Rnd = new Random();
		}

		public override void InitializeGame()
		{
			Console.WriteLine("換我了2");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}


		//fitness function
		/*
		public int MyScore(PlayerTask _option)
		{
			
		
			if (_option.Controller.Opponent.Hero.Health < 1)
				return Int32.MaxValue;

			if (_option.Controller.Hero.Health < 1)
				return Int32.MinValue;

			

			if (_option.Controller.Opponent.BoardZone.Count == 0 && _option.Controller.BoardZone.Count > 0)
				result += 1000;

			if (_option.Controller.Opponent.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health) > 0)
				result += _option.Controller.Opponent.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health) * -1000;

			result += _option.Controller.BoardZone.Sum(p => p.AttackDamage);

			result += (_option.Controller.Hero.Health - _option.Controller.Opponent.Hero.Health) * 1000;

			

			return result;
		}
		*/

		public int Socre_control(PlayerTask _option)
		{
			if (_option.Controller.Opponent.Hero.Health < 1)
				return Int32.MaxValue;

			if (_option.Controller.Hero.Health < 1)
				return Int32.MinValue;

			float resultHero = 0;
			float resultOpponent = 0;

			resultHero += 2 * (float)Math.Sqrt(_option.Controller.Hero.Health);
			resultHero += _option.Controller.HandZone.Count > 3 ? 9 + 2 * (_option.Controller.HandZone.Count - 3) : 3 * _option.Controller.HandZone.Count;
			resultHero += (float)Math.Sqrt(_option.Controller.DeckZone.Count); // - next draw damage
			resultHero += _option.Controller.BoardZone.Sum(p => p.AttackDamage) + _option.Controller.BoardZone.Sum(p => p.AttackDamage);
			resultHero += _option.Controller.Opponent.BoardZone.Count < 1 ? 2 + Math.Min(_option.Controller.NumTurnsInPlay, 10) : 0;

			resultOpponent += 2 * (float)Math.Sqrt(_option.Controller.Opponent.Hero.Health);
			resultOpponent += _option.Controller.Opponent.HandZone.Count > 3 ? 9 + 2 * (_option.Controller.Opponent.HandZone.Count - 3) : 3 * _option.Controller.Opponent.HandZone.Count;
			resultOpponent += (float)Math.Sqrt(_option.Controller.Opponent.DeckZone.Count); // - next draw damage
			resultOpponent += _option.Controller.Opponent.BoardZone.Sum(p => p.AttackDamage) + _option.Controller.Opponent.BoardZone.Sum(p => p.AttackDamage);
			resultOpponent += _option.Controller.BoardZone.Count < 1 ? 2 + Math.Min(_option.Controller.NumTurnsInPlay, 10) : 0;

			return (int)(resultHero - resultOpponent);
		}
	}
}
