using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SGFParser : MonoBehaviour
{

    public List<TextAsset> sgfFiles;            //SGFファイル（txt形式）のリスト
    private List<List<int>> sgfIndexList;       //最後のログ出力用の変換後データ格納リスト
    private List<int> treeNodes;                //分岐が終わった or 無い手順のときに手順情報を保持するリスト
    private List<List<int>> branchIndexList;    //分岐があったときに一時的に手順情報を保持するリスト
    private List<int> treeBranchList;           //分岐があったときのtreeNodesより後の手順情報を保持するリスト
    private string sgfStr = "";                 //SGFファイルから読み込んだテキストデータ

    private bool isTree;                        //現在、分岐があるかどうかのフラグ　分岐あり -> true, 分岐無し -> false
    private int treeDepth = 0;                  //分岐の深さ

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
            //変数の初期化
            treeDepth = 0;
            isTree = false;
            treeNodes = new List<int>();
            treeBranchList = new List<int>();
            branchIndexList = new List<List<int>>();

            //変換に不要なMultiGoのプロパティを削除
            StringBuilder sb = new StringBuilder(sgf.text);
            sb.Replace("SZ[13]", "")
            .Replace("MULTIGOGM[1]", "")
            .Replace("AP[MultiGo:4.2.4]", "");

            //ファイルのテキストを行ごとのテキストデータにして配列に格納
            sgfStr = sb.ToString();
            string[] sgfLines = sgfStr.Split('\n');

            //行ごとにループ
            for (int n = 0; n < sgfLines.Length; n++)
            {
                if (sgfLines[n].Length < 7) continue;
                if (n == 0 || n == 1) continue;
                
                Debug.Log("文字列(1行): " + sgfLines[n]);
                Debug.Log("ツリーの深さ: " + treeDepth);

                //1行に分岐終了の括弧がいくつあるか
                int endCount = CountChar(sgfLines[n], ')');

                //分岐がある場合
                if (endCount == 0)
                {
                    //分岐の深さを1加算
                    treeDepth++;
                    if (treeDepth >= 1) isTree = true;
                    
                    //座標データごとに分割して配列に格納
                    string[] nodeArray = sgfLines[n].Split(';');
                    foreach (string node in nodeArray)
                    {
                        //座標データを囲む大括弧がどの位置からはじまってどの位置で終わるか
                        int open = node.IndexOf("[");
                        int close = node.IndexOf("]");
                        
                        //座標データじゃなければスキップ
                        if (open < 0 || close < 0)
                        {
                            continue;
                        }
                        //座標データである場合
                        else
                        {
                            Debug.Log("座標文字: " + node);
                            string value = node.Substring(open + 1, 2);
                            string key = node.Substring(0, open);

                            //手順データ(B or W)である場合のみ処理を行う
                            if (key == "B" || key == "W")
                            {
                                if (isTree)
                                {
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
                        if (branchIndexList.Count == treeDepth)
                        {
                            branchIndexList.Add(treeBranchList);

                            for (int i = 0; i < treeDepth; i++)
                            {
                                foreach (int bIndex in branchIndexList[i])
                                {
                                    treeNodes.Add(bIndex);
                                }
                            }
                        }
                    }
                    else
                    {
                        sgfIndexList.Add(treeNodes);
                        treeNodes = new List<int>();
                        treeBranchList = new List<int>();
                        branchIndexList = new List<List<int>>();
                    }
                }
                //分岐がある場合かつ、現在の分岐の最後である場合
                else if (endCount >= 2)
                {
                    //括弧が2つの場合
                    if (endCount == 2)
                    {
                        treeDepth--;
                    }
                    //括弧が3つ以上の場合
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
                                if (isTree)
                                {
                                    treeBranchList.Add(ConvertZahyoStringToInt(value));
                                }
                                else
                                {
                                    treeNodes.Add(ConvertZahyoStringToInt(value));
                                }
                            }
                        }
                    }

                    if (treeDepth <= 0) isTree = false;
                    if ((sgfLines.Length - 1) == n) isTree = false;

                    if (isTree)
                    {
                        if (branchIndexList.Count == treeDepth)
                        {
                            branchIndexList.Add(treeBranchList);

                            for (int i = 0; i < treeDepth; i++)
                            {
                                foreach (int bIndex in branchIndexList[i])
                                {
                                    treeNodes.Add(bIndex);
                                }
                            }
                        }
                    }
                    else
                    {
                        bool isLast = (sgfLines.Length - 1) == n;

                        //ファイルの最終行のデータだった場合
                        if (isLast)
                        {
                            if (branchIndexList.Count == treeDepth)
                            {
                                branchIndexList.Add(treeBranchList);

                                for (int i = 0; i < treeDepth; i++)
                                {
                                    foreach (int bIndex in branchIndexList[i])
                                    {
                                        treeNodes.Add(bIndex);
                                    }
                                }
                            }
                        }

                        sgfIndexList.Add(treeNodes);
                        treeNodes = new List<int>();
                        treeBranchList = new List<int>();
                        branchIndexList = new List<List<int>>();
                    }
                }
            }

            //変換結果をログ出力
            foreach (List<int> indexList in sgfIndexList)
            {
                string result = string.Join(",", indexList.Select(x => x.ToString()).ToArray());
                Debug.Log(result);
            }
        }
    }

    //SGFの文字座標を数字座標に変換
    private int ConvertZahyoStringToInt(string zahyo)
    {
        int boardSize = 13; //碁盤のサイズ

        int zahyo1 = CharToInt(zahyo[0]);
        int zahyo2 = CharToInt(zahyo[1]);
        int index = zahyo1 + (zahyo2 * boardSize);

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
