using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SabberStoneCoreAi.Score;
using TreeScore.treescore;
using SabberStoneCore.Model.Zones;







namespace SabberStoneCoreAi.do_MCTS
{
	//based on https://daim.idi.ntnu.no/masteroppgaver/014/14750/masteroppgave.pdf
	 class do_MCTS
	{
		public static int num_my_board;//我方手下數量
		public static int num_op_board;//敵方手下數量
		public static int num_my_hand;//我方手牌數量
		public static int num_op_hand;//敵方手牌數量
		public static int num_my_hero;//我方英雄血量
		public static int num_op_hero;//敵方英雄血量
		public static int num_my_deck;//我方牌庫數量
		public static int num_op_deck;//敵方牌庫數量
		public static int num_remaining_mana;//我方剩餘水晶數量


		public static PlayerTask child_BestMove;
		public static int child_BestScore = Int32.MinValue;
		public static PlayerTask simu_BestMove;
		public static int simu_BestScore = Int32.MinValue;

		public static Score.Score myScore = new SaliScore();

		public static double c;//平衡常數，預設根號2
		public static int simulation_num=5 ;//模擬次數，預設5

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
					int r = node.SimulateGames(simulation_num);
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
				Console.WriteLine("visits: " + child.TotNumVisits);
				Console.WriteLine("wins: " + child.Wins);

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
				//Board_Analysis(game);//場面分析函式
				Controller my = game.CurrentPlayer;
				Controller op = game.CurrentOpponent;
				//分配數值
				num_my_board = my.BoardZone.Count;
				num_op_board = op.BoardZone.Count;
				num_my_hand = my.HandZone.Count;
				num_op_hand = op.HandZone.Count;
				num_my_hero = my.Hero.Health;
				num_op_hero = op.Hero.Health;
				num_my_deck = my.DeckZone.Count;
				num_op_deck = op.DeckZone.Count;
				num_remaining_mana = my.RemainingMana;
				
				TreeScore.treescore.tree_score.tree_node = SabberStoneCoreAi.src.Program.main.node_string;


				c = Math.Round(TreeScore.treescore.tree_score.Node_Evaluation(num_my_board, num_op_board, num_my_hand, num_op_hand,
					num_my_hero, num_op_hero, num_my_deck, num_op_deck,num_remaining_mana), 2, MidpointRounding.AwayFromZero);
				//Console.WriteLine(num_my_board+"  "+ num_op_board + "  " + num_my_hand + "  " + num_op_hand + "  " +
				//	num_my_hero + "  " + num_op_hero + "  " + num_my_deck + "  " + num_op_deck + "  " + num_remaining_mana);

				if (c < 0) 
				{
					c = 0;
				}
				//Console.WriteLine("c=" + c);
				//simulation_num = (int) Math.Round(TreeScore.treescore.tree_score.Node_Evaluation(num_my_board, num_op_board, num_my_hand, num_op_hand,
				//	num_my_hero, num_op_hero, num_my_deck, num_op_deck), 0, MidpointRounding.AwayFromZero);

				//if (simulation_num < 0) 
				//{
				//	simulation_num = 1;
				//}

				//Console.WriteLine("模擬次數為:" + simulation_num);

