﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SabberStoneCore.Enums;
using SabberStoneCore.Config;
using SabberStoneCore.Model;
using SabberStoneCore.Tasks;

namespace SabberStoneCoreConsole
{
    public static class StabilityTest
    {
	    private const int TESTCOUNT = 500000;
	    private static Random rnd = new Random();

	    public static void CloneStabilityTest()
	    {
		    Console.WriteLine("Test started");
			for (int i = 0; i < TESTCOUNT; i++)
			{
				var config = new GameConfig
				{
					Player1HeroClass = (CardClass)rnd.Next(2, 11),
					Player2HeroClass = (CardClass)rnd.Next(2, 11),
					FillDecks = true,
					FillDecksPredictably = true,
					Shuffle = false,
					SkipMulligan = true,
					History = false,
					Logging = true,
				};
			    var game = new Game(config);
				game.StartGame();
				List<PlayerTask> optionHistory = new List<PlayerTask>();
				Queue<LogEntry> logs = new Queue<LogEntry>();
				//try
				//{
					do
					{
						while (game.Logs.Count > 0)
							logs.Enqueue(game.Logs.Dequeue());
						game = game.Clone(true);
						List<PlayerTask> options = game.CurrentPlayer.Options();
						PlayerTask option = options[rnd.Next(options.Count)];
						optionHistory.Add(option);
						game.Process(option);
					} while (game.State != State.COMPLETE);
				//} catch (Exception e)
				//{
				//	ShowLog(logs, LogLevel.DEBUG);
				//	Program.ShowLog(game, LogLevel.DEBUG);
				//	Console.WriteLine(e.Message);
				//	Console.WriteLine(e.Source);
				//	Console.WriteLine(e.TargetSite);
				//	Console.WriteLine(e.StackTrace);
				//	PlayerTask last = optionHistory[optionHistory.Count - 1];
				//	Console.WriteLine($"LastOption: {last.FullPrint()}");
				//	break;
				//}


			if (i % (TESTCOUNT / 10) == 0)
					Console.WriteLine($"{((double)i / TESTCOUNT) * 100}% done");
			}
	    }

	    public static void ThreadSafetyTest()
	    {
		    Console.WriteLine("Test started");
		    int i = 0;
		    while (i < TESTCOUNT)
		    {
			    int num = System.Environment.ProcessorCount * 2;
			    var tasks = new Task[num];
			    var cts = new CancellationTokenSource();
			    var token = cts.Token;
			    for (int j = 0; j < tasks.Length; j++)
			    {
				    tasks[j] = new Task(() =>
				    {
					    var config = new GameConfig
					    {
						    Player1HeroClass = (CardClass) rnd.Next(2, 11),
						    Player2HeroClass = (CardClass) rnd.Next(2, 11),
						    FillDecks = true,
						    FillDecksPredictably = true,
						    Shuffle = false,
						    SkipMulligan = true,
						    History = false,
						    Logging = true,
					    };
					    var game = new Game(config);
					    game.StartGame();
						//List<PlayerTask> optionHistory = new List<PlayerTask>();
						//Queue<LogEntry> logs = new Queue<LogEntry>();
						PlayerTask option = null;
					    try
					    {
						    do
						    {
							    //while (game.Logs.Count > 0)
								   // logs.Enqueue(game.Logs.Dequeue());
							    game = game.Clone(true);
							    List<PlayerTask> options = game.CurrentPlayer.Options();
							    option = options[rnd.Next(options.Count)];
							    //optionHistory.Add(option);
							    game.Process(option);
						    } while (game.State != State.COMPLETE);
						}
					    catch (Exception e)
					    {
						    //ShowLog(logs, LogLevel.DEBUG);
						    Program.ShowLog(game, LogLevel.DEBUG);
						    Console.WriteLine(e.Message);
						    Console.WriteLine(e.Source);
						    Console.WriteLine(e.TargetSite);
						    Console.WriteLine(e.StackTrace);
							//  Console.WriteLine($"LastOption: {option?.FullPrint()}");//刪除註解
							cts.Cancel();
					    }
					    if (token.IsCancellationRequested)
						    token.ThrowIfCancellationRequested();

						Interlocked.Increment(ref i);
				    }, token);
			    }

			    for (int j = 0; j < tasks.Length; j++)
				    tasks[j].Start();

			    Task.WaitAll(tasks);


				if (i % (TESTCOUNT / 10) == 0)
				Console.WriteLine($"{((double) i / TESTCOUNT) * 100}% done");
		    }
	    }

	    private static void ShowLog(Queue<LogEntry> logs, LogLevel level)
	    {
		    var str = new StringBuilder();
		    while (logs.Count > 0)
		    {
			    LogEntry logEntry = logs.Dequeue();
			    if (logEntry.Level <= level)
			    {
				    ConsoleColor foreground = ConsoleColor.White;
				    switch (logEntry.Level)
				    {
					    case LogLevel.DUMP:
						    foreground = ConsoleColor.DarkCyan;
						    break;
					    case LogLevel.ERROR:
						    foreground = ConsoleColor.Red;
						    break;
					    case LogLevel.WARNING:
						    foreground = ConsoleColor.DarkRed;
						    break;
					    case LogLevel.INFO:
						    foreground = logEntry.Location.Equals("Game") ? ConsoleColor.Yellow :
							    logEntry.Location.StartsWith("Quest") ? ConsoleColor.Cyan :
							    ConsoleColor.Green;
						    break;
					    case LogLevel.VERBOSE:
						    foreground = ConsoleColor.DarkGreen;
						    break;
					    case LogLevel.DEBUG:
						    foreground = ConsoleColor.DarkGray;
						    break;
					    default:
						    throw new ArgumentOutOfRangeException();
				    }

				    Console.ForegroundColor = foreground;

				    string logStr = $"{logEntry.TimeStamp.ToLongTimeString()} - {logEntry.Level} [{logEntry.BlockType}] - {logEntry.Location}: {logEntry.Text}";
				    str.Append(logStr + "\n");
				    Console.WriteLine(logStr);
			    }
		    }
		    Console.ResetColor();

		    File.WriteAllText(Directory.GetCurrentDirectory() + @"\dump.log", str.ToString());
	    }
	}
}
