using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class MahjongButton : MonoBehaviour
{
    public static string[] MahjongType = { "1萬", "2萬", "3萬", "4萬", "5萬", "6萬", "7萬", "8萬", "9萬",
                                          "1筒", "2筒", "3筒", "4筒", "5筒", "6筒", "7筒", "8筒", "9筒",
                                          "1條", "2條", "3條", "4條", "5條", "6條", "7條", "8條", "9條",
                                          "東風", "西風", "南風", "北風",  "紅中",  "發財",  "白板"};
    void Update()
    {
        //eat+cancel buttons
        if (MahjongMovement.eatPlayer==1)
        {
            MahjongCanvas.timestop = true;
            for(int i = 0; i < MahjongAlgorithm.eatList.Count;++i)
            {
                GameObject button = GameObject.Find("Action").transform.GetChild(i).gameObject;
                button.SetActive(true);
                button.GetComponentInChildren<Text>().text = $"吃： {MahjongType[MahjongAlgorithm.eatList[i]]} {MahjongType[MahjongAlgorithm.eatList[i]+1]} {MahjongType[MahjongAlgorithm.eatList[i]+2]}";
            }
            GameObject.Find("Action").transform.GetChild(5).gameObject.SetActive(true);
        }

        //pong+cancel button
        if (MahjongMovement.pongPlayer==1)
        {
            MahjongCanvas.timestop = true;
            GameObject button = GameObject.Find("Action").transform.GetChild(3).gameObject;
            button.SetActive(true);
            button.GetComponentInChildren<Text>().text = $"碰： {MahjongType[MahjongAlgorithm.pongNum]}";
            GameObject.Find("Action").transform.GetChild(5).gameObject.SetActive(true);
        }

        //hu+cancel button
        if (MahjongMovement.huPlayer==1)
        {
            MahjongCanvas.timestop = true;
            GameObject.Find("Action").transform.GetChild(4).gameObject.SetActive(true);
            GameObject.Find("Action").transform.GetChild(5).gameObject.SetActive(true);
        }

    }

    public static void ClearButton()
    {
        for(int i = 0; i < 6; ++i) GameObject.Find("Action").transform.GetChild(i).gameObject.SetActive(false);
    }


    public void LoadingMenu()
    {
        
        using (StreamReader sr_1 = new StreamReader("./GameData/PlayerData.txt"))
        {
            string str = "";
            if (sr_1 != null)
            {
                do
                {
                    string player_data;
                    player_data = sr_1.ReadLine();
                    if (player_data == null)
                    {

                    }
                    string[] temp = player_data.Split(' ');
                    string temp_name = (temp[1].Split(',')[0]).ToString();
                    if (temp_name == SceneController.playerName)
                    {
                        str = temp[3];
                        break;
                    }
                } while (true);
            }
            sr_1.Close();
            Debug.Log("playername:" + SceneController.playerName + ",loadmoney:" + MahjongSound.loadMoney);
            string text = File.ReadAllText("./GameData/PlayerData.txt");
            Debug.Log(text);
            text = text.Replace($"Name: {SceneController.playerName}, Money: {str}", $"Name: {SceneController.playerName}, Money: {MahjongSound.loadMoney}");
            Debug.Log(text);
            SceneController.str1 = $"{MahjongSound.loadMoney}";
            /*
            using (StreamWriter sw_1 = new StreamWriter("./GameData/PlayerData.txt"))
            {
                //sw_1.Write(text);
                sw_1.Close();
            }
            */

            File.WriteAllText("./GameData/PlayerData.txt", text);
        }
        SceneManager.LoadScene("Menu_2");
    }
}
