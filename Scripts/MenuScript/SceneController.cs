using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class SceneController : MonoBehaviour, IPointerClickHandler
{
    string path;
    Dropdown dropdownitem;
    GameObject drop_temp;
    Text NameLabel, MoneyLabel;

    int playerMoney=0;
    public static string playerName;
    bool checkActive=false;
    float activetime;
    public static string str1="";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"{SceneManager.GetActiveScene().buildIndex}: {str1}");
        //Debug.Log("scene controller");
        
        //SceneManager.LoadScene("Menu");
        if (GameObject.Find("Name") != null && GameObject.Find("Money")!=null)
        {
            NameLabel = GameObject.Find("Name").GetComponent<Text>();
            MoneyLabel = GameObject.Find("Money").GetComponent<Text>();
            
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            NameLabel.text = playerName;
            MoneyLabel.text = str1;

        }

        if (SceneManager.GetActiveScene().buildIndex == 1) GameObject.Find("Canvas/btn_Start/noMoney").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (GameObject.Find("Canvas/btn_Start/noMoney").activeSelf && !checkActive)
            {
                activetime = Time.time;
                checkActive = true;
            }

            if (checkActive && Time.time > activetime + 2)
            {
                GameObject.Find("Canvas/btn_Start/noMoney").SetActive(false);
                checkActive = false;
            }
        }
        
    }

    public void OnPointerClick(PointerEventData e)
    {
        Scene scene = SceneManager.GetActiveScene();

        if(scene.buildIndex == 0)
        {
            if(this.name == "btn_Start")
            {
                drop_temp = GameObject.Find("Dropdown");
                dropdownitem = drop_temp.GetComponent<Dropdown>();
                string player_name = dropdownitem.captionText.text;
                //Debug.Log(player_name);
                playerName = player_name;
                //Menu_Info.Load_Player_info(player_name);

                //NameLabel.text = playerName;
                path = "./GameData/PlayerData.txt";

                using (StreamReader sr_1 = new StreamReader(path))
                {
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
                        if (temp_name == playerName)
                        {
                            str1 = $"{int.Parse(temp[3])}";
                            //MoneyLabel.text = $"{int.Parse(temp[3])}";
                            break;
                        }
                    } while (true);
                    sr_1.Close();
                    Debug.Log("str1:" + str1);
                }
                SceneManager.LoadScene("Menu_2");
            }

            else if(this.name == "btn_Exit")
            {
                Application.Quit();
            }
            
        }
        else if(scene.buildIndex == 1)
        {
            if (this.name == "btn_Exit")
            {
                Application.Quit();
            }
            else if(this.name == "btn_Start")
            {

                using (StreamReader sr_2 = new StreamReader("./GameData/PlayerData.txt"))
                {
                    string str = "";
                    do
                    {
                        string player_data;
                        player_data = sr_2.ReadLine();
                        if (player_data == null)
                        {

                        }
                        string[] temp = player_data.Split(' ');
                        string temp_name = (temp[1].Split(',')[0]).ToString();
                        if (temp_name == playerName)
                        {
                            str = temp[3];
                            break;
                        }
                    } while (true);
                    MahjongSound.loadMoney = int.Parse(str);
                    sr_2.Close();
                }
                    
               
                Debug.Log("loadmoney" + MahjongSound.loadMoney);

                if (MahjongSound.loadMoney >= 50)
                {
                    SceneManager.LoadScene("SampleScene");
                }
                else
                {
                    Debug.Log("not enough money");
                    GameObject.Find("Canvas/btn_Start/noMoney").SetActive(true);
                    GameObject.Find("Canvas/btn_Start/noMoney").GetComponent<Text>().color = Color.red;
                }
                

            }
            else if(this.name == "btn_Minigame")
            {
                SceneManager.LoadScene("menu");

            }
            else if(this.name == "btn_ChangePlayer")
            {
                SceneManager.LoadScene("PlayerMenu");
            }
        }
        else if (scene.buildIndex == 2)
        {
            if (this.name == "MenuButton")
            {
                
            }
        }
        else if (scene.buildIndex == 3)
        {
            if (this.name == "MainMenuButton")
            {
                SceneManager.LoadScene("Menu_2");
                Destroy(GameObject.Find("CanvasMon"));
            }
        }

    }


    
}
