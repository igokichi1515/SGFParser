using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SGFParser : MonoBehaviour
{

    public List<TextAsset> sgfFiles;
    private List<List<int>> sgfIndexList;
    private List<int> treeNodes;
	private List<List<int>> branchIndexList;
	private List<int> treeBranchList;
	private string sgfStr = "";

	private bool isTree;
	private int treeDepth = 0;

    void Start()
    {
		sgfStr = "";
		treeDepth = 0;
        sgfIndexList = new List<List<int>>();
		branchIndexList = new List<List<int>>();

		//SGF解析スタート！
        ParseSGF();
    }

    private void ParseSGF()
    {
        foreach (TextAsset sgf in sgfFiles)
        {
			treeDepth = 0;
			isTree = false;
			treeNodes = new List<int>();
			treeBranchList = new List<int>();
			branchIndexList = new List<List<int>>();

            StringBuilder sb = new StringBuilder(sgf.text);
            sb.Replace("SZ[13]", "")
            .Replace("MULTIGOGM[1]", "")
            .Replace("AP[MultiGo:4.2.4]", "");

            sgfStr = sb.ToString();
            string[] sgfLines = sgfStr.Split('\n');

            for (int n = 0; n < sgfLines.Length; n++)
            {
                if (sgfLines[n].Length == 0) continue;
				if (n == 0 || n == 1) continue;
				Debug.Log("文字列(1行): " + sgfLines[n]);
				Debug.Log("ツリーの深さ: " + treeDepth);

                int endCount = CountChar(sgfLines[n], ')');
				//分岐がある場合
                if (endCount == 0)
                {
					treeDepth++;
					if (treeDepth >= 1) isTree = true;
					
                    string[] nodeArray = sgfLines[n].Split(';');
                    foreach (string node in nodeArray)
                    {
                        int open = node.IndexOf("[");
                        int close = node.IndexOf("]");

                        if (open < 0 || close < 0)
                        {
                            continue;
                        }
                        else
                        {
							Debug.Log("座標文字: " + node);
                        	string value = node.Substring(open + 1, 2);
                            string key = node.Substring(0, open);
                            if (key == "B" || key == "W")
                            {
								if (isTree)
								{
									Debug.Log("終わり括弧が0つでtreeBranchListに " + ConvertZahyoStringToInt(value) + " をAddした！");
									treeBranchList.Add(ConvertZahyoStringToInt(value));
								}
								else
								{
									treeNodes.Add(ConvertZahyoStringToInt(value));
								}
                            }
                        }
                    }

					if (isTree)
					{
						branchIndexList.Add(treeBranchList);
						Debug.Log("branchIndexList要素数: " + branchIndexList.Count);
					}
                }
				//分岐がない場合
                else if (endCount == 1)
                {
                    string[] nodeArray = sgfLines[n].Split(';');
                    foreach (string node in nodeArray)
                    {
                        int open = node.IndexOf("[");
                        int close = node.IndexOf("]");

                        if (open < 0 || close < 0)
                        {
                            continue;
                        }
                        else
                        {
							Debug.Log("座標文字: " + node);
							string value = node.Substring(open + 1, 2);
                            string key = node.Substring(0, open);
                            if (key == "B" || key == "W")
                            {
								if (isTree)
								{
									Debug.Log("終わり括弧が1つでtreeBranchListに " + ConvertZahyoStringToInt(value) + " をAddした！");
									treeBranchList.Add(ConvertZahyoStringToInt(value));
								}
								else
								{
									treeNodes.Add(ConvertZahyoStringToInt(value));
								}
                            }
                        }
                    }

					if (isTree)
					{
						branchIndexList.Add(treeBranchList);
						Debug.Log("branchIndexList要素数: " + branchIndexList.Count);

						for (int i = 0; i < treeDepth; i++)
						{
							foreach (int bIndex in branchIndexList[i])
							{
								Debug.Log("閉じ括弧が1つでtreeNodesに " + bIndex + " をAddした！");
								treeNodes.Add(bIndex);
							}
						}
					}
					else
					{
						string result = string.Join(",", treeNodes.Select(x => x.ToString()).ToArray());
						Debug.Log("sgfIndexListに " + result + " をAddした！");
						
						sgfIndexList.Add(treeNodes);
						treeNodes = new List<int>();
					}
                }
				//分岐がある場合かつ、現在の分岐の最後である場合
				else if (endCount >= 2)
				{
					//bool isLast = (sgfLines.Length - 1) == n;

					if (endCount == 2)
					{
						treeDepth--;
					}
					else if (endCount >= 3)
					{
						treeDepth -= (endCount - 1);
					}

			        string[] nodeArray = sgfLines[n].Split(';');
                    foreach (string node in nodeArray)
                    {
                        int open = node.IndexOf("[");
                        int close = node.IndexOf("]");

                        if (open < 0 || close < 0)
                        {
                            continue;
                        }
                        else
                        {
							Debug.Log("座標文字: " + node);
							string value = node.Substring(open + 1, 2);
                            string key = node.Substring(0, open);
                            if (key == "B" || key == "W")
                            {
								foreach (int index in treeNodes)
								{
									if (isTree)
									{
										Debug.Log("終わり括弧が2つ以上でtreeBranchListに " + ConvertZahyoStringToInt(value) + " をAddした！");
										treeBranchList.Add(ConvertZahyoStringToInt(value));
									}
									else
									{
										treeNodes.Add(ConvertZahyoStringToInt(value));
									}
								}
                            }
                        }
                    }

					if (isTree)
					{
						branchIndexList.Add(treeBranchList);
						Debug.Log("branchIndexList要素数: " + branchIndexList.Count);

						for (int i = 0; i < treeDepth; i++)
						{
							foreach (int bIndex in branchIndexList[i])
							{
								Debug.Log("閉じ括弧が2つ以上でtreeNodesに " + bIndex + " をAddした！");
								treeNodes.Add(bIndex);
							}
						}
					}
					else
					{
						string result = string.Join(",", treeNodes.Select(x => x.ToString()).ToArray());
						Debug.Log("sgfIndexListに " + result + " をAddした！");

						sgfIndexList.Add(treeNodes);
						treeNodes = new List<int>();
						treeBranchList = new List<int>();
					}

					if (treeDepth <= 0) isTree = false;
				}
            }

			foreach (List<int> indexList in sgfIndexList)
			{
				string result = string.Join(",", indexList.Select(x => x.ToString()).ToArray());
				Debug.Log(result.ToString());
			}
        }
    }

    //SGFの文字座標を数字座標に変換
    private int ConvertZahyoStringToInt(string zahyo)
    {
		int boardSize = 13; //碁盤のサイズ
		
		int zahyo1 = CharToInt(zahyo[0]);
		int zahyo2 = CharToInt(zahyo[1]);
		int index = (zahyo1 * boardSize) + zahyo2;
		
		return index;
    }

    //文字を数値に変換する関数
    private int CharToInt(char c)
    {
        int i = 0;

        switch (c)
        {
            case 'a': i = 0; break;
            case 'b': i = 1; break;
            case 'c': i = 2; break;
            case 'd': i = 3; break;
            case 'e': i = 4; break;
            case 'f': i = 5; break;
            case 'g': i = 6; break;
            case 'h': i = 7; break;
            case 'i': i = 8; break;
            case 'j': i = 9; break;
            case 'k': i = 10; break;
            case 'l': i = 11; break;
            case 'm': i = 12; break;
            case 'n': i = 13; break;
            case 'o': i = 14; break;
            case 'p': i = 15; break;
            case 'q': i = 16; break;
            case 'r': i = 17; break;
            case 's': i = 18; break;
            case 't': i = -1; break;
            default: i = -1; break;
        }

        return i;
    }

    //文字の出現回数をカウント
    private int CountChar(string s, char c)
    {
        return s.Length - s.Replace(c.ToString(), "").Length;
    }
}
