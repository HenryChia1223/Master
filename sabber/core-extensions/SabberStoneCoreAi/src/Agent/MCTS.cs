using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SabberStoneCoreAi.MCTS
{
	//based on https://daim.idi.ntnu.no/masteroppgaver/014/14750/masteroppgave.pdf
	class MCTS
	{
		/*
		public static PlayerTask GetBestAction_iteration(POGame.POGame game, int iterations)
		{
			TaskNode root = new TaskNode(null, null, game.getCopy());

			for (int i = 0; i < iterations; ++i)
			{
				try
				{
					TaskNode node = root.SelectNode();
					node = node.Expand();
					int r = node.SimulateGames(10);
					node.Backpropagate(r);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
					Debug.WriteLine(e.StackTrace);
				}
			}

			TaskNode best = null;

			foreach (TaskNode child in root.Children)
			{
				//Console.WriteLine("visits: " + child.TotNumVisits);
				//Console.WriteLine("wins: " + child.Wins);

				if (best == null || child.TotNumVisits > best.TotNumVisits)
				{
					best = child;
				}
			}

			//Console.WriteLine("best visits: " + best.TotNumVisits);
			//Console.WriteLine("best wins: " + best.Wins);

			return best.Action;
		}
		*/
		
		public static PlayerTask GetBestAction_second(POGame.POGame game, double seconds)
		{
			DateTime start = DateTime.Now;
			TaskNode root = new TaskNode(null, null, game.getCopy());

			int i = 0;

			while (true)
			{
				if (TimeUp(start, seconds - 0.1)) break;

				try
				{
					TaskNode node = root.SelectNode();
					if (TimeUp(start, seconds)) break;

					node = node.Expand();
					if (TimeUp(start, seconds)) break;

					int r = node.SimulateGames(5);//預設5
					if (TimeUp(start, seconds)) break;

					node.Backpropagate(r);
				}
				catch (Exception e)
				{
					//Debug.WriteLine(e.Message);
					//Debug.WriteLine(e.StackTrace);
				}

				++i;
			}

			TaskNode best = null;

			//Console.WriteLine($"Iterations: {i}, Time: " + (DateTime.Now-start).TotalMilliseconds + "ms");

			foreach (TaskNode child in root.Children)
			{
				//Console.WriteLine("visits: " + child.TotNumVisits);
				//Console.WriteLine("wins: " + child.Wins);

				if (best == null || child.TotNumVisits > best.TotNumVisits || (child.TotNumVisits == best.TotNumVisits && child.Wins > best.Wins))
				{
					best = child;
				}
			}

			//Console.WriteLine("best visits: " + best.TotNumVisits);
			//Console.WriteLine("best wins: " + best.Wins);

			if (best == null)
			{
				//Debug.WriteLine("best == null");
				return game.CurrentPlayer.Options()[0];
			}

			//Console.WriteLine("best wins: " + best.Wins + " best visits: " + best.TotNumVisits);
			return best.Action;
		}
		

		

		private static bool TimeUp(DateTime start, double seconds)
		{
			return (DateTime.Now - start).TotalSeconds > seconds;
		}

		private class TaskNode
		{
			static Random rand = new Random();
			static double biasParameter = Math.Sqrt(0.5);  //ucb平衡常數0.5

			POGame.POGame Game = null;
			TaskNode Parent = null;
			List<PlayerTask> PossibleActions = null;

			public PlayerTask Action { get; private set; } = null;
			public List<TaskNode> Children { get; private set; } = null;

			public int TotNumVisits { get; private set; } = 0; //辦訪次數
			public int Wins { get; private set; } = 0; //勝利次數

			public TaskNode(TaskNode parent, PlayerTask action, POGame.POGame game) //TaskNode建構子
			{
				Game = game;
				Parent = parent;
				Action = action;
				PossibleActions = Game.CurrentPlayer.Options();
				Children = new List<TaskNode>();


			}

			public TaskNode SelectNode()
			{
				//Console.WriteLine("---------Selection--------");

				if (PossibleActions.Count == 0 && Children.Count > 0)//有子節點的狀況，且第一層都建完
				{
					double candidateScore = Double.MinValue;
					TaskNode candidate = null;

					foreach (TaskNode child in Children)
					{
						double childScore = child.UCB1Score();
						if (childScore > candidateScore)
						{
							candidateScore = childScore;
							candidate = child;
						}
					}

					return candidate.SelectNode();
				}

				return this; //沒有子節點
			}

			private double UCB1Score()
			{
				double exploitScore = (double)Wins / (double)TotNumVisits;
				double explorationScore = Math.Sqrt(Math.Log(Parent.TotNumVisits) / TotNumVisits);

				explorationScore *= biasParameter;

				return exploitScore + explorationScore;
			}

			public TaskNode Expand()
			{
				//Console.WriteLine("---------Expandtion---------");
				if (PossibleActions.Count == 0)
				{
					// the selected node cannot be expanded further
					// this is a leaf, as it has no children that would had been selected
					// --> this node markes the end of the game
					return null;
				}

				PlayerTask action = PossibleActions[rand.Next(PossibleActions.Count)];


				// there are some actions left to do, so we can add a new child
				try
				{
					return AddChild(action);
				}
				catch (Exception e)
				{
					//Debug.WriteLine("Exception during adding child to MCTS Tree");
					//Debug.WriteLine(action.FullPrint());
					//Debug.WriteLine(e.Message);
					return null;
				}
			}

			private TaskNode AddChild(PlayerTask action)
			{
				PossibleActions.Remove(action);
				//Console.WriteLine("---------addchild---------");
				// simulate the action so we can expand the tree

				Dictionary<PlayerTask, POGame.POGame> dic = Game.Simulate(new List<PlayerTask> { action });
				POGame.POGame childGame = dic[action];//更新遊戲狀態

				TaskNode child = new TaskNode(this, action, childGame);
				this.Children.Add(child);

				return child;
			}



			public int SimulateGames(int numGames)
			{
				//Console.WriteLine("---------Simulation---------");
				int wins = 0;
				for (int i = 0; i < numGames; ++i)
				{
					try
					{
						wins += Simulate();
					}
					catch (Exception e)
					{
						//Debug.WriteLine("Exception during Simulation");
						//Debug.WriteLine(e.Message);
					}
				}
				return wins;
			}

			private int Simulate()
			{
				POGame.POGame gameClone = Game.getCopy();
				int initialPlayer = gameClone.CurrentPlayer.PlayerId;

				while (true)
				{
					if (gameClone.State == SabberStoneCore.Enums.State.COMPLETE)
					{
						Controller currPlayer = gameClone.CurrentPlayer;
						if (currPlayer.PlayState == SabberStoneCore.Enums.PlayState.WON
							&& currPlayer.PlayerId == initialPlayer)
						{
							return 1;
						}
						if (currPlayer.PlayState == SabberStoneCore.Enums.PlayState.LOST
							&& currPlayer.PlayerId == initialPlayer)
						{
							return 0;
						}
					}

					List<PlayerTask> options = gameClone.CurrentPlayer.Options();



					int randomNumber = rand.Next(options.Count);
					PlayerTask action = options[randomNumber];
					try
					{
						// Process fails as soon as opponent plays a card, so use simulate here
						Dictionary<PlayerTask, POGame.POGame> dic = gameClone.Simulate(new List<PlayerTask> { action });
						gameClone = dic[action];//更新遊戲狀態
						if (gameClone == null)
						{
							Debug.WriteLine(action.FullPrint());
						}
					}
					catch (Exception e)
					{
						//Debug.WriteLine("Exception during single game simulation");
						//Debug.WriteLine(e.StackTrace);
					}
				}
			}

			public void Backpropagate(int score)
			{
				//Console.WriteLine("---------Back---------");
				int currentPlayerID = Game.CurrentPlayer.PlayerId;
				TaskNode node = this;

				// While the node has a parent, backpropagate the result of the simulation up the game tree
				while (node.Parent != null)
				{
					if (node.Parent.Game.CurrentPlayer.PlayerId == currentPlayerID)
					{
						node.UpdateScore(score);
					}
					else
					{
						if (score == 0)
						{
							node.UpdateScore(1);
						}
						else
						{
							node.UpdateScore(0);
						}
					}
					node = node.Parent;
				}
				node.TotNumVisits++;
			}

			private void UpdateScore(int score)
			{
				TotNumVisits++;
				Wins += score;
			}
		}
	}
}