				try
				{
					TaskNode node = root.SelectNode();
					if (TimeUp(start, seconds)) break;

					node = node.Expand();
					if (TimeUp(start, seconds)) break;

					int r = node.SimulateGames(simulation_num);//預設5
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
				//Console.WriteLine("visits: " + child.TotNumVisits);//刪除註解
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
		
		 static void Board_Analysis(POGame.POGame game)
		{
			//絕對優勢，比對手多10滴，場面多3隻，打穩，exploitation，加深模擬
			if ((game.CurrentPlayer.Hero.Health - game.CurrentOpponent.Hero.Health > 10) && (game.CurrentPlayer.BoardZone.Count - game.CurrentOpponent.BoardZone.Count >= 3))
			{
				if (game.CurrentOpponent.HandZone.Count < 3)//對手沒手牌
				{
					c = 0.5;
					simulation_num = 20;
				}
				else if (game.CurrentOpponent.HandZone.Count < 7 && game.CurrentOpponent.HandZone.Count > 3 )
				{
					c = 0.7;
					simulation_num = 16;
				}
				else
				{
					c = 1;
					simulation_num = 12;
				}
			}

			//均勢，7滴內，使用預設，場面差小於等於2
			else if ((game.CurrentPlayer.Hero.Health - game.CurrentOpponent.Hero.Health > -7 && game.CurrentPlayer.Hero.Health - game.CurrentOpponent.Hero.Health < 7 ) &&
						Math.Abs(game.CurrentPlayer.BoardZone.Count - game.CurrentOpponent.BoardZone.Count) <=2)
			{
				c = Math.Sqrt(2);
				simulation_num = 10;
			}

			//絕對劣勢，比對手少10滴，對手比我多3隻以上，盡量探索，exploration，模擬次數壓低
			else if ((game.CurrentPlayer.Hero.Health - game.CurrentOpponent.Hero.Health < -10) && (game.CurrentOpponent.BoardZone.Count - game.CurrentPlayer.BoardZone.Count >= 3))
			{
				if (game.CurrentOpponent.HandZone.Count < 3)//對手沒手牌
				{
					c = 1.7;
					simulation_num = 8;
				}
				else if (game.CurrentOpponent.HandZone.Count < 7 && game.CurrentOpponent.HandZone.Count > 3)
				{
					c = 2;
					simulation_num = 6;
				}
				else
				{
					c =	2.4;
					simulation_num = 3;
				}
			}

		}


		private static bool TimeUp(DateTime start, double seconds)
		{
			return (DateTime.Now - start).TotalSeconds > seconds;
		}

		private class TaskNode
		{
			static Random rand = new Random();
			//static double biasParameter = Math.Sqrt(2);  //ucb平衡常數0.5

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

				explorationScore *= c;

				return exploitScore + explorationScore;
			}

			//public TaskNode Expand()
			//{
			//	//Console.WriteLine("---------Expandtion---------");
			//	if (PossibleActions.Count == 0)
			//	{
			//		// the selected node cannot be expanded further
			//		// this is a leaf, as it has no children that would had been selected
			//		// --> this node markes the end of the game
			//		return null;
			//	}


			//	List<PlayerTask> bestscore = new List<PlayerTask>();//紀錄最高分動作
			//	int change_num = 0;//有幾個最高分
			//	//PlayerTask action = PossibleActions[rand.Next(PossibleActions.Count)];


			//	Score.Score s = null;
				
			//	s = myScore;

			//	if (PossibleActions.Count >= 2)
			//	{


			//		Dictionary<PlayerTask, int> score = new Dictionary<PlayerTask, int>();

			//		foreach (PlayerTask i in PossibleActions)
			//		{
			//			//int r = Agent.MyMCTS_Agent.do_Score(Game,i);
			//			//	Console.WriteLine("-------------------------");//刪除註解
			//			Dictionary<PlayerTask, POGame.POGame> d = Game.Simulate(new List<PlayerTask> { i });
			//			s.Controller = d[i].CurrentPlayer;

	
			//			score.Add(i, s.Rate());//(動作,得分)

			//			//result = my_Score(i);
			//			//Console.WriteLine("以上建立child得" + s.Rate() + "分");////刪除註解
			//			//Console.WriteLine("-------------------------");
			//			//Console.WriteLine();

			//			if (s.Rate() > child_BestScore)
			//			{
			//				child_BestMove = i;
			//				child_BestScore = s.Rate();
			//			}
			//		}

			//		foreach (KeyValuePair<PlayerTask, int> a in score)
			//		{
			//			if (a.Value == child_BestScore)
			//			{
			//				change_num++;//最高分數量加1
			//				bestscore.Add(a.Key);//加入最高分list中
			//			}
			//		}


			//		if (change_num > 1)//有超過一個動作最高分
			//		{
			//			//從最高分裡面隨機選一個動作
			//			int randomNumber = rand.Next(bestscore.Count);
			//			child_BestMove = bestscore[randomNumber];
			//			//PlayerTask action = options[randomNumber];//隨機模擬
			//		}
					
			//	}
			//	else{

			//		child_BestMove = PossibleActions[0];
			//	}

			//	PlayerTask action = child_BestMove;//存入action

			//	child_BestMove = null;//初始化
			//	child_BestScore = Int32.MinValue;//初始化
			//	bestscore.Clear();//初始化
				
			//	// there are some actions left to do, so we can add a new child
			//	try
			//	{
			//		return AddChild(action);
			//	}
			//	catch (Exception e)
			//	{
			//		//Debug.WriteLine("Exception during adding child to MCTS Tree");
			//		//Debug.WriteLine(action.FullPrint());
			//		//Debug.WriteLine(e.Message);
			//		return null;
			//	}
			//}


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
				//Console.WriteLine("---------addchild---------");//刪除註解
				// simulate the action so we can expand the tree

				Dictionary<PlayerTask, POGame.POGame> dic = Game.Simulate(new List<PlayerTask> { action });
				POGame.POGame childGame = dic[action];//更新遊戲狀態
				//Console.WriteLine("--------------------------");//刪除註解
				//Console.WriteLine();
				TaskNode child = new TaskNode(this, action, childGame);
				this.Children.Add(child);

				return child;
			}



