using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSensor : MonoBehaviour
{
    Vector3 diceSpeed;
    int diceNum;
    int rollDice;
    int isCheckedDiceNum;
    public static bool checkEnd;
    float timepast;
    private void Start()
    {
        isCheckedDiceNum = 0;
        checkEnd = false;
        timepast = 0;
    }

    private void Update()
    {
        rollDice = Dice.rollDice;
    }

    private void FixedUpdate()
    {
        diceSpeed = Dice.diceSpeed;
        
    }

    private void OnTriggerStay(Collider collider)
    {
        if (!checkEnd)
        {
            if (diceSpeed.x == 0 && diceSpeed.y == 0 && diceSpeed.z == 0 && rollDice==2)
            {
                timepast += Time.deltaTime;
                if (timepast > 1 && isCheckedDiceNum<2)
                {
                    switch (collider.gameObject.name)
                    {
                        case "side1":
                            diceNum += 6;
                            break;
                        case "side2":
                            diceNum += 3;
                            break;
                        case "side3":
                            diceNum += 2;
                            break;
                        case "side4":
                            diceNum += 5;
                            break;
                        case "side5":
                            diceNum += 4;
                            break;
                        case "side6":
                            diceNum += 1;
                            break;
                    }
                    ++isCheckedDiceNum;
                }

                if (timepast > 3)
                {
                    checkEnd = true;
                    MahjongCanvas.DiceNumber= diceNum;
                }
            }
        }
    }
}
