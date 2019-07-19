using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Score;
using SabberStoneCoreAi.POGame;

namespace GeneticProgramming
{
	class create_tree
	{
		public static int Max_hight = 5;//層數,cross後最多
		public static int num_node = 31;//node總數
		public static int node_id = 0;
		public static Random rand = new Random();
		public static int count = 1;
		public static Dictionary<int, string> Termianl = new Dictionary<int, string>();
		public static Dictionary<int, string> Operator = new Dictionary<int, string>();

		public static List<Node> Create_tree()
		{
			Node.tree_list.Clear();



			int a = rand.Next(1, Operator.Count + 1);//從operator隨機挑一個

			node_id++;
			Node root = new Node(node_id, null, null, null, 1, 2, Operator[a]);//建立root,必為oper

			Node.tree_list.Add(root);

			for (int i = 0; i < num_node - 1; i++)
			{
				Node node = root.Selection();
				node = node.Expand();

			}

			string st = root.PostOrder(root);


			string ino_st = root.InOrder(root);
			Node.renew_inorder(root);

			if (!Node.dic_post_tree.ContainsKey(st))
			{
				Node.dic_post_tree.Add(st, Node.tree_list);
			}


			Node.get_tree_list_act(st);

			Node.renew_postorder(root);


			return Node.tree_list;
		}

		public class Node
		{

			public int ID;
			public Node Parent = null;
			public Node Left = null;
			public Node Right = null;
			public int High;
			public string Action;
			public int TERorOP;
			// public List<Node> Children { get; private set; } = null;
			public string postorder_path = "";
			public string inorder_path = "";
			public string preorder_path = "";
			public static Dictionary<string, List<Node>> dic_post_tree = new Dictionary<string, List<Node>>();

			public static List<Node> tree_list = new List<Node>();

			public Node(int id, Node parent, Node left, Node right, int high, int teroroper, string action)
			{
				Parent = parent;
				Action = action;
				Left = left;
				Right = right;
				High = high;
				ID = id;
				TERorOP = teroroper;//1為ter，2為Oper
									// Children = new List<Node>();
			}


			public Node Selection()
			{

				if (this.Left != null && this.Right != null)//子節點建滿了
				{
					int a = rand.Next(0, 2); //隨機選子節點
					if (a == 0)//左子樹
					{
						return this.Left.Selection();
					}
					else if (a == 1)
					{//右子樹

						return this.Right.Selection();
					}
				}

				return this;
			}

