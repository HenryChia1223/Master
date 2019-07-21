using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static GeneticProgramming.create_tree;
using TreeScore.treescore;
using System.Threading;
using System.Threading.Tasks;


namespace GeneticProgramming
{
	class Program
	{
		//public static int return_winrate = 0;
		
		public static object countLock = new object();
		public static Random rand = new Random();
		public static List<List<Node>> population = new List<List<Node>>();//
		public static List<List<Node>> next_population = new List<List<Node>>();//下一代族群
		public static List<Node> reb = new List<Node>();
		public static List<Node> bst_list = new List<Node>();
		public static List<Node> po;
		public static List<Node> Cross_PostOrder_list_1 = new List<Node>();
		public static List<Node> Cross_PostOrder_list_2 = new List<Node>();
		//public static Dictionary<List<Node>, int> tree_fitness = new Dictionary<List<Node>, int>();//用tree查fitness
		public static List<Node> child1 = new List<Node>();//1st子代
		public static List<Node> child2 = new List<Node>();//2nd子代
		public static Dictionary<int, Node> selection_rate = new Dictionary<int, Node>();//機率字典,輪盤法選到的機率
		//public static int best_difference = Int32.MaxValue;//所有代差最少
		//public static int second_difference = Int32.MaxValue;//所有代差次少
		public static List<Node> best_tree = new List<Node>();//最佳樹
		public static List<Node> best_tree_inorder = new List<Node>();//最佳樹
		public static List<Node> second_tree = new List<Node>();//次佳樹
		public static List<Node> cross1_tree = new List<Node>();//cross1
		public static List<Node> cross2_tree = new List<Node>();//cross2
		public static List<Node> store_score = new List<Node>();
		public static int best_score;//所有代中最好的
		public static int second_score;
		public static int ger_best_socre = Int32.MinValue;//每代最好
		public static int ger_second_socre = Int32.MinValue;//每代次好
		public static string str_best_post="";
		public static string str_best_in="";
		public static string find;
		public static string s_start;
		public static int first_score;//初代分數
		public static Dictionary<Node, int> tem_rate = new Dictionary<Node, int>();
		public static Dictionary<string, int> dic_score_preorder = new Dictionary<string, int>();//分數字典

		public static int poor1_in_ger = Int32.MaxValue;//每代最差
		public static int poor2_in_ger = Int32.MaxValue;//每代次差
		//public static int poor3_difference_in_ger = Int32.MinValue;//每代3rd差
		//public static int poor4_difference_in_ger = Int32.MinValue;//每代4th差

		public static List<Node> poor1_tree_in_ger = new List<Node>();//每代最差樹
		public static List<Node> poor2_tree_in_ger = new List<Node>();//每代次差樹
		//public static List<Node> poor3_tree_in_ger = new List<Node>();//每代3rd差樹
		//public static List<Node> poor4_tree_in_ger = new List<Node>();//每代4th差樹

		//public static int best_difference_in_ger = Int32.MinValue;//每代差最少
		//public static int second_difference_in_ger = Int32.MinValue;//每代差次少
	
		public static List<Node> best_tree_in_ger = new List<Node>();//每代最佳樹
		public static List<Node> second_tree_in_ger = new List<Node>();//每代次佳樹
		public static int fitness_sum = 0;
		public static int winrate_score = -1;
		public static int sum = 0;

		//public static List<List<Node>> add_pop = new List<List<Node>>();//突變新增名單
		//public static List<List<Node>> del_pop = new List<List<Node>>();//突變刪除名單

		public static int population_size = 100;//人口數   //3 2000*2000 300M//
		public static int generation_num = 100;//世代數
		public static double mutation_rate = 0.1;//突變率
		public static double cross_rate = 0.8;//交配率

