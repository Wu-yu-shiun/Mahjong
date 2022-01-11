using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MahjongCanvas : MonoBehaviour
{
    Text NameLabel, MoneyLabel,TimerLabel,Message;
    Image TimerImage;
    Image[] PlayerStateImage=new Image[4];
    StreamReader sr_1;

    public static int natureChange,DiceNumber;
    public static float player1TimeCount,player234TimeCount;
    public static bool[] playerTurn= new bool[4];
    public static bool timestop;

    int firstPlayer, round, allChange, lastplayer;
    float timepast;
    bool checkEnd, diceInvisible, startgame;

    void Start()
    {
        sr_1 = new StreamReader("./GameData/PlayerData.txt");
        TimerImage = GameObject.Find("TimerImage").GetComponent<Image>();
        TimerLabel = GameObject.Find("TimerCountdown").GetComponent<Text>();
        NameLabel = GameObject.Find("Name").GetComponent<Text>();
        MoneyLabel = GameObject.Find("Money").GetComponent<Text>();
        Message= GameObject.Find("Message").GetComponent<Text>();

        //init
        player1TimeCount = 10; player234TimeCount = 3; round = 1;
        timepast = 0; DiceNumber = 0; natureChange = 0; allChange = 0;
        diceInvisible = false; startgame = false; timestop = false;
        Message.text += $"Press [Space] to roll dices...";
        Message.color = Color.white;
        MahjongButton.ClearButton();
        NameLabel.text = $"{SceneController.playerName}";

        do
        {
            string player_data;
            player_data = sr_1.ReadLine();
            if (player_data == null)
            {
                break;
            }
            string[] temp = player_data.Split(' ');
            string temp_name = (temp[1].Split(',')[0]).ToString(); 

            if (temp_name == SceneController.playerName)
            {
                MoneyLabel.text = $"{int.Parse(temp[3])}";
                break;
            }
        } while (true);

    }

    void Update()
    {
        //check dice number then start
        checkEnd = DiceSensor.checkEnd;
        if (!checkEnd) return;
        if (checkEnd && !diceInvisible)
        {
            firstPlayer = DiceNumber % 4;
            if (firstPlayer == 0) firstPlayer = 4;
            Message.text += $"{DiceNumber}\n";
            Message.text += $"Player{firstPlayer} first!\nGame Start!!!";
            GameObject.Find("Dice").SetActive(false);
            diceInvisible = true;
        }
        if (diceInvisible && !startgame)
        {
            timepast += Time.deltaTime;
            if (timepast > 2)
            {
                startgame = true;
                Message.text = "Round1\n";
                ChangePlayerTurn(firstPlayer);
                natureChange++;
                allChange++;
            }
        }

        //if player1234's turn
        ExecutePlayer1();
        ExecutePlayer234(2);
        ExecutePlayer234(3);
        ExecutePlayer234(4);


        //if player1 choose cancel
        if (MahjongMovement.chooseCancel)
        {
            Debug.Log("Nothing happened! Change player!");
            natureChange++;
            if (lastplayer == 4) ChangePlayerTurn(1);
            else ChangePlayerTurn(lastplayer + 1);

            MahjongMovement.chooseCancel = false;
        }

        //if player1 choose eat/pong
        if (MahjongMovement.chooseAction)
        {
            ChangePlayerTurn(1);
            MahjongMovement.chooseAction = false;
        }
    }

    //1.playerturn bool->true
    //2.playerturn image color->gray
    //3.message->player's turn
    //4.allChange++
    void ChangePlayerTurn(int player)
    {
        allChange++;
        for (int i = 0; i < 4; ++i)
        {
            PlayerStateImage[i] = GameObject.Find($"Player{i+1}_State").GetComponent<Image>();
            if (i == player-1)
            {
                playerTurn[i] = true;
                PlayerStateImage[i].color = Color.gray;
            }
            else
            {
                playerTurn[i] = false;
                PlayerStateImage[i].color = Color.white;
            }
        }

        //refresh message in new round
        if (allChange > 4)  
        {
            round++;
            allChange = 1;
            Message.text = $"Round{round}\n>>Player{player}'s Turn\n";
            Debug.Log("clean message");
        }
        else Message.text += $">>Player{player}'s Turn\n";
    }

    void ExecutePlayer1()
    {
        if (playerTurn[0])
        {
            if (player1TimeCount >= 0)
            {
                if(!timestop) player1TimeCount -= Time.deltaTime;
                TimerImage.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));
                TimerLabel.text = $"{(int)player1TimeCount}";
            }
            else
            {
                TimerLabel.text = $"10";
                if (player1TimeCount > -3)
                {
                    if (!timestop) player1TimeCount -= Time.deltaTime;
                    TimerImage.color = Color.white;
                }
                else
                {
                    player1TimeCount = 10;
                    CheckIncident(1);
                }
            } 
        }
    }

    void ExecutePlayer234(int player)
    {
        if (playerTurn[player-1] && !timestop)
        {
            if (player234TimeCount >= 0)
            {
                player234TimeCount -= Time.deltaTime;
            }
            else
            {
                player234TimeCount = 3;
                CheckIncident(player);
            }
        }
    }

    void CheckIncident(int player)
    {
        //check hu
        for(int i = 1; i <= 4; ++i)
        {
            if (i == player) continue;
            MahjongAlgorithm.Check_One_Hu(i, MahjongMovement.bags,MahjongMovement.currentMahjong);
            if (MahjongMovement.huPlayer == 1) lastplayer = player;
            if (MahjongMovement.huPlayer > 0) return;
        }
        //check pong
        MahjongAlgorithm.Check_All_Pong(player, MahjongMovement.currentMahjong, MahjongMovement.bags);
        if (MahjongMovement.pongPlayer > 0)
        {
            if (MahjongMovement.pongPlayer == 1) lastplayer = player;
            else ChangePlayerTurn(MahjongMovement.pongPlayer);
            return;
        }

        //check eat
        if (player == 4) MahjongAlgorithm.Check_Next_Eat(1, MahjongMovement.currentMahjong, MahjongMovement.bags[0]);
        else MahjongAlgorithm.Check_Next_Eat(player + 1, MahjongMovement.currentMahjong, MahjongMovement.bags[player]);
        if (MahjongMovement.eatPlayer > 0)
        {
            if (MahjongMovement.eatPlayer == 1) lastplayer = player;
            else ChangePlayerTurn(MahjongMovement.eatPlayer);
            return;
        }

        //nothing
        Debug.Log("Nothing happened! Change player!");
        natureChange++;
        if (player == 4) ChangePlayerTurn(1);
        else ChangePlayerTurn(player + 1);
    }
}
