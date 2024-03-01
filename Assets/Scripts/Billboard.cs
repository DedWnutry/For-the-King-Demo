using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private DungeonMaster GM;

    void Start()
    {
        GM = GameObject.FindGameObjectWithTag("GameController").GetComponent<DungeonMaster>();
    }

    void Update()
    {
        transform.LookAt(GM.player.transform.position);
    }
}
