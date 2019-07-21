using System;
using System.Collections.Generic;
using System.Linq;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Agent;
using SabberStoneCore.Enums;

namespace SabberStoneCoreAi.Agent
{
	/// <summary>
	/// Sali Mc Terson
	/// Chooses its actions by performing Monte Carlo Tree Search
	/// 
	/// By Lisa Piotrowski and Sarah Mittenentzwei
	/// </summary>
	class SaliMCTS : AbstractAgent
	{
		private double remainingSeconds = 15;
		private int actionCount = 0;

		public override void InitializeAgent()
		{
		}

		public override void FinalizeAgent()
		{
			//Nothing to do here
		}

		public override void FinalizeGame()
		{
			//Nothing to do here
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			DateTime start = DateTime.Now;

			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			PlayerTask t = options[0];








			/*
			Console.WriteLine("---------SaliMCTS---------");
			
			foreach (PlayerTask option1 in options)
			{
				
				Console.WriteLine(option1);
				
			}
			
			Console.WriteLine("--------------------------");
			Console.WriteLine();
			*/







			if (options.Count == 1)
			{
				t = options[0];
			}
			/*else if (options.Count == 2)
			{
				t = options[1];
			}*/
			else
			{
				t = MCTS.MCTS.GetBestAction_second(poGame, remainingSeconds / (Math.Max(options.Count - actionCount, 1)));
				//t = MCTS.MCTS.GetBestAction_second(poGame,75);
			}

			double seconds = (DateTime.Now - start).TotalSeconds;

			if (t.PlayerTaskType == PlayerTaskType.END_TURN)
			{
				remainingSeconds = 15;
				actionCount = 0;
			}
			else
			{
				remainingSeconds -= seconds;
				actionCount++;
			}
			return t;
		}



		public override void InitializeGame()
		{
			remainingSeconds = 15;
			actionCount = 0;
		}
	}
}
