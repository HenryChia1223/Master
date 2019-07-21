using System;
using System.Collections.Generic;
using System.Linq;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Agent;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.Score;
using SabberStoneCoreAi;


namespace SabberStoneCoreAi.Agent
{
	/// <summary>
	/// Sali Mc Terson
	/// Chooses its actions by performing Monte Carlo Tree Search
	/// 
	/// By Lisa Piotrowski and Sarah Mittenentzwei
	/// </summary>
	class MyMCTS_Agent : AbstractAgent
	{
		private Random Rnd = new Random();
		private double remainingSeconds = 15;//假設一手最多15秒
		//private double remainingTurnSeconds = 75;
		//private double thisMoveSeconds = 10;
		private double totalTime;
		private Score.Score myScore = new SaliScore();
		//public static int won_num;
		private int actionCount = 0;//此回合動作數
		

	//	public static int  iteration_num = 20;
		//public static Score.Score myScore = new SaliScore();

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

			//thisMoveSeconds = Rnd.Next((int)remainingTurnSeconds / 10, (int)remainingTurnSeconds/5);//一手秒隨機
			//thisMoveSeconds = 10;

			Score.Score s = null;
			s = myScore;

			totalTime = 8;
			if (options.Count>=20)
			{
				totalTime = totalTime*1;
			}
			else if(options.Count<20 && options.Count>=10){

				totalTime = totalTime*0.75;
			}
			else if (options.Count<10 && options.Count>=5)
			{
				totalTime = totalTime*0.5;
			}
			else if (options.Count < 5)
			{
				totalTime = totalTime*0.25;
			}
			else
			{
				//
			}


			if (options.Count == 1)
			{
				t = options[0];
			}
			else
			{
				//t = do_MCTS.do_MCTS.GetBestAction_second(poGame, remainingSeconds / (Math.Max(options.Count - actionCount, 1)));
				//Console.WriteLine("動作有:" + options.Count + "個");
				t = do_MCTS.do_MCTS.GetBestAction_second(poGame, totalTime);
				//t = do_MCTS.do_MCTS.GetBestAction_second(poGame, thisMoveSeconds);
				//t = do_MCTS.do_MCTS.GetBestAction_iteration(poGame, 30);
			}

			double seconds = (DateTime.Now - start).TotalSeconds;//這次動作幾秒

			//totalSecond += seconds;
			
			if (t.PlayerTaskType == PlayerTaskType.END_TURN)
			{
				//Console.WriteLine( "此回合共" + totalSecond + "秒");
				//totalSecond = 0;
				remainingSeconds = 15;
				actionCount = 0;
			}
			else
			{
				remainingSeconds -= seconds;
				actionCount++;
			}
			
			/*
			if (t.PlayerTaskType == PlayerTaskType.END_TURN)
			{
				Console.WriteLine( "此回合共" + totalSecond + "秒");
				totalSecond = 0;
				remainingTurnSeconds = 75;
				
			}
			else
			{
			
				remainingTurnSeconds -= seconds;
				
			}
			*/
			
			return t;
		}

		public override void InitializeGame()
		{
			//remainingTurnSeconds = 30;
			remainingSeconds = 15;
			actionCount = 0;
		}
		
		public static void game_percentage()
		{
			//Console.WriteLine("目前進度:" + ((Program.now_games / Program.totalgames) * 100)/ iteration_num + "%");
		}

	}
}
