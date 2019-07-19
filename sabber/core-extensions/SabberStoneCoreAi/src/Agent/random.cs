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
using SabberStoneCore.Enums;


namespace SabberStoneCoreAi.src.Agent
{


	class random : AbstractAgent
	{
		private Random Rnd = new Random();
		public static float result = 0;

		private Score.Score myScore = new MidRangeScore();
		public static int  won_num;

		public override void FinalizeAgent()
		{

		}

		public override void FinalizeGame()
		{

		}
		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			//Rnd = new Random();
			PlayerTask option;
			List<PlayerTask> options = poGame.CurrentPlayer.Options();


		
			/*
			POGame.POGame gameClone = poGame.getCopy();//
			List<PlayerTask> new_options = gameClone.CurrentPlayer.Options();//
			int num = new_options.Count-1; //第幾種												 //sim.Add();
			PlayerTask action = new_options[num];//


			Console.WriteLine("----------一開始有------------");
			foreach (PlayerTask a in new_options)
			{
				Console.WriteLine(a);
			}
			Console.WriteLine("----------一開始有------------");
			Console.WriteLine("共" + new_options.Count + "種選擇");
			Console.WriteLine();


			Console.WriteLine("-----------執行第" + num + "項後-----------");
			Dictionary<PlayerTask, POGame.POGame> dic = gameClone.Simulate(new List<PlayerTask> { action });
			Console.WriteLine("-----------執行第" + num + "項後-----------");
			Console.WriteLine();

			gameClone = dic[action];//更新遊戲狀態

			List<PlayerTask> newnew_options = gameClone.CurrentPlayer.Options();


			Console.WriteLine("----------最後有------------");
			foreach (PlayerTask a in newnew_options)
			{
				Console.WriteLine(a);
			}
			Console.WriteLine("----------最後有------------");
			Console.WriteLine("共" + newnew_options.Count + "種選擇");
			Console.WriteLine();


			option = options[0];
			*/
			//Program.game_percentage();

			Score.Score s = null;
			s = myScore;
			int r = Rnd.Next(options.Count);
			option = options[r];


			//	List<PlayerTask> sim = new List<PlayerTask>();
			//	sim.Add(option);
			//poGame.Simulate(sim);
			//result = my_Score(option);
			
			Dictionary<PlayerTask, POGame.POGame> dic = poGame.Simulate(new List<PlayerTask> { option });
			s.Controller = dic[option].CurrentPlayer;

			int initialPlayer = poGame.CurrentPlayer.PlayerId;
			if (s.Controller.Opponent.Hero.Health < 1)
			{
				won_num++;
			}
			Console.WriteLine("目前贏" + won_num + "場");
			

			//	Console.WriteLine(s.Rate());
			/*
			for (int i = 0; i < options.Count; i++)
			{

				

				Console.WriteLine("-----------^-----------");
				Console.WriteLine("-----------" + options[i] + "----------");   //列出該回合所有可能的行動

			//	result = my_Score(options[i]);
				//is_change.Add(result);
				Console.WriteLine("得" + s.Rate() + "分");
				Console.WriteLine("-----------ˇ------------");
				Console.WriteLine();
				
			}*/
			/*
			foreach(int change in is_change)
			{
				Console.WriteLine(change);
			}
			*/
			foreach (PlayerTask option1 in options)
			{
				Console.WriteLine(option1);
			}
			Console.WriteLine("共" + options.Count + "種選擇");

			Console.WriteLine("選第" + r + "個");
			return option;
		}
		public override void InitializeAgent()
		{

		}

		public override void InitializeGame()
		{

		}



		/*
		public int my_Score(PlayerTask _option)
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
			
		}*/
		/*
		public static float my_Score(PlayerTask _option)
		{
			float a = 0;

			if (_option.Controller.Opponent.Hero.Health < 1)
				return Int32.MaxValue;

			if (_option.Controller.Hero.Health < 1)
				return Int32.MinValue;



			if (_option.Controller.Opponent.BoardZone.Count == 0 && _option.Controller.BoardZone.Count > 0)
				a += 1000;

			if (_option.Controller.Opponent.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health) > 0)
				a += _option.Controller.Opponent.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health) * -1000;

			a += _option.Controller.BoardZone.Sum(p => p.AttackDamage);

			a += (_option.Controller.Hero.Health - _option.Controller.Opponent.Hero.Health) * 1000;



			return a;
		}
		*/
	}
	
}