		static void Main(string[] args)
		{
			DateTime st = DateTime.Now;

			create_tree.Encode();//編碼


			//產生初代population並存入字典
			for (int i = 0; i < population_size; i++)//根據tree_list產生的
			{
				po = new List<Node>(create_tree.Create_tree());
				population.Add(po);
				//  Console.WriteLine();
			}

			for (int g = 0; g < generation_num; g++)//菁英法
			{
				Console.WriteLine("---------------------第" + (g + 1) + "代開始------------------------");
				ger_best_socre = Int32.MinValue;
				ger_second_socre = Int32.MinValue;
				//-------------generate_population--------------
				//  Console.WriteLine("第" + g  + "代");


				//------------selection---------------
				selection();

				//複製到下一代族群
				next_population = population.ToList();

				if (poor1_tree_in_ger.Count != 0)
				{//有每代最差就刪除
					foreach (List<Node> p in next_population.ToList())
					{
						string find = p[0].rebuild_pre(p[0]);
						p[0].preorder_path = "";
						reb.Clear();
						if (dic_score_preorder[find] == poor1_in_ger)
						{
							Console.WriteLine("1刪除前next_population有" + next_population.Count + "個");
							Console.WriteLine("1刪除最差");
							next_population.Remove(p);
							Console.WriteLine("1刪完後next_population剩下" + next_population.Count + "個");
							reb.Clear();
							break;
						}
						reb.Clear();
					}
				}
				else
				{

					int a = rand.Next(0, next_population.Count);
					Console.WriteLine("ELSE1刪除前next_population有" + next_population.Count + "個");
					next_population.RemoveAt(a);
					Console.WriteLine("ELSE1刪完後next_population剩下" + next_population.Count + "個");
				}




				if (poor2_tree_in_ger.Count != 0)
				{//有每代次差就刪除
					foreach (List<Node> p in next_population.ToList())
					{
						string find = p[0].rebuild_pre(p[0]);
						p[0].preorder_path = "";
						reb.Clear();
						if (dic_score_preorder[find] == poor2_in_ger)
						{
							Console.WriteLine("2刪除前next_population有" + next_population.Count + "個");
							Console.WriteLine("2刪除次差");
							next_population.Remove(p);
							Console.WriteLine("2刪完後next_population剩下" + next_population.Count + "個");
							reb.Clear();
							break;
						}
						reb.Clear();
					}
				}
				else
				{
					int a = rand.Next(0, next_population.Count);
					Console.WriteLine("ELSE2刪除前next_population有" + next_population.Count + "個");
					next_population.RemoveAt(a);
					Console.WriteLine("ELSE2刪完後next_population剩下" + next_population.Count + "個");
				}



				int popu_size = next_population.Count;//每代人口


				//交配率0.8
				int cr = rand.Next(1, 11);
				if (cr == 1 || cr == 2) //不交配
				{
					Console.WriteLine("沒發生交配");
				}
				else //交配
				{
					Console.WriteLine("發生交配");

					//輪盤法抽出誰能交配
					Lotto();


					//------------crossover---------------
					crossover(cross1_tree, cross2_tree);//一次產生兩個子代,更新child1 child2

					po = new List<Node>(child1);
					next_population.Add(po);


					po = new List<Node>(child2);
					next_population.Add(po);

					popu_size += 2;

				}
				Console.WriteLine("補滿前popilation=" + next_population.Count+"個");

				while (popu_size < population_size)
				{
					//從population 抽出人到下一代,直到補滿next

					int zzz = rand.Next(1, next_population.Count);//選一個突變,除了root
					next_population.Add(mutation(next_population[zzz]));

					popu_size++;


				}
				Console.WriteLine("補滿後popilation=" + next_population.Count + "個");
				Console.WriteLine("best score:" + best_score);

				//string s = best_tree[0].PostOrder(best_tree[0]);
				//best_tree[0].postorder_path = "";
				Console.WriteLine("best_tree postorder:" + str_best_post);

				//s= best_tree[0].InOrder(best_tree[0]);
				//best_tree[0].inorder_path = "";
				Console.WriteLine("best_tree inorder:" + str_best_in);

				//Console.WriteLine("---------------------------------------------");


				//------------------世代結束----------------------


				child1.Clear();
				child2.Clear();
				selection_rate.Clear();
				population = next_population.ToList();
				next_population.Clear();



				//-----------------------mutation-----------------------------
			
				//選一個population有可能突變

				int mu_rate = rand.Next(0, 10);//0.1
				if (mu_rate == 8 )
				{
					Console.WriteLine("發生突變");
					int zzz = rand.Next(1, population.Count);//選一個突變,除了root

					population[zzz] = (mutation(population[zzz])).ToList();
				}
				else
				{
					Console.WriteLine("沒發生突變");
					//不突變
				}


				Console.WriteLine("generation best score:" + ger_best_socre);

				string s = best_tree_in_ger[0].PostOrder(best_tree_in_ger[0]);
				best_tree_in_ger[0].postorder_path = "";
				Console.WriteLine("best_tree_in_generation  postorder:" + s);

		
				poor1_in_ger = Int32.MaxValue;
				poor2_in_ger = Int32.MaxValue;
				best_tree_in_ger.Clear();
				second_tree_in_ger.Clear();
				poor1_tree_in_ger.Clear();
				poor2_tree_in_ger.Clear();
				sum = 0;
				fitness_sum = 0;
				winrate_score = -1;
				// ger_best_socre = Int32.MinValue;
				// ger_second_socre = Int32.MinValue;
				if (g == 0)
				{
					first_score = ger_best_socre;
				}
				Console.WriteLine("初代最佳= " + first_score +"%");
				Console.WriteLine("---------------------第" + (g + 1) + "代結束------------------------");
				
				if (g == generation_num - 1)
				{
					DateTime end = DateTime.Now;
					Console.WriteLine("共耗時" + ((TimeSpan)(end - st)).TotalMilliseconds + "Milliseconds");
				}
				

			}



			Console.ReadLine();

		}