			public Node Expand()
			{
				if (this.High != Max_hight)//不是最後一層
				{

					if (this.High == 1)//root判斷
					{
						if (this.Action.Equals(Operator[5]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//  this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[6]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[7]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//  this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[8]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
					}


					if (this.High == Max_hight - 1)//此節點是倒數第二層，只能建立terminal節點
					{


						if (this.Action.Equals(Operator[5]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//  this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[6]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[7]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									// this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						else if (this.Action.Equals(Operator[8]))
						{
							if (this.Left != null)
							{
								return null;//直接return null
							}
							else
							{
								int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

								if (this.Left == null)//左子樹
								{

									node_id++;
									Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
									this.Left = child;
									//this.Children.Add(child);
									tree_list.Add(child);
									return null;
								}

							}
						}
						if (this.TERorOP == 1)//為termianl，無法再建立
						{
							return null;
						}
						else if (this.TERorOP == 2)
						{
							int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

							if (this.Left == null)//左子樹
							{

								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
								this.Left = child;
								// this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
							else if (this.Right == null)//右子樹
							{
								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
								this.Right = child;
								// this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
						}
						else
						{
							return null;
							//
						}

					}
					else//可隨機建立節點
					{

						int x = rand.Next(0, 2);

						if (x == 0)//建立terminal
						{
							int a = rand.Next(1, Termianl.Count + 1);//從terminal隨機挑一個

							if (this.Left == null)//左子樹
							{
								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
								this.Left = child;
								// this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
							else if (this.Right == null)//右子樹
							{
								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 1, Termianl[a]);
								this.Right = child;
								//this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
						}

						else if (x == 1)//建立oper
						{
							if (this.Action.Equals(Operator[5]) && this.Left != null)//此節點只能有左子樹
							{
								return null;//直接return null
							}
							else if ((this.Action.Equals(Operator[6]) && this.Left != null))
							{
								return null;//直接return null
							}
							else if ((this.Action.Equals(Operator[7]) && this.Left != null))
							{
								return null;//直接return null
							}
							else if ((this.Action.Equals(Operator[8]) && this.Left != null))
							{
								return null;//直接return null
							}
							int a = rand.Next(1, Operator.Count + 1);//從operator隨機挑一個

							if (this.Left == null)//左子樹
							{
								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 2, Operator[a]);
								this.Left = child;
								//  this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
							else if (this.Right == null)//右子樹
							{
								node_id++;
								Node child = new Node(node_id, this, null, null, this.High + 1, 2, Operator[a]);
								this.Right = child;
								// this.Children.Add(child);
								tree_list.Add(child);
								return null;
							}
						}




					}


				}

				else//最後一層，不能再expand
				{
					return null;
				}

				return null;
			}


			public string rebuild_pre(Node root)
			{


				if (root != null)
				{
					count++;
					if (count > num_node)
					{
						count = 0;
						return preorder_path; 
					}

					Program.reb.Add(root);
					preorder_path += root.Action;
					rebuild_pre(root.Left);
					rebuild_pre(root.Right);




				}


				count = 0;
				return preorder_path;
			}




			public static void bst(List<Node> ln)
			{
				int i = 1;
				while (i <= Max_hight)
				{
					foreach (Node n in ln)
					{
						if (n.High == i)
						{
							Program.bst_list.Add(n);
						}
						i++;
					}
				}

			}


			public string PostOrder(Node root)
			{

				if (root != null)
				{
					count++;
					if (count > num_node)
					{
						count = 0;
						return postorder_path; 
					}

					PostOrder(root.Left);
					PostOrder(root.Right);
					postorder_path += root.Action;

				}
				count = 0;
				return postorder_path;
			}

			public string Cross_PostOrder_1(Node root)
			{
				if (root != null)
				{
					Cross_PostOrder_1(root.Left);
					Cross_PostOrder_1(root.Right);
					postorder_path += root.Action;
					Program.Cross_PostOrder_list_1.Add(root);
				}
				return postorder_path;
			}

			public string Cross_PostOrder_2(Node root)
			{
				if (root != null)
				{
					Cross_PostOrder_2(root.Left);
					Cross_PostOrder_2(root.Right);
					postorder_path += root.Action;
					Program.Cross_PostOrder_list_2.Add(root);
				}
				return postorder_path;
			}

			public string InOrder(Node root)
			{
				if (root != null)
				{
					InOrder(root.Left);
					inorder_path += root.Action;
					InOrder(root.Right);
				}
				return inorder_path;
			}




			public static void get_tree_list_act(string s)
			{
				if (dic_post_tree.ContainsKey(s))
				{
					foreach (Node i in dic_post_tree[s])
					{
						  Console.WriteLine(i.ID+"    "+i.Action);
					}
					Console.WriteLine();
				}
				else
				{
					//  Console.WriteLine("key error!");
				}

			}

			public static void renew_postorder(Node root)
			{
				root.postorder_path = "";

			}

			public static void renew_Cross_Postorder_1(Node root)
			{
				root.postorder_path = "";
				Program.Cross_PostOrder_list_1.Clear();
			}

			public static void renew_Cross_Postorder_2(Node root)
			{
				root.postorder_path = "";
				Program.Cross_PostOrder_list_2.Clear();
			}

			public static void renew_inorder(Node root)
			{
				root.inorder_path = "";
			}
		}

		public static void Encode()
		{
			

			Termianl.Add(1, "1");
			Termianl.Add(2, "2");
			Termianl.Add(3, "3");
			Termianl.Add(4, "4");
			Termianl.Add(5, "5");
			Termianl.Add(6, "6");
			Termianl.Add(7, "7");
			Termianl.Add(8, "8");
			Termianl.Add(9, "9");

			Termianl.Add(10, "a");//我方手下數量
			Termianl.Add(11, "b");//敵方手下數量
			Termianl.Add(12, "c");//我方手牌數量
			Termianl.Add(13, "d");//敵方手牌數量
			Termianl.Add(14, "e");//我方英雄血量
			Termianl.Add(15, "f");//敵方英雄血量
			Termianl.Add(16, "g");//我方牌庫數量
			Termianl.Add(17, "h");//敵方牌庫數量
			Termianl.Add(18, "i");//我方剩餘水晶數量

			Operator.Add(1, "A");// +
			Operator.Add(2, "B");// -
			Operator.Add(3, "C");// *
			Operator.Add(4, "D");// /

			Operator.Add(5, "E");// ^2
			Operator.Add(6, "F");// 開根號
			Operator.Add(7, "G");// log
			Operator.Add(8, "H");//Exp
		}

	}
}
