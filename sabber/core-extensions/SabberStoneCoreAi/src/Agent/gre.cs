using System;
using System.Collections.Generic;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Score;

namespace SabberStoneCoreAi.Agent
{
	/// <summary> 
	/// Special Agent Li
	/// Chooses each action greedyly rated by a given score function
	/// 
	/// By Sarah Mittenentzwei and Lisa Piotrowski
	/// </summary>
	class gre : AbstractAgent
	{
		private Score.Score myScore = new SaliScore();
		private Score.Score contrScore = new ControlScore();
		private Score.Score midrScore = new MidRangeScore();

		public override void FinalizeAgent()
		{
		}

		public override void FinalizeGame()
		{
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			Score.Score s = null;
			switch (poGame.CurrentPlayer.HeroClass)
			{
				case SabberStoneCore.Enums.CardClass.SHAMAN:
					s = midrScore;
					break;
				case SabberStoneCore.Enums.CardClass.WARRIOR:
					s = myScore;
					break;
				case SabberStoneCore.Enums.CardClass.MAGE:
					s = contrScore;
					break;
				default:
					s = myScore;
					break;
			}

			List<PlayerTask> options = poGame.CurrentPlayer.Options();
			float maxScore = Single.MinValue;
			PlayerTask best = options[0];

			foreach (PlayerTask task in options)
			{

				//for each task, that is not giving up or ending the turn
				if (task.PlayerTaskType != PlayerTaskType.CONCEDE && task.PlayerTaskType != PlayerTaskType.END_TURN)
				{
					//simulate the choice of that task numSim times
					try
					{
						Dictionary<PlayerTask, POGame.POGame> dic = poGame.Simulate(new List<PlayerTask> { task });
						//set controller for the Scores
						s.Controller = dic[task].CurrentPlayer;
						//get the score with the rate function
						
						if (s.Rate() > maxScore)
						{
							maxScore = s.Rate();
							best = task;
						}
					}
					catch (Exception e)
					{
						continue;
					}
				}
			}
			
				//Console.WriteLine(best.FullPrint());
				if (best == null)
			{
				best = options[0];
			}
			return best;
		}


		public override void InitializeAgent()
		{
		}

		public override void InitializeGame()
		{
		}
	}
}