		public static void selection()
		{
			////輪盤法

			////先算出每個個體的fitness存入字典
			//foreach (List<Node> p in population)
			//{
			//	try
			//	{

			//		fitness(p);

			//	}
			//	catch (System.IndexOutOfRangeException e)
			//	{
			//		// Set IndexOutOfRangeException to the new exception's InnerException.
			//		Console.WriteLine("index parameter is out of range.");
			//		throw new System.ArgumentOutOfRangeException("index parameter is out of range.", e);
			//	}
			//}

			//使用thread
			int WORKER_COUNT = 4;
			Thread[] workers = new Thread[WORKER_COUNT];
			int jobsCountPerWorker = population_size / WORKER_COUNT;




			for (int i = 0; i < WORKER_COUNT; i++)
			{
				//將全部工作切成WORKER_COUNT份，
				//分給WORKER_COUNT個Thread執行
				int st = jobsCountPerWorker * i;
				int ed = jobsCountPerWorker * (i + 1);
				if (ed > population_size) ed = population_size;
				workers[i] = new Thread(() =>
				{
					//Console.WriteLine("LOOP: {0:N0} - {1:N0}", st, ed);
					for (int j = st; j < ed; j++)
					{
						
						fitness(population[j]);
					}
				});
				
				workers[i].Start();
				Thread.Sleep(100);
			}
			for (int i = 0; i < WORKER_COUNT; i++)
				workers[i].Join();



		}

		
		public static void Lotto()
		{

			//fitness愈大愈好
			//抽籤
			int a = rand.Next(1, fitness_sum + 1);
			// Console.WriteLine("1st抽到" + a + "號");

			//用postorder重新建一次List<Node>
			string p1 = selection_rate[a].rebuild_pre(selection_rate[a]);
			selection_rate[a].preorder_path = "";
			cross1_tree = reb.ToList();//被抽到的放進cross1
			reb.Clear();


			int b = rand.Next(1, fitness_sum + 1);
			while (selection_rate[b].Equals(selection_rate[a]))//若抽到cross1
			{
				b = rand.Next(1, fitness_sum + 1);//再抽一次
			}
			// Console.WriteLine("2ndt抽到" + b + "號");

			string p2 = selection_rate[b].rebuild_pre(selection_rate[b]);
			selection_rate[b].preorder_path = "";
			cross2_tree = reb.ToList();//被抽到的放進cross2
			reb.Clear();

		}



