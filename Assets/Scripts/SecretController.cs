using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretController : MonoBehaviour
{
    private int paperCount = 0;

    PaperSpawner paperSpawner;
    void Start()
    {
        paperSpawner = FindObjectOfType<PaperSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreasePaperCount() {
        paperCount += 1;
    }

    public bool CanSpawnFirstSecret() {
        return paperCount >= 5;
    }
}
