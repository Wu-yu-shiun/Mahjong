using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MahjongAlgorithm : MonoBehaviour
{

    public static int eatNum,pongNum;
    public static List<int> eatList = new List<int>();

    //下個人做
    public static void Check_Next_Eat(int playerToCheck, GameObject currentMahjong, List<GameObject> bag)
    {
        //Debug.Log($"Algorithm check player{playerToCheck} eat...");
        int currentNum = int.Parse(currentMahjong.name.Split('.')[0]);
        List<int> temp = new List<int>();
        for (int i = 0; i < bag.Count; ++i) temp.Add(int.Parse(bag[i].name.Split('.')[0]));

        //currentNum能出現的位置
        bool right = true, left = true, center = true; 
        if (currentNum == 0 || currentNum == 9 || currentNum == 18)
        {
            left = true;
            right = false;
            center = false;
        }
        else if (currentNum == 1 || currentNum == 10 || currentNum == 19)
        {
            left = true;
            right = false;
            center = true;
        } 
        else if (currentNum == 7 || currentNum == 16 || currentNum == 25)
        {
            left = false;
            right = true;
            center = true;
        }
        else if (currentNum == 8 || currentNum == 17 || currentNum == 26)
        {
            left = false;
            right = true;
            center = false;
        }
        else if(currentNum>26)
        {
            left = false;
            right = false;
            center = false;
        }

        //right
        if (right && temp.Contains(currentNum - 1) && temp.Contains(currentNum - 2))
        {
            Debug.Log($"Find eat! (player={playerToCheck},Head number={currentNum - 2})");
            MahjongMovement.eatPlayer = playerToCheck;
            if (playerToCheck == 1) eatList.Add(currentNum - 2);
            else
            {
                eatNum = currentNum - 2;
                return;
            }
        }
        //center
        if (center && temp.Contains(currentNum - 1) && temp.Contains(currentNum + 1))
        {
            Debug.Log($"Find eat! (player={playerToCheck},Head number={currentNum - 1})");
            MahjongMovement.eatPlayer = playerToCheck;
            if (playerToCheck == 1) eatList.Add(currentNum - 1);
            else
            {
                eatNum= currentNum - 1;
                return;
            }
        }
        //left
        if (left && temp.Contains(currentNum + 1) && temp.Contains(currentNum + 2))
        {
            Debug.Log($"Find eat! (player={playerToCheck},Head number={currentNum})");
            MahjongMovement.eatPlayer = playerToCheck;
            if (playerToCheck == 1) eatList.Add(currentNum);
            else
            {
                eatNum = currentNum;
                return;
            }
        }

        if (eatList.Count>0)
        {
            string str=$"currentMahjong:{currentMahjong},eatlist:";
            for (int i = 0; i < eatList.Count;i++) str += $"{eatList[i]} ";
            Debug.Log(str);
        }
        
    }

    //每人每局都要做
    public static void Check_All_Pong(int currentPlayer, GameObject currentMahjong, List<List<GameObject>> bags)
    {
        //Debug.Log($"Algorithm pong...");
        int currentNum = int.Parse(currentMahjong.name.Split('.')[0]);
        for (int p = 1; p <= bags.Count; ++p)
        {
            if (p == currentPlayer) continue;
            else
            {
                List<int> temp = new List<int>();
                for (int i = 0; i < bags[p - 1].Count; ++i) temp.Add(int.Parse(bags[p - 1][i].name.Split('.')[0]));

                for (int i = 0; i < temp.Count - 1; ++i)
                {
                    if (currentNum==temp[i] && currentNum == temp[i + 1])
                    {
                        pongNum = currentNum;
                        MahjongMovement.pongPlayer = p;
                        Debug.Log($"Find p{p} pong!");
                        return;
                    }
                }
            }
        }
    }


    public static void Check_One_Hu(int player, List<List<GameObject>> bags,GameObject currentMahjong)
    {
        //Debug.Log($"Algorithm hu...");
        //get baglist(1~9,11~19,21~29,31~37)
        List<int> tempList = new List<int>();
        for (int i = 0; i < bags[player-1].Count; ++i) tempList.Add(int.Parse(bags[player-1][i].name.Split('.')[0]));
        tempList.Add(int.Parse(currentMahjong.name.Split('.')[0]));
        tempList.Sort();
        List<int> bagList = new List<int>();
        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i] < 9) bagList.Add(tempList[i] + 1);
            else if (tempList[i] < 18) bagList.Add(tempList[i] + 2);
            else if (tempList[i] < 27) bagList.Add(tempList[i] + 3);
            else bagList.Add(tempList[i] + 4);
        }

        string str = $"check player{player} hu baglist: ";
        for(int i = 0; i < bagList.Count; ++i)
        {
            str += $"{bagList[i]} ";
        }
        Debug.Log(str);

        //找出最後一對可能解
        List<int> pairList = new List<int>();
        for (int i = 1; i <= 37; ++i)
        {
            if (bagList.FindAll(x => x == i).Count >= 2)
            {
                pairList.Add(i);
            }
        }
        //剩下3n張找解
        for (int i = 0; i < pairList.Count; ++i)
        {
            List<int> checkList = new List<int>();
            for (int j = 0; j < bagList.Count; ++j) checkList.Add(bagList[j]);
            checkList.Remove(pairList[i]);
            checkList.Remove(pairList[i]);
            FindSolution(checkList);            
            if (MahjongMovement.huPlayer != 0)
            {
                MahjongMovement.huPlayer = player;
                Debug.Log($"find {player} hu");
                break;
            }
        }
    }

    public static void FindSolution(List<int> checkList)
    {
        if (checkList.Count==0)
        {
            MahjongMovement.huPlayer = -1;
            return;
        }
        else
        {
            if (checkList.FindAll(x => x == checkList[0]).Count <= 2)
            {
                if (checkList.Contains(checkList[0] + 1) && checkList.Contains(checkList[0] + 2))
                {
                    int[] temp = { checkList[0], checkList[0] + 1, checkList[0] + 2 };
                    for (int i = 0; i < 3; ++i) checkList.Remove(temp[i]);
                    FindSolution(checkList);
                }
                else return;

            }
            else if (checkList[0] == checkList[1] && checkList[0] == checkList[2])
            {
                for (int i = 0; i < 3; ++i) checkList.Remove(checkList[0]);
                FindSolution(checkList);
            }
            else return;
        }
    }
    
}