		public static void crossover(List<Node> cross1, List<Node> cross2)
		{
			List<Node> better_cross = new List<Node>();


			//crossovwe_test     1.(尚未限制交換後總層數,未來可用交換後總層數不大於5才能交換)
			//                   2.沒有crossover rate
			//                   3.tree大小沒有限制,一直cross有可能stack overflow

			string st = cross1[0].PostOrder(cross1[0]);
			 Console.WriteLine("cross1=" + st);
			cross1[0].postorder_path = "";

			st = cross2[0].PostOrder(cross2[0]);
			 Console.WriteLine("cross2=" + st);
			cross2[0].postorder_path = "";




			//選出兩棵樹各自要交換的節點子樹
			int a1 = rand.Next(1, cross1.Count);
			int b1 = rand.Next(1, cross2.Count);

			// Console.WriteLine("cross1.count" +cross1.Count);
			//  Console.WriteLine("a1=" + a1);
			//限制大小,判斷層數
			int hight1 = cross1[a1].High;
			int hight2 = cross2[b1].High;



			//判斷要交換的node個數
			string p1 = cross1[a1].Cross_PostOrder_1(cross1[a1]);
			cross1[a1].postorder_path = "";
			string p2 = cross2[b1].Cross_PostOrder_2(cross2[b1]);
			cross2[b1].postorder_path = "";
			int c1 = Cross_PostOrder_list_1.Count;
			int c2 = Cross_PostOrder_list_2.Count;
			//renew Cross_Postorder_1和Cross_Postorder_2
			create_tree.Node.renew_Cross_Postorder_1(Cross_PostOrder_list_1[0]);
			create_tree.Node.renew_Cross_Postorder_2(Cross_PostOrder_list_2[0]);




			//可以建立幾層,幾個node
			double num1 = Math.Pow(2, (create_tree.Max_hight - cross1[a1].High + 1)) - 1;//cross1可以建立幾個node
			double num2 = Math.Pow(2, (create_tree.Max_hight - cross2[b1].High + 1)) - 1;//cross2可以建立幾個node


			int c = 1;
			while (c2 > num1 || c1 > num2)//交配後會超出層數,無法交配
			{

				//Console.WriteLine("重抽  " + "hight1=" + hight1 + "  hight2=" + hight2+"  c1有="+c1+"個"+ 
				//    "  c2有=" + c2 + "個"+"  num1可以"+num1+"個" + "  num2可以" + num2 + "個");
				a1 = rand.Next(1, cross1.Count);
				hight1 = cross1[a1].High;
				b1 = rand.Next(1, cross2.Count);
				hight2 = cross2[b1].High;

				p1 = cross1[a1].Cross_PostOrder_1(cross1[a1]);
				cross1[a1].postorder_path = "";
				p2 = cross2[b1].Cross_PostOrder_2(cross2[b1]);
				cross2[b1].postorder_path = "";
				c1 = Cross_PostOrder_list_1.Count;
				c2 = Cross_PostOrder_list_2.Count;
				//renew Cross_Postorder_1和Cross_Postorder_2
				create_tree.Node.renew_Cross_Postorder_1(Cross_PostOrder_list_1[0]);
				create_tree.Node.renew_Cross_Postorder_2(Cross_PostOrder_list_2[0]);

				num1 = Math.Pow(2, (create_tree.Max_hight - cross1[a1].High + 1)) - 1;//cross1可以建立幾個node
				num2 = Math.Pow(2, (create_tree.Max_hight - cross2[b1].High + 1)) - 1;//cross2可以建立幾個node

				if (c == 100)
				{
					child1 = cross1;
					child2 = cross2;
					// Console.WriteLine("交配100次不成功,不交配了!");
					return;
				}

				c++;
			}




			string s1 = cross1[a1].Cross_PostOrder_1(cross1[a1]);
			cross1[a1].postorder_path = "";
			string s2 = cross2[b1].Cross_PostOrder_2(cross2[b1]);
			cross2[b1].postorder_path = "";





			Node a1_copy = new Node(cross1[a1].ID, cross1[a1].Parent, cross1[a1].Left, cross1[a1].Right,
							cross1[a1].High, cross1[a1].TERorOP, cross1[a1].Action);//複製1st目標節點

			Node b1_copy = new Node(cross2[b1].ID, cross2[b1].Parent, cross2[b1].Left, cross2[b1].Right,
						 cross2[b1].High, cross2[b1].TERorOP, cross2[b1].Action);//複製2nd目標節點

			//找出目標是誰的Left_child或Right_child並連接上新目標_1st
			foreach (Node n in cross1)//1st要交換的
			{
				//判斷葉節點直接跳過迴圈
				if (n.Left == null && n.Right == null)//葉節點
				{
					//
				}
				else if (n.Left != null && n.Right != null)
				{ //左右子節點都有

					//有找到
					if (n.Left.ID == a1_copy.ID)//左子節點id=目標節點id
					{
						n.Left = b1_copy;//b1_copy
										 //  n.Children.Add(b1_copy);//
						b1_copy.Parent = n;//
										   //  b1_copy.High = n.High + 1;//設定子代的hight
					}
					else if (n.Right.ID == a1_copy.ID)//右子節點id=目標節點id
					{
						n.Right = b1_copy;//
										  //   n.Children.Add(b1_copy);//
						b1_copy.Parent = n;//
										   //  b1_copy.High = n.High + 1;//設定子代的hight
					}

					//沒找到
					else
					{
						//
					}
				}//else if
				else   //只有一個子節點
				{

					if (n.Left != null)//有左節點
					{
						if (n.Left.ID == a1_copy.ID)
						{//左子節點id=目標節點id
							n.Left = b1_copy;//
											 //   n.Children.Add(b1_copy);//
							b1_copy.Parent = n;//
											   //  b1_copy.High = n.High + 1;//設定子代的hight
						}
					}
					else if (n.Right != null) //有右節點
					{
						if (n.Right.ID == a1_copy.ID)
						{//右子節點id=目標節點id
							n.Right = b1_copy;//
											  //   n.Children.Add(b1_copy);//
							b1_copy.Parent = n;//
											   //  b1_copy.High = n.High + 1;//設定子代的hight

						}
					}
					//沒找到
					else
					{
						//
					}

				}


			}
			//找出目標是誰的Left_child或Right_child並連接上新目標_2nd
			foreach (Node n in cross2)//2nd要交換的(被交換)
			{
				//判斷葉節點直接跳過迴圈
				if (n.Left == null && n.Right == null)//葉節點
				{
					//
				}
				else if (n.Left != null && n.Right != null)
				{ //左右子節點都有

					//有找到
					if (n.Left.ID == b1_copy.ID)//左子節點id=目標節點id
					{
						n.Left = a1_copy;//a1_copy
										 //  n.Children.Add(a1_copy);//
						a1_copy.Parent = n;//
										   //  a1_copy.High = n.High + 1;//設定子代的hight
					}
					else if (n.Right.ID == b1_copy.ID)//右子節點id=目標節點id
					{
						n.Right = a1_copy;//
										  // n.Children.Add(a1_copy);//
						a1_copy.Parent = n;//
										   //  a1_copy.High = n.High + 1;//設定子代的hight
					}

					//沒找到
					else
					{
						//
					}
				}//else if
				else   //只有一個子節點
				{


					if (n.Left != null)//有左節點
					{
						if (n.Left.ID == b1_copy.ID)
						{
							n.Left = a1_copy;//
											 //   n.Children.Add(a1_copy);//
							a1_copy.Parent = n;
							// a1_copy.High = n.High + 1;//設定子代的hight
						}


					}
					else if (n.Right.ID == b1_copy.ID)
					{
						n.Right = a1_copy;//
										  //  n.Children.Add(a1_copy);//
						a1_copy.Parent = n;//
										   // a1_copy.High = n.High + 1;//設定子代的hight
					}

					else
					{
						//
					}


				}


			}


			//重新設定已交換子樹的hight

			string ra = a1_copy.rebuild_pre(a1_copy);
			create_tree.Node.bst(reb);

			foreach (Node n in bst_list)
			{
				if (n.High < Max_hight)
				{
					n.High = n.Parent.High + 1;
				}

			}
			a1_copy.preorder_path = "";
			reb.Clear();
			bst_list.Clear();


			ra = b1_copy.rebuild_pre(b1_copy);
			create_tree.Node.bst(reb);

			foreach (Node n in bst_list)
			{
				if (n.High < Max_hight)
				{
					n.High = n.Parent.High + 1;
				}

			}
			b1_copy.preorder_path = "";
			reb.Clear();
			bst_list.Clear();

			//複製與刪除節點到List
			// Console.WriteLine("-------------交換後-------------");


			//1st
			foreach (Node n in Cross_PostOrder_list_1)//1st複製到2nd
			{

				cross2.Add(n);//複製到2nd

				foreach (Node p in cross1.ToArray())//刪除要複製過去的
				{
					if (n.ID == p.ID)
					{
						cross1.Remove(p);
					}
				}
			}



			//2nd
			foreach (Node n in Cross_PostOrder_list_2)//2nd複製到1st
			{
				cross1.Add(n);//複製到1st

				foreach (Node p in cross2.ToArray())//刪除要複製過去的
				{
					if (n.ID == p.ID)
					{
						cross2.Remove(p);
					}
				}
			}

			//renew Cross_Postorder_1和Cross_Postorder_2
			create_tree.Node.renew_Cross_Postorder_1(Cross_PostOrder_list_1[0]);
			create_tree.Node.renew_Cross_Postorder_2(Cross_PostOrder_list_2[0]);



			string as1 = cross1[0].PostOrder(cross1[0]);
			cross1[0].postorder_path = "";
			string as2 = cross2[0].PostOrder(cross2[0]);
			cross2[0].postorder_path = "";

			// Console.WriteLine("1st交換後的節點子樹有post字串:" + as1);
			// Console.WriteLine("2nd交換後的節點子樹有post字串:" + as2);


			string ai1 = cross1[0].InOrder(cross1[0]);
			cross1[0].inorder_path = "";
			string ai2 = cross2[0].InOrder(cross2[0]);
			cross2[0].inorder_path = "";

			// Console.WriteLine("1st交換後的節點子樹有in字串:" + ai1);
			// Console.WriteLine("2nd交換後的節點子樹有in字串:" + ai2);





			//-------------產生兩個子代----------------
			child1 = cross1;
			child2 = cross2;


		}