			public int SimulateGames(int numGames)
			{
				
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

			//private int Simulate()
			//{
			//	POGame.POGame gameClone = Game.getCopy();
			//	int initialPlayer = gameClone.CurrentPlayer.PlayerId;

			//	int num = 0;


			//	while (true)
			//	{
			//		if (num > 100) return 1;//time out
			//		if (gameClone.CurrentPlayer.Opponent.Hero.Health < 1) return 1;
			//		else if (gameClone.CurrentPlayer.Hero.Health < 1) return 0;

			//		if (gameClone.State == SabberStoneCore.Enums.State.COMPLETE)
			//		{
			//			Controller currPlayer = gameClone.CurrentPlayer;
			//			if (currPlayer.PlayState == SabberStoneCore.Enums.PlayState.WON
			//				&& currPlayer.PlayerId == initialPlayer)
			//			{
			//				return 1;
			//			}
			//			if (currPlayer.PlayState == SabberStoneCore.Enums.PlayState.LOST
			//				&& currPlayer.PlayerId == initialPlayer)
			//			{
			//				return 0;
			//			}
			//		}

			//		List<PlayerTask> options = gameClone.CurrentPlayer.Options();


			//		Score.Score s = null;
			//		s = myScore;



			//		List<int> change = new List<int>();
			//		List<PlayerTask> bestscore = new List<PlayerTask>();//紀錄最高分動作
			//		Dictionary<PlayerTask, int> change = new Dictionary<PlayerTask, int>();//紀錄每個動作分數
			//		int change_num = 0;//有幾個最高分

			//		if (options.Count >= 2)
			//		{
			//			/*
			//			for (int i = 0; i < options.Count; i++)//逐一檢查每個動作取最高分
			//			{
			//				result = my_Score(options[i]);
			//				change.Add(result);//紀錄每個動作分數
							
			//				change.Add(options[i],result);//紀錄每個動作分數
			//				Console.WriteLine("模擬得" + result + "分");
			//				if ( result >= simu_BestScore)
			//				{
			//					simu_BestMove = options[i];
			//					simu_BestScore = result;
			//					change_num++;//最高分數量加1
			//					bestscore.Add(options[i]);//加入最高分list中
			//				}
			//			}
			//			*/
			//			Dictionary<PlayerTask, int> score = new Dictionary<PlayerTask, int>();

			//			foreach (PlayerTask i in options)
			//			{
			//				int r = Agent.MyMCTS_Agent.do_Score(gameClone, i);

			//				Agent.MyMCTS_Agent.game_percentage();


			//				Console.WriteLine("-------------------------");//刪除註解
			//				Dictionary<PlayerTask, POGame.POGame> d = gameClone.Simulate(new List<PlayerTask> { i });
			//				s.Controller = d[i].CurrentPlayer;


			//				score.Add(i, s.Rate());//(動作,得分)

			//				result = my_Score(i);
			//				Console.WriteLine("以上模擬得" + s.Rate() + "分");//刪除註解
			//				Console.WriteLine("-------------------------");
			//				Console.WriteLine();

			//				if (s.Rate() > simu_BestScore)
			//				{
			//					simu_BestMove = i;
			//					simu_BestScore = s.Rate();
			//				}
			//			}


			//			foreach (KeyValuePair<PlayerTask, int> a in score)
			//			{
			//				if (a.Value == simu_BestScore)
			//				{
			//					change_num++;//最高分數量加1
			//					bestscore.Add(a.Key);//加入最高分list中
			//				}
			//			}

			//			if (change_num > 1)//有超過一個動作最高分//
			//			{
			//				從最高分裡面隨機選一個動作//
			//				int randomNumber = rand.Next(bestscore.Count);//
			//				simu_BestMove = bestscore[randomNumber];//
			//				PlayerTask action = options[randomNumber];//隨機模擬//
			//			}

			//		}
			//		else
			//		{
			//			PlayerTask action = options[0];//存入action
			//			simu_BestMove = options[0];
			//		}
			//		PlayerTask action = simu_BestMove;//存入action

			//		simu_BestMove = null;//初始化
			//		simu_BestScore = Int32.MinValue;//初始化
			//		bestscore.Clear();//初始化


			//		try
			//		{
			//			Process fails as soon as opponent plays a card, so use simulate here
			//			Console.WriteLine("---------Simulation---------");//刪除註解
			//			Dictionary<PlayerTask, POGame.POGame> dic = gameClone.Simulate(new List<PlayerTask> { action });
			//			gameClone = dic[action];//更新遊戲狀態
			//			Console.WriteLine("--------------------------");//刪除註解
			//			Console.WriteLine();
			//			if (gameClone == null)
			//			{
			//				Debug.WriteLine(action.FullPrint());
			//			}
			//		}
			//		catch (Exception e)
			//		{
			//			Debug.WriteLine("Exception during single game simulation");
			//			Debug.WriteLine(e.StackTrace);
			//		}


			//		num++;
			//	}
			//}
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
