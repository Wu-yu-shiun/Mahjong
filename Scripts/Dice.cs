using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    Rigidbody rb;
    public static Vector3 diceSpeed;
    public static int rollDice;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rollDice = 0;

    }

    // Update is called once per frame
    void Update()
    {
        diceSpeed = rb.velocity;

        if(rollDice<2 && Input.GetKeyDown(KeyCode.Space))
        {
            MahjongMovement.incidentSound = true;
            float dx = Random.Range(0, 5000);
            float dy = Random.Range(0, 5000);
            float dz = Random.Range(0, 5000);
            float strength = Random.Range(200, 600);
            //transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * strength);
            rb.AddTorque(dx, dy, dz);
            ++rollDice;
        }

    }
}

