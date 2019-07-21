using System;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.src.Agent;
using SabberStoneCoreAi.Meta;
using SabberStoneCore.Model;
using System.Collections.Generic;

namespace SabberStoneCoreAi
{
	
	internal class Program
	{
		private static List<Card> Player1Deck;

		public static int now_games = 0;

		public static int totalgames = 5;

		private static void Main(string[] args)
		{

			


			
			Console.WriteLine("Setup gameConfig");

			//todo: rename to Main
			GameConfig gameConfig = new GameConfig
			{
			
				StartPlayer = 1,
				Player1HeroClass = CardClass.WARRIOR, // <- put your hero class here
				Player2HeroClass = CardClass.WARRIOR,
				Player1Deck = Decks.AggroPirateWarrior, // <- put your new deck here
				Player2Deck = Decks.AggroPirateWarrior,
				FillDecks = true,
				Logging = false

				
			};

			
			Console.WriteLine("Setup POGameHandler");
			AbstractAgent player1 = new MyMCTS_Agent();
			AbstractAgent player2 = new SaliMCTS();
			var gameHandler = new POGameHandler(gameConfig, player1, player2, debug:true);

			Console.WriteLine("PlayGame");
			//gameHandler.PlayGame();
			gameHandler.PlayGames(totalgames);//30模50場66%，20模50場74%，15模50場82%，15模擬1000場70%
			GameStats gameStats = gameHandler.getGameStats();

			gameStats.printResults();

			Console.WriteLine(now_games);
			Console.WriteLine("Test successful");
			Console.ReadLine();

			
			
		}
		


	}

}
