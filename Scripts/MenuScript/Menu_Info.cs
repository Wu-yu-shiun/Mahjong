using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class Menu_Info : MonoBehaviour
{
    Player_Class.Player player;

    FileInfo file;
    string path;

    // Start is called before the first frame update
    void Start()
    {
        path = "./GameData/PlayerData.txt";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
