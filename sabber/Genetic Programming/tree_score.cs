using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;


namespace GeneticProgramming
{
	class tree_score
	{
		public static string tree_node;
		public static int node_score;
		public static int digits = 1;
		public static int Node_Evaluation()
		{

			Stack mystack = new Stack();
			Stack digit = new Stack();
			// string str = "5 7 + 9 1 + *";//postorder"1 2 + 3 4 + *"
			char[] b = new char[tree_node.Length];


			using (StringReader sr = new StringReader(tree_node))
			{
				// Read 13 characters from the string into the array.
				sr.Read(b, 0, tree_node.Length);

				string[] xxx = new string[tree_node.Length];
				int sum = 0;
				string q;
				string w;
				int digits_sum = 0;
				digit = null;
				for (int i = 0; i < b.Length; i++)//掃堆疊  //b.length
				{
					/*
                    if (i != (b.Length - 1) && (!b[i].Equals(' ')) && (!b[i + 1].Equals(' '))) //大於一位數
                    {

                        digits_sum = int.Parse(b[i].ToString());
                        for (int z = 0; z < digits - 1; z++)
                        {
                            digits_sum = digits_sum * 10;
                        }
                        digits++;
                        digit.Push(digits_sum);
                    }*/
					// else//個位數字
					//  {
					// */
					try
					{
						digits = 1;
						if (digit != null)
						{
							digits_sum = int.Parse(digit.Pop().ToString()) + int.Parse(b[i].ToString());
							mystack.Push(digits_sum);
							digits_sum = 0;
							digit = null;
						}
						else
						{
							/*
                            if (b[i].Equals('+'))//如果掃到+
                            {
                                q = mystack.Pop().ToString();
                                w = mystack.Pop().ToString();
                                sum = int.Parse(q) + int.Parse(w);
                                // Console.WriteLine("q=" + q);
                                // Console.WriteLine("w=" + w);
                                // Console.WriteLine("sum="+sum);
                                mystack.Push(sum);

                            }
                            else if (b[i].Equals('-'))//如果掃到-
                            {
                                q = mystack.Pop().ToString();
                                w = mystack.Pop().ToString();
                                sum = int.Parse(q) - int.Parse(w);
                                mystack.Push(sum);
                            }
                            else if (b[i].Equals('*'))//如果掃到*
                            {
                                q = mystack.Pop().ToString();
                                w = mystack.Pop().ToString();
                                sum = int.Parse(q) * int.Parse(w);
                                mystack.Push(sum);
                            }
                            else if (b[i].Equals('/'))//如果掃到/
                            {
                                q = mystack.Pop().ToString();
                                w = mystack.Pop().ToString();
                                sum = int.Parse(q) / int.Parse(w);
                                mystack.Push(sum);
                            }
                            */

							if (b[i].Equals('a'))//如果掃到a
							{

								mystack.Push(SabberStoneCoreAi.do_MCTS.do_MCTS.num_my_board);

							}
							else if (b[i].Equals('b'))//如果掃到b
							{
								mystack.Push(SabberStoneCoreAi.do_MCTS.do_MCTS.num_op_board);

							}
							else if (b[i].Equals('c'))//如果掃到c
							{
								mystack.Push("12");

							}
							else if (b[i].Equals('d'))//如果掃到d
							{
								mystack.Push("13");

							}
							else if (b[i].Equals('e'))//如果掃到e
							{
								mystack.Push("14");

							}
							else if (b[i].Equals('f'))//如果掃到f
							{
								mystack.Push("15");

							}
							else if (b[i].Equals('g'))//如果掃到g
							{
								mystack.Push("16");

							}
							else if (b[i].Equals('h'))//如果掃到h
							{
								mystack.Push("17");

							}
							else if (b[i].Equals('A'))//如果掃到A(+)
							{

								q = mystack.Pop().ToString();
								w = mystack.Pop().ToString();
								sum = int.Parse(q) + int.Parse(w);
								// Console.WriteLine("q=" + q);
								// Console.WriteLine("w=" + w);
								// Console.WriteLine("sum="+sum);
								mystack.Push(sum);

							}
							else if (b[i].Equals('B'))//如果掃到B(-)
							{

								q = mystack.Pop().ToString();
								w = mystack.Pop().ToString();

								sum = int.Parse(q) - int.Parse(w);
								mystack.Push(sum);

							}
							else if (b[i].Equals('C'))//如果掃到C(*)
							{

								q = mystack.Pop().ToString();
								w = mystack.Pop().ToString();
								sum = int.Parse(q) * int.Parse(w);
								mystack.Push(sum);

							}
							else if (b[i].Equals('D'))//如果掃到D(/)
							{

								q = mystack.Pop().ToString();
								w = mystack.Pop().ToString();

								int p = int.Parse(w);

								if (p == 0)//防止除以零
								{
									p = 1;
								}

								sum = int.Parse(q) / p;
								mystack.Push(sum);

							}
							else if (b[i].Equals('E'))//如果掃到E(^2)
							{
								q = mystack.Pop().ToString();
								sum = int.Parse(q) * int.Parse(q);

							}
							else if (b[i].Equals('F'))//如果掃到F(根號)
							{
								q = mystack.Pop().ToString();
								sum = (int)Math.Sqrt(int.Parse(q));
							}
							else if (b[i].Equals('G'))//如果掃到G(log)
							{
								q = mystack.Pop().ToString();
								sum = (int)Math.Log(int.Parse(q));
							}
							else if (b[i].Equals('H'))//如果掃到H(exp)
							{
								q = mystack.Pop().ToString();
								sum = (int)Math.Exp(int.Parse(q));
							}

							if (!b[i].Equals('a') && !b[i].Equals('b') && !b[i].Equals('c') && !b[i].Equals('d')
								 && !b[i].Equals('e') && !b[i].Equals('f') && !b[i].Equals('g') && !b[i].Equals('h') &&
								 !b[i].Equals('A') && !b[i].Equals('B') && !b[i].Equals('C')
								 && !b[i].Equals('D') && !b[i].Equals('E') && !b[i].Equals('F') && !b[i].Equals('G') && !b[i].Equals('H'))//不是空格或a-h/A-G才加入堆疊
							{
								mystack.Push(b[i]);

							}
							//   }
						}
					}
					catch
					{
						//Console.WriteLine("異常了，沒差繼續執行");
						continue;
					}

					//  Console.WriteLine(b[i]);

				}
				/*
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("sum=" + sum);
                */
				node_score = sum;
				if (node_score <= 0)//防止小於等於零
				{
					node_score = 1;
				}
				/*
                Console.WriteLine();
                */
				/*
                Console.WriteLine("堆疊裡剩下");
                for (int i=0;i<7; i++)//print堆疊
                {
                    Console.WriteLine(mystack.Pop());
                }
                */
				// Read the rest of the string starting at the current string position.
				// Put in the array starting at the 6th array member.
				// sr.Read(b, 5, str.Length - 13);
				// Console.WriteLine(b);

				//Console.ReadLine();
			}

			return node_score;
		}
	}
}
