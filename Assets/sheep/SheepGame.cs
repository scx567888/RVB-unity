using System.Collections;
using System.Collections.Generic;
using sheep;
using UnityEngine;

public class SheepGame : MonoBehaviour
{
    private SheepWorld sheepWorld;
    
    void Start()
    {
        sheepWorld = new SheepWorld();
    }

    
    void Update()
    {
        sheepWorld.tick();
    }
}
