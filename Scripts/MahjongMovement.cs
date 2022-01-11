using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MahjongMovement : MonoBehaviour
{
    public GameObject[] Mahjongs = new GameObject[34];
    public static GameObject space,currentMahjong;
    public static List<List<GameObject>> bags;
    List<GameObject> bag1,bag2,bag3,bag4,box;
    Text Message;
    public static int[] fixedNum;
    public static bool useray,chooseCancel,chooseAction,gameover,incidentSound;
    public static int eatPlayer, pongPlayer, huPlayer; 
    int centerNum,natureChange;

    public static string player1End; // tie,lose,win,winall

    void Start()
    {
        //init
        bags = new List<List<GameObject>>();
        bag1 = new List<GameObject>(); bags.Add(bag1);
        bag2 = new List<GameObject>(); bags.Add(bag2);
        bag3 = new List<GameObject>(); bags.Add(bag3);
        bag4 = new List<GameObject>(); bags.Add(bag4);
        box = new List<GameObject>();
       
        fixedNum = new int[4];
        for (int i = 0; i < 4; ++i) fixedNum[i] = 0;
        useray = true; chooseCancel = false; chooseAction = false; gameover = false; incidentSound = false;
        centerNum = 0; natureChange=0; eatPlayer = 0; pongPlayer=0; huPlayer=0;
        Message = GameObject.Find("Message").GetComponent<Text>();
        player1End = "";

        //clean table
        for (int c = 1; c <= 5; ++c)
        {
            for (int i = 1; i <= 15; ++i)
            {
                if (c == 5 && i > 12) break;
                space = GameObject.Find($"C{c}-{i}");
                space.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        space = GameObject.Find($"P1-17");
        space.transform.GetChild(0).gameObject.SetActive(false);

        //create all mahjong
        for(int i = 0; i < 34; ++i)
        {
            for(int j=0;j<4; ++j)
            {
                space = GameObject.Find($"unused");
                GameObject temp = Instantiate(Mahjongs[i]);
                temp.transform.parent = space.transform;
                temp.transform.position = space.transform.position;
                temp.name = $"{i}.{j}";
                box.Add(temp);
            }
        }

        //distribute mahjongs
        for (int p = 1; p <= 4; ++p)
        {
            for (int i = 1; i <= 16; ++i)
            {
                GameObject temp = GetOneInUnused();
                bags[p - 1].Add(temp);
                space = GameObject.Find($"P{p}-{i}");
                space.transform.GetChild(0).gameObject.SetActive(false);
                temp.transform.parent = space.transform;
                temp.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90 * p) % 360, 0));
            }
        }
        for (int i = 1; i <= 4; ++i) Arrange(i);

    }

    void Update()
    {
        //檢查是否結束
        if (box.Count == 0)
        {
            Debug.Log("Tie! No more mahjong!");
            player1End = "tie";
        }

        //檢查p234是否吃碰胡
        if (huPlayer > 1)
        {
            HuMove(huPlayer);
            player1End = "lose";
        }
        if (eatPlayer>1) EatMove(eatPlayer);
        if (pongPlayer>1) PongMove(pongPlayer);

        //若自然換人發新麻將(p1擺到P1-17/p234直接出牌)
        if (natureChange != MahjongCanvas.natureChange)
        {
            for (int p = 1; p <= 4; ++p)
            {
                if (MahjongCanvas.playerTurn[p-1])
                {
                    GameObject temp = GetOneInUnused();
                     if (p == 1)
                    {
                        space = GameObject.Find($"P1-17");
                        temp.transform.parent = space.transform;
                        temp.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, 180, 0));
                        MahjongAlgorithm.Check_One_Hu(p, bags, temp);
                        bag1.Add(temp);
                    }
                    else
                    {
                        bags[p - 1].Add(temp);
                        TakeToCenterMove(p, ChooseOneInBag(bags[p - 1]));
                        Arrange(p);
                        for (int i = 0; i < bags[p - 1].Count; ++i)
                        {
                            GameObject moveMahjong = bags[p - 1][i];
                            space = GameObject.Find($"P{p}-{fixedNum[p - 1] + i + 1}");
                            moveMahjong.transform.parent = space.transform;
                            moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90 * p) % 360, 0));
                        }
                    }
                }
            }
        }
        natureChange = MahjongCanvas.natureChange;

        //檢查p1是否需出牌
        if (GameObject.Find("P1-17").transform.childCount > 1 && !MahjongCanvas.timestop) useray = false;
        if (Input.GetMouseButtonDown(0) && !useray && GameObject.Find("P1-17").transform.GetChild(1).transform.rotation.eulerAngles.x!=270)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rch;
            if (Physics.Raycast(ray, out rch))
            {
                if(rch.transform.parent!=null && rch.transform.parent.parent != null)
                {
                    if (rch.transform.parent.parent.transform.name == "player1")
                    {
                        useray = true;
                        int index = rch.transform.parent.GetSiblingIndex();
                        TakeToCenterMove(1, bag1[index - fixedNum[0]]);
                        Arrange(1);
                        MahjongCanvas.player1TimeCount = -1;
                    }
                }
            }
        }
        if (MahjongCanvas.player1TimeCount < 0 && !useray)
        {
            useray = true;
            TakeToCenterMove(1, bag1[bag1.Count-1]);
            Arrange(1);
        }
    }

    //functions
    GameObject GetOneInUnused()
    {
        int random = Random.Range(0, box.Count);
        GameObject temp = box[random];
        box.RemoveAt(random);
        return temp;
    }

    void Arrange(int player)
    {
        bags[player - 1].Sort((x, y) => int.Parse(x.name.Split('.')[0]).CompareTo(int.Parse(y.name.Split('.')[0])));
        for (int i = 0; i < 16 - fixedNum[player - 1]; ++i)
        {
            space = GameObject.Find($"P{player}-{i + 1 + fixedNum[player - 1]}");
            GameObject temp = bags[player - 1][i];
            temp.transform.parent = space.transform;
            temp.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90 * player) % 360, 0));
        }
        //print bag item

        string bagItem = $"{bags[player - 1].Count} in player{player}'s bag: ";
        foreach (var item in bags[player - 1])
        {
            bagItem += $"{item.name.Split('.')[0]} ";
        }
        Debug.Log(bagItem);

    }

    public void TakeToCenterMove(int player, GameObject moveMahjong)
    {
        currentMahjong = moveMahjong;
        int a = (centerNum + 1) / 15 + 1;
        int b = (centerNum + 1) % 15;
        if (b == 0)
        {
            a -= 1;
            b = 15;
        }
        space = GameObject.Find($"C{a}-{b}");
        moveMahjong.transform.parent = space.transform;
        moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(270, 180, 0));
        centerNum++;
        bags[player - 1].Remove(moveMahjong);
        Debug.Log($"player{player} move {moveMahjong.name} to center.");

        Message.text+=$"player{player} 出 [{MahjongButton.MahjongType[int.Parse(moveMahjong.name.Split('.')[0])]}]\n";
    }

    public void TakeFromCenterMove(int player)
    {
        space = GameObject.Find("unused"); //暫時先將其放回收區 等出完牌取回
        currentMahjong.transform.parent = space.transform.parent;
        currentMahjong.transform.position = space.transform.GetChild(0).position;
        centerNum--;
        bags[player - 1].Add(currentMahjong);
        Debug.Log($"player{player} take {currentMahjong} from center.");
    }

    public GameObject ChooseOneInBag(List<GameObject> bag)
    {
        for (int i = 27; i <= 33; i++)
        {
            if(bag.FindAll(x => int.Parse(x.name.Split('.')[0]) == i).Count == 1)
            {
                return bag.Find(x => int.Parse(x.name.Split('.')[0]) == i);
            }
        }
        return bag[bag.Count-1];
    }

    //eat
    public void EatMove(int player)
    {
        incidentSound = true;
        if (player < 0)
        {
            Message.text += $"player1 吃!\n";
            TakeFromCenterMove(1);
            for (int i = 0; i < 3; ++i)
            {
                GameObject moveMahjong = bag1.Find(x => int.Parse(x.name.Split('.')[0]) == MahjongAlgorithm.eatList[(player+1)*-1] + i);
                space = GameObject.Find($"P1-{fixedNum[0] + i + 1}");
                moveMahjong.transform.parent = space.transform;
                moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(270, (630 - 90) % 360, 0));
                bag1.Remove(moveMahjong);
            }
            fixedNum[0] += 3;
            Arrange(1);
            for (int i = 0; i < bag1.Count; ++i)
            {
                GameObject moveMahjong = bag1[i];
                space = GameObject.Find($"P1-{fixedNum[0] + i + 1}");
                moveMahjong.transform.parent = space.transform;
                moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90) % 360, 0));
            }
            MahjongCanvas.timestop = false;
            MahjongAlgorithm.eatList.Clear();
            chooseAction = true;
            MahjongButton.ClearButton();
        }
        else
        {
            Message.text += $"player{player} 吃!\n";
            TakeFromCenterMove(player);
            for (int i = 0; i < 3; ++i)
            {
                GameObject moveMahjong = bags[player - 1].Find(x => int.Parse(x.name.Split('.')[0]) == MahjongAlgorithm.eatNum+i);
                space = GameObject.Find($"P{player}-{fixedNum[player - 1] + i + 1}");
                moveMahjong.transform.parent = space.transform;
                moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(270, (630 - 90 * player) % 360, 0));
                bags[player - 1].Remove(moveMahjong);
            }
            fixedNum[player - 1] += 3;
            TakeToCenterMove(player, ChooseOneInBag(bags[player - 1]));
            Arrange(player);
            for (int i = 0; i < bags[player - 1].Count; ++i)
            {
                GameObject moveMahjong = bags[player - 1][i];
                space = GameObject.Find($"P{player}-{fixedNum[player - 1] + i + 1}");
                moveMahjong.transform.parent = space.transform;
                moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90 * player) % 360, 0));
            }
        }
        eatPlayer = 0;
        //Debug.Log($"EatMove End.FixedNum-> player1:{fixedNum[0]} player2:{fixedNum[1]} player3:{fixedNum[2]} player4:{fixedNum[3]}");
    }

    //pong
    public void PongMove(int player)
    {
        incidentSound = true;
        Message.text += $"player{player} 碰!\n";
        TakeFromCenterMove(player);
        for (int i = 0; i < 3; ++i)
        {
            GameObject moveMahjong = bags[player - 1].Find(x => int.Parse(x.name.Split('.')[0]) == MahjongAlgorithm.pongNum);
            space = GameObject.Find($"P{player}-{fixedNum[player - 1] + i + 1}");
            moveMahjong.transform.parent = space.transform;
            moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(270, (630 - 90 * player) % 360, 0));
            bags[player - 1].Remove(moveMahjong);
        }
        fixedNum[player - 1] += 3;
        if(player!=1) TakeToCenterMove(player, ChooseOneInBag(bags[player - 1]));
        for (int i = 0; i < bags[player - 1].Count; ++i)
        {
            GameObject moveMahjong = bags[player - 1][i];
            space = GameObject.Find($"P{player}-{fixedNum[player - 1] + i + 1}");
            moveMahjong.transform.parent = space.transform;
            moveMahjong.transform.SetPositionAndRotation(space.transform.GetChild(0).position, Quaternion.Euler(0, (630 - 90 * player) % 360, 0));
        }
        if (player == 1)
        {
            MahjongCanvas.timestop = false;
            chooseAction = true;
            MahjongButton.ClearButton();  
        }
        Arrange(player);
        pongPlayer = 0;
        //Debug.Log($"PongMove End. FixedNum-> player1: {fixedNum[0]} player2: {fixedNum[1]} player3: {fixedNum[2]} player4: {fixedNum[3]}");
    }

    //hu
    public void HuMove(int player)
    {
        Message.text += $"player{player} 胡!遊戲結束！\n";
        Debug.Log($"{player} Win!");
        gameover = true;
        for (int i = 0; i < 16 - fixedNum[player - 1]; ++i)
        {
            GameObject temp = GameObject.Find($"P{player}-{i + 1 + fixedNum[player - 1]}").transform.GetChild(1).gameObject;
            temp.transform.rotation = Quaternion.Euler(270, (630 - 90 * player) % 360, 0);
        }
        if (huPlayer == 1)
        {
            if (GameObject.Find($"P1-17").transform.childCount > 1)
            {
                GameObject.Find($"P1-17").transform.GetChild(1).gameObject.transform.rotation = Quaternion.Euler(270, (630 - 90 * player) % 360, 0);
                player1End = "winall";
            }
            else player1End = "win";

            MahjongButton.ClearButton();
            //音效
        }
        else player1End = "lose";
        
        huPlayer = 0;
        
    }

    //cancel
    public void CancelMove()
    {
        eatPlayer = 0;
        pongPlayer = 0;
        huPlayer = 0;
        MahjongAlgorithm.eatList.Clear();
        MahjongCanvas.timestop = false;
        chooseCancel = true;
        MahjongButton.ClearButton();
        Debug.Log($"You choose cancel.");
    }

}
