using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;


public class CreatePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject Msg, btn_obj, drop_temp;
    Button btn_Create;
    int frm;
    FileInfo file;
    public InputField playername;
    Text CreateMessage;
    string path;
    Dropdown dropdownitem;

    void Start()
    {
        //Debug.Log("create player");
        path = "./GameData/PlayerData.txt";
        if (Directory.Exists("./GameData") == false)
        {
            Directory.CreateDirectory("./GameData");
        }
        
        file = new FileInfo(path);
        if(!file.Exists)
        {
            FileStream player_file;
            player_file = file.Create();
            player_file.Close();

        }

        //Debug.Log(file.Exists);

        Msg = GameObject.Find("Canvas/btn_Create/CreateMessage");
        Msg.SetActive(true);
        CreateMessage = Msg.GetComponent<Text>();
        //Debug.Log(CreateMessage.text);
        
        btn_obj = GameObject.Find("btn_Create");
        btn_Create = btn_obj.GetComponent<Button>();

        drop_temp = GameObject.Find("Dropdown");
        dropdownitem = drop_temp.GetComponent<Dropdown>();
        dropdownitem.interactable = true;

        dropdownitem.ClearOptions();

        btn_Create.onClick.AddListener(delegate ()
        {
            this.OnClick(btn_obj);
        });

        Msg.SetActive(false);
        frm = 500;


        //read the Player Data
        using(StreamReader sr_1 = new StreamReader(path))
        {
            string player_data;

            do
            {
                player_data = sr_1.ReadLine();
                if (player_data == null)                     //Ū����Ʈw
                {
                    break;
                }
                Dropdown.OptionData data = new Dropdown.OptionData();
                string[] temp = player_data.Split(' ');
                string temp_name = (temp[1].Split(',')[0]).ToString();          //name
                data.text = temp_name.ToString();
                dropdownitem.options.Add(data);

            } while (true);
            sr_1.Close();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(--frm == 0)
        {
            GameObject inputFieldGo = GameObject.Find("InputField_PlayerName");
            InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
            inputFieldCo.text = "";
            btn_Create.enabled = true;
            Msg.SetActive(false);
        }
    }

    public void OnClick(GameObject sender)
    {
        btn_Create.enabled = false;
        int is_found = 0;
       
        GameObject inputFieldGo = GameObject.Find("InputField_PlayerName");
        InputField inputFieldCo = inputFieldGo.GetComponent<InputField>();
        //Debug.Log(inputFieldCo.text);
        string input_name = inputFieldCo.text;
        string player_data;

        if(input_name == "")
        {
            CreateMessage.text = "Error!! Field Empty";
            CreateMessage.color = Color.red;
            Msg.SetActive(!Msg.activeSelf);
            frm = 500;
            return;
        }


        using (StreamReader sr = new StreamReader(path))
        {
            do
            {
                player_data = sr.ReadLine();
                if (player_data == null)                     //Ū����Ʈw�A���s�b�o�ӷs�Τ�
                {
                    is_found = 0;
                    break;
                }
                string[] temp = player_data.Split(' ');
                string temp_name = (temp[1].Split(',')[0]).ToString();

                if (temp_name == input_name)
                {
                    is_found = 1;
                    break;
                }
            } while (true);
            sr.Close();
        }

        if(is_found == 0)
        {
            //Create new player
            using (StreamWriter sw = file.AppendText())
            {
                sw.WriteLine("Name: " + input_name + ", Money: " + 100);
                sw.Flush();
                CreateMessage.text = ("Create Player: " + input_name.ToString() + " Success!!");
                CreateMessage.color = Color.green;

                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = input_name.ToString();
                dropdownitem.options.Add(data);
                sw.Close();
            }
            
        }
        else if(is_found == 1)
        {
            //return error
            CreateMessage.text = ("Fail!!! Player: " + input_name.ToString());
            CreateMessage.text += " Exits!";
            CreateMessage.color = Color.red;

        }
        
        Msg.SetActive(!Msg.activeSelf);
        frm = 500;
    }

   
}

