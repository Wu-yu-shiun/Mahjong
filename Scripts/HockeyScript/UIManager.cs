using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class UIManager : MonoBehaviour
{

    [Header("Canvas")]
    public GameObject CanvasGame;
    public GameObject CanvasRestart;

    [Header("CanvasRestart")]
    public GameObject WinTxt;
    public GameObject LooseTxt;

    [Header("Other")]
    public AudioManager audioManager;

    public ScoreScript scoreScript;

    public PuckScript puckScript;
    public PlayerMovement playerMovement;
    public ComScript aiScript;


    private Scene scene;
    private MenuManager MenuManager;
    private DontDestroy DontDestroy;



    void Start()
    {
        scene = gameObject.scene;
        
    }

    public void ShowRestartCanvas(bool didAiWin)
    {
        Time.timeScale = 0;

        CanvasGame.SetActive(false);
        CanvasRestart.SetActive(true);

        if (didAiWin)
        {
            audioManager.PlayLostGame();
            WinTxt.SetActive(false);
            LooseTxt.SetActive(true);
        }
        else
        {
            switch (scene.name)
            {
                case "main":
                    DontDestroy.money += 10;
                    break;
                case "main1":
                    DontDestroy.money += 20;
                    break;
                case "main2":
                    break;
            }

            if (MenuManager.MaxMovementSpeed == 5)
                DontDestroy.money += 10;
            else if (MenuManager.MaxMovementSpeed <= 20)
                DontDestroy.money += 20;
            else if (MenuManager.MaxMovementSpeed <= 80)
                DontDestroy.money += 50;
            GameObject.Find("CanvasMon/MoneyTx").gameObject.GetComponent<Text>().text = "Money: " + DontDestroy.money.ToString();
            audioManager.PlayWonGame();
            WinTxt.SetActive(true);
            LooseTxt.SetActive(false);
        }

       
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        CanvasGame.SetActive(true);
        CanvasRestart.SetActive(false);

        scoreScript.ResetScores();
        puckScript.CenterPuck();
        playerMovement.ResetPosition();
        aiScript.ResetPosition();
    }

    public void reset_time()
    {
        Time.timeScale = 0;
        BacktoMenu();
    }

    public void BacktoMenu()
    {
        using (StreamReader sr_1 = new StreamReader("./GameData/PlayerData.txt"))
        {
            string str = "";
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
            sr_1.Close();

            //sr_1 = File.OpenText("./GameData/PlayerData.txt");
            string text = File.ReadAllText("./GameData/PlayerData.txt");
            text = text.Replace($"Name: {SceneController.playerName}, Money: {str}", $"Name: {SceneController.playerName}, Money: {DontDestroy.money}");
            File.WriteAllText("./GameData/PlayerData.txt", text);
            SceneController.str1 = $"{DontDestroy.money}";
            //using (StreamWriter sw_1 = new StreamWriter("./GameData/PlayerData.txt"))
            //{
            //    sw_1.Write(text);
            //    sw_1.Close();
            //}

        }





        Time.timeScale = 1;
        SceneManager.LoadScene("menu");
    }
}