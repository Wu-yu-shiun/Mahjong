using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;



public class MahjongSound : MonoBehaviour
{
    public AudioClip incidentSound;
    public AudioClip winSound;
    AudioSource au;


    float gameoverTime;

    public static int loadMoney = 0;

    void Start()
    {
        au = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (MahjongMovement.incidentSound)
        {
            au.PlayOneShot(incidentSound, 10f);
            MahjongMovement.incidentSound = false;
        }

        if (MahjongMovement.player1End == "win" || MahjongMovement.player1End == "winall")
        {
            au.PlayOneShot(winSound);
        }

        if (MahjongMovement.gameover)
        {
            gameoverTime = Time.time;
            MahjongMovement.gameover = false;

        }

        if (Time.time > gameoverTime + 3 && MahjongMovement.player1End != "")
        {
            if (MahjongMovement.player1End == "lose")
            {
                loadMoney -= 50;
            }
            else if (MahjongMovement.player1End == "win")
            {
                loadMoney += 50;
            }
            else if (MahjongMovement.player1End == "winall")
            {
                loadMoney += 150;
            }
            /*
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
                string text = File.ReadAllText("./GameData/PlayerData.txt");
                text = text.Replace($"Name: {SceneController.playerName}, Money: {str}", $"Name: {SceneController.playerName}, Money: {loadMoney}");
                //File.WriteAllText("./GameData/PlayerData.txt", text);

                using (StreamWriter sw_1 = new StreamWriter("./GameData/PlayerData.txt"))
                {
                    sw_1.Write(text);
                    sw_1.Close();
                }

            }
            */

            Debug.Log($"loadMoney:{loadMoney},playerend:{MahjongMovement.player1End}");


            SceneManager.LoadScene("loading");
            MahjongMovement.player1End = "";
        }
    }

    public void backMenu()
    {
        loadMoney -= 50;
        SceneManager.LoadScene("loading");
    }
 
    
}
