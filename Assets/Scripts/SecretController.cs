using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretController : MonoBehaviour
{
    private int paperCount = 0;

    // PaperSpawner paperSpawner;
    private int currentSecret = 1;

    private Dictionary<int, int> secretNumberToItsAppearance = new Dictionary<int, int>();

    [SerializeField] List<GameObject> secretPrefabs;

    void Start()
    {
        // paperSpawner = FindObjectOfType<PaperSpawner>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IncreasePaperCount()
    {
        paperCount += 1;
    }

    public void RevealSecret(int secretNum)
    {
        if (secretNumberToItsAppearance.ContainsKey(secretNum))
        {
            Debug.LogWarning("TRYING TO REVEAL SECRET THAT ALREADY REVEALED: " + secretNum.ToString());
        }
        else if (currentSecret != secretNum)
        {
            Debug.LogWarning("TRYING TO REVEAL SECRET: " + secretNum.ToString() + " !=currentSecret (" + currentSecret.ToString() + ")");
        }
        else
        {
            Debug.LogWarning("REVEALING SECRET: " + secretNum.ToString());
            secretNumberToItsAppearance[secretNum] = paperCount;
            currentSecret += 1;
        }
    }

    // 0 stays for neutral paper
    // -1 for green paper
    public int CurrentSecret()
    {
        if (currentSecret == 1)
        { // paper with first text about ultraviolet
            if (paperCount >= 5 && paperCount % 6 == 5)
            {
                return 1;
            }
            else return 0;
        }
        else if (currentSecret == 2)
        { // first paper wuth uv light
            // save from random revealing
            if ((paperCount - secretNumberToItsAppearance[1]) % 7 == 0)
            {
                return 1;
            }
            // secret number 2
            if ((paperCount - secretNumberToItsAppearance[1]) % 4 == 0)
            {
                return 2;
            }
            return 0;
        }
        else if (currentSecret == 3)
        {  // paper u should heat
            // save from random revealing (continue to spawn uv secret)
            if ((paperCount - secretNumberToItsAppearance[2]) % 7 == 0) {
                return 2;
            }
            // secret number 3
            if ((paperCount - secretNumberToItsAppearance[2]) % 6 == 0) {
                return 3;
            }
            return 0;
        }
        else if (currentSecret == 4) { // officer quiz
            // save prev secret
            if ((paperCount - secretNumberToItsAppearance[3]) % 18 == 0) {
                return 3;
            }
            // green light
            if ((paperCount - secretNumberToItsAppearance[3]) % 18 == 3) {
                return -1;
            }
            // secret number 4
            if ((paperCount - secretNumberToItsAppearance[3]) % 18 == 16) {
                return 4;
            }
            return 0;
        }
        else
        {
            Debug.LogWarning("Secrets ends: " + currentSecret.ToString());
            return 0;
        }
    }

    public int PotentialCurrentSecret() {
        return currentSecret;
    }

    public GameObject GetSecretPrefab(int index)
    {
        return secretPrefabs[index];
    }
}