		public static List<Node> mutation(List<Node> mutate_population)
		{
			//del_pop.Add(mutate_population);

			int a;

			//1.數值突變
			int rate;//選擇1. 或 2. 的機率

			if (mutate_population.Count == 2)//整棵樹只有兩個節點,只有一個節點能選
			{
				a = 1;
			}
			else if (mutate_population.Count == 1)
			{
				int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
				Node n = new Node(node_id + 1, mutate_population[0], null, null, mutate_population[0].High + 1, 1, Termianl[t]);
				mutate_population[0].Left = n;
				//  mutate_population[0].Children.Add(n);//
				n.Parent = mutate_population[0];//
				mutate_population.Add(n);
				a = 1;
			}
			else if (mutate_population.Count == 0)
			{

				a = 0;
			}
			else
			{
				a = rand.Next(1, mutate_population.Count);//選擇突變位置(除了root)
			}


			//ternimal
			if (mutate_population[a].TERorOP == 1)
			{
				rate = rand.Next(1, 3);
				int h = mutate_population[a].High;

				//1.數值突變
				if (rate == 1 || h == create_tree.Max_hight)//抽到1或本身是最底層ternimal,不能再建立樹
				{
					int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
					while (mutate_population[a].Action.Equals(Termianl[t]))//重複
					{
						t = rand.Next(1, Termianl.Count + 1);//再挑一次
					}
					mutate_population[a].Action = Termianl[t];
					return mutate_population;
					
				}
				else if (rate == 2)
				{


					//2.結構突變
					//將terminal改成operator(tree越來越大)(建立一顆子樹)
					int oper = rand.Next(1, Operator.Count + 1);//從operator隨機挑一個

					Node r = new Node(mutate_population[a].ID, mutate_population[a].Parent, null, null, mutate_population[a].High,
						2, Operator[oper]);//改為隨機一個operator



					//可以建立幾層,幾個node
					double num = Math.Pow(2, (create_tree.Max_hight - r.High + 1)) - 1;



					for (int i = 0; i < ((int)num) - 1; i++)
					{
						Node node = r.Selection();
						node = node.Expand();
					}



					Node a1_copy = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
							mutate_population[a].High, mutate_population[a].TERorOP, mutate_population[a].Action);//複製1st目標節點


					//parent 連過來
					//找出目標是誰的Left_child或Right_child並連接上新目標_1st
					foreach (Node n in mutate_population)//1st要交換的
					{
						//判斷葉節點直接跳過迴圈
						if (n.Left == null && n.Right == null)//葉節點
						{
							//
						}
						else if (n.Left != null && n.Right != null)
						{ //左右子節點都有

							//有找到
							if (n.Left.ID == a1_copy.ID)//左子節點id=目標節點id
							{

								n.Left = r;//
										   // n.Children.Add(r);//
								r.Parent = n;//


							}
							else if (n.Right.ID == a1_copy.ID)//右子節點id=目標節點id
							{

								n.Right = r;//
											//   n.Children.Add(r);//
								r.Parent = n;//

							}

							//沒找到
							else
							{
								//
							}
						}//else if
						else   //只有一個子節點
						{

							if (n.Left != null)//有左節點
							{
								if (n.Left.ID == a1_copy.ID)
								{//左子節點id=目標節點id

									n.Left = r;//
											   //   n.Children.Add(r);//
									r.Parent = n;//

								}
							}
							else if (n.Right != null) //有右節點
							{
								if (n.Right.ID == a1_copy.ID)
								{//右子節點id=目標節點id

									n.Right = r;//
												//  n.Children.Add(r);//
									r.Parent = n;//

								}
							}
							//沒找到
							else
							{
								//
							}

						}


					}
					return mutate_population;
				}
				else
				{

					Console.WriteLine("突變率錯誤");
					return mutate_population;
				}

			}

