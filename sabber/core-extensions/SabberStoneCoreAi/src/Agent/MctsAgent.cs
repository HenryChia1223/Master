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
	class MctsAgent : AbstractAgent
	{

		public static int result = 0;
		private Random rnd = new Random();

		public override void InitializeAgent()
		{
			rnd = new Random();
		}

		public override void InitializeGame()
		{
			Console.WriteLine("換我了1");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{


			PlayerTask option;
			List<PlayerTask> options = poGame.CurrentPlayer.Options(); //當下所有可執行動作的List



			List<PlayerTask> sim = new List<PlayerTask>(); //建立要模擬動作的List
			int num1 = rnd.Next(options.Count);




			sim.Add(options[num1]);


			Console.WriteLine("-----------^-----------");
			Console.WriteLine("-----------" + options[num1] + "----------");
			poGame.Simulate(sim);
			//Socre_control(options[0]);
			//Console.WriteLine("得" + result + "分");
			Console.WriteLine("-----------ˇ------------");
			Console.WriteLine();

			Console.WriteLine("共" + options.Count + "種選擇");
			Console.WriteLine("選第" + num1 + "個");

			result = 0; //初始化result
			sim.Clear();





			/*

			POGame.POGame gameClone = poGame.getCopy();

			List<PlayerTask> options2 = gameClone.CurrentPlayer.Options();
			
			int num_2 = rnd.Next(options2.Count);
			PlayerTask action = options[num_2];
			try
			{
				// Process fails as soon as opponent plays a card, so use simulate here
				Dictionary<PlayerTask, POGame.POGame> dic = gameClone.Simulate(new List<PlayerTask> { action });
				gameClone = dic[action];
			
			}
			catch (Exception e)
			{
				//Debug.WriteLine("Exception during single game simulation");
				//Debug.WriteLine(e.StackTrace);
			}


			PlayerTask option_2;
		

			
			sim.Add(options2[num_2]);
			sim.Add(options2[1]);


			Console.WriteLine("-----------^-----------");
			Console.WriteLine("-----------" + options2[num_2] + "----------");
			gameClone.Simulate(sim);
			//Socre_control(options_2[0]);
			//Console.WriteLine("得" + result + "分");

			
			Console.WriteLine("-----------ˇ------------");
			Console.WriteLine();

			Console.WriteLine("共" + options_2.Count + "種選擇");
			Console.WriteLine("選第" + num_2 + "個");

			result = 0; //初始化result
			sim.Clear();

	        */
			option = options[num1];

			return option;
		}

		public override void FinalizeGame()
		{

		}

		public override void FinalizeAgent()
		{

		}

		public int Socre_control(PlayerTask _option)
		{
			if (_option.Controller.Opponent.Hero.Health < 1)
			{
				result = Int32.MaxValue;
				return Int32.MaxValue;
			}

			if (_option.Controller.Hero.Health < 1)
			{
				result = Int32.MinValue;
				return Int32.MinValue;
			}

			if (_option.Controller.Opponent.BoardZone.Count == 0 && _option.Controller.BoardZone.Count > 0)
				result += 1000;

			result += (_option.Controller.BoardZone.Count - _option.Controller.Opponent.BoardZone.Count) * 50;

			result += (_option.Controller.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health) - _option.Controller.Opponent.BoardZone.Where(p => p.HasTaunt).Sum(p => p.Health)) * 25;

			result += _option.Controller.BoardZone.Sum(p => p.AttackDamage);

			result += (_option.Controller.Hero.Health - _option.Controller.Opponent.Hero.Health) * 10;



			return result;
		}


	
	}
}
