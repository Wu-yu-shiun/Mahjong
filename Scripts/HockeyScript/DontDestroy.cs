using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy Instance { get; set; }//單例
    public static int money = 0;
    FileInfo file;
    StreamReader sr;
    string path,player_data;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        path = "./GameData/PlayerData.txt";
        sr = new StreamReader(path);
        do
        {
            string player_data;
            player_data = sr.ReadLine();
            if (player_data == null)                     //讀完資料庫
            {
                break;
            }
            string[] temp = player_data.Split(' ');
            string temp_name = (temp[1].Split(',')[0]).ToString();          //name
            if (temp_name == SceneController.playerName)
            {
                money = changeMONEY(int.Parse(temp[3]));
                GameObject.Find("MoneyTx").GetComponent<Text>().text = $"Money:{money}";
                break;
            }

        } while (true);
        sr.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int changeMONEY(int MONEY)
    {
        Debug.Log(MONEY);
        return MONEY;
    }
    
}