			//operator
			else if (mutate_population[a].TERorOP == 2)
			{
				rate = rand.Next(1, 3);

				//1.數值突變
				if (rate == 1)
				{
					int t = rand.Next(1, Operator.Count + 1);//從terminal隨機挑一個
					while (mutate_population[a].Action.Equals(Operator[t]))//重複
					{
						t = rand.Next(1, Operator.Count + 1);//再挑一次
					}
					mutate_population[a].Action = Operator[t];
					return mutate_population;
					//add_pop.Add(mutate_population);
				}

				//2.結構突變
				//將operator改成terminal(tree越來越小)
				else if (rate == 2)
				{
					Node a1_copy = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
							mutate_population[a].High, mutate_population[a].TERorOP, mutate_population[a].Action);//複製1st目標節點

					//找出目標是誰的Left_child或Right_child並連接上新目標_1st
					foreach (Node n in mutate_population)//1st要交換的
					{
						//判斷葉節點直接跳過迴圈
						if (n.Left == null && n.Right == null)//葉節點
						{
							//
						}
						else if (n.Left != null && n.Right != null)
						{ //左右子節點都有

							//有找到
							if (n.Left.ID == a1_copy.ID)//左子節點id=目標節點id
							{

								int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
								Node c = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
											mutate_population[a].High, 1, Termianl[t]);//複製1st目標節點

								n.Left = c;//
										   //   n.Children.Add(c);//
								c.Parent = n;//


							}
							else if (n.Right.ID == a1_copy.ID)//右子節點id=目標節點id
							{
								int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
								Node c = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
											mutate_population[a].High, 1, Termianl[t]);//複製1st目標節點

								n.Right = c;//
											//  n.Children.Add(c);//
								c.Parent = n;//

							}

							//沒找到
							else
							{
								//
							}
						}//else if
						else   //只有一個子節點
						{

							if (n.Left != null)//有左節點
							{
								if (n.Left.ID == a1_copy.ID)
								{//左子節點id=目標節點id
									int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
									Node c = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
												mutate_population[a].High, 1, Termianl[t]);//複製1st目標節點

									n.Left = c;//
											   //   n.Children.Add(c);//
									c.Parent = n;//

								}
							}
							else if (n.Right != null) //有右節點
							{
								if (n.Right.ID == a1_copy.ID)
								{//右子節點id=目標節點id
									int t = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個
									Node c = new Node(mutate_population[a].ID, mutate_population[a].Parent, mutate_population[a].Left, mutate_population[a].Right,
												mutate_population[a].High, 1, Termianl[t]);//複製1st目標節點

									n.Right = c;//
												//   n.Children.Add(c);//
									c.Parent = n;//

								}
							}
							//沒找到
							else
							{
								//
							}

						}


					}


					//把目標節點子樹刪除
					string s1 = (mutate_population[a].Cross_PostOrder_1((mutate_population[a])));
					mutate_population[a].postorder_path = "";


					//1st
					foreach (Node n in Cross_PostOrder_list_1)
					{

						foreach (Node p in (mutate_population.ToArray()))//刪除要複製過去的
						{
							if (n.ID == p.ID)
							{
								mutate_population.Remove(p);
								//p.clear
							}
						}
					}

					create_tree.Node.renew_Cross_Postorder_1(Cross_PostOrder_list_1[0]);
					return mutate_population;
					
				}

				else
				{
					Console.WriteLine("突變率錯誤");
					return mutate_population;
				}






			}
			return mutate_population;

		}


		public static void fitness(List<Node> tree)//算勝率，存入字典winrate
		{
		

			lock (countLock)
			{
				//查字典,若已經算過fitness就不用再算了
				find = tree[0].rebuild_pre(tree[0]);
				tree[0].preorder_path = "";
				reb.Clear();
			}
				
			if (dic_score_preorder.ContainsKey(find))//若查到
			{
				Console.WriteLine("查詢到字典了,使用字典的勝率");
				Console.WriteLine();
				lock (countLock)
				{
					winrate_score = dic_score_preorder[find];
				}
			}
			else
			{//沒查到
				Console.WriteLine("沒查到,開始計算fitness");
			
				lock (countLock) { 
					//要算fitness
					//丟進postorder轉字串
					s_start = tree[0].PostOrder(tree[0]);
					tree[0].postorder_path = "";
				}
				
				SabberStoneCoreAi.src.Program.main.main_start(s_start);

				Console.WriteLine("算完了");
				Console.WriteLine("勝率為playA:" + SabberStoneCoreAi.POGame.GameStats.return_winrate);
				Console.WriteLine();
				lock (countLock) { 
					winrate_score = SabberStoneCoreAi.POGame.GameStats.return_winrate;
				}
			}

			
			lock (countLock)//全域變數,一次只能一條thread更改
			{
				if (winrate_score == 0)
				{
					winrate_score = 1;
				}


				//把每棵樹root存入分數字典

				if (!dic_score_preorder.ContainsKey(find))
				{
					dic_score_preorder.Add(find, winrate_score);
				}

				//firness愈大愈好
				int range1 = fitness_sum + 1; //此個體的fintess所佔的機率範圍,下界
				int range2 = fitness_sum + winrate_score;//上界

				for (int i = range1; i <= range2; i++)
				{
					if (!selection_rate.ContainsKey(i)) //建立機率字典
					{
						//dic = new Dictionary<int, List<Node>>(i, tree);
						selection_rate.Add(i, tree[0]);
					}
				}
				fitness_sum += winrate_score; //加入futness_sum


				if (winrate_score > ger_best_socre) //找每代最佳
				{
					ger_best_socre = winrate_score;
					//best_tree_in_ger = tree.ToList();



					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";


					best_tree_in_ger.Clear();
					foreach (Node n in reb)//複製每代最佳樹
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						best_tree_in_ger.Add(new_tree);
					}

					reb.Clear();

				}
				else if (winrate_score < ger_best_socre && winrate_score > ger_second_socre)//每代次佳
				{
					ger_second_socre = winrate_score;
					second_tree_in_ger = tree.ToList();



					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";

					second_tree_in_ger.Clear();
					foreach (Node n in reb)//複製每代次佳樹
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						second_tree_in_ger.Add(new_tree);
					}
					reb.Clear();

				}



				if (winrate_score < poor1_in_ger) //找每代最差
				{
					poor1_in_ger = winrate_score;
					//poor1_tree_in_ger = tree.ToList();


					// ger_best_socre = tree_score.node_score;


					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";

					poor1_tree_in_ger.Clear();
					foreach (Node n in reb)//複製每代最差
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						poor1_tree_in_ger.Add(new_tree);
					}
					reb.Clear();

				}
				else if (winrate_score > poor1_in_ger && winrate_score < poor2_in_ger)
				{
					poor2_in_ger = winrate_score;
					//poor2_tree_in_ger = tree.ToList();

					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";

					poor2_tree_in_ger.Clear();
					foreach (Node n in reb)//複製每代次差
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						poor2_tree_in_ger.Add(new_tree);
					}
					reb.Clear();

				}



				if (winrate_score > best_score) //找最佳
				{
					best_score = winrate_score;
					//best_tree = tree.ToList();

					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";

					best_tree.Clear();
					foreach (Node n in reb)//複製最佳樹
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						best_tree.Add(new_tree);
					}
					reb.Clear();

					str_best_post = tree[0].PostOrder(tree[0]);
					tree[0].postorder_path = "";


					str_best_in = tree[0].InOrder(tree[0]);
					tree[0].inorder_path = "";



				}
				else if (winrate_score < best_score && winrate_score > second_score) //找次佳
				{

					second_score = winrate_score;
					// second_tree = tree.ToList();


					string p2 = tree[0].rebuild_pre(tree[0]);
					tree[0].preorder_path = "";

					second_tree.Clear();
					foreach (Node n in reb)//複製次佳樹
					{
						Node new_tree = new Node(n.ID, n.Parent, n.Left, n.Right, n.High, n.TERorOP, n.Action);
						second_tree.Add(new_tree);
					}
					reb.Clear();

				}

			}


			

		}




	}
}
