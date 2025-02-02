using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] GameObject small;
    [SerializeField] GameObject large;
    private bool grown = false;

    private void Start() {
        small.SetActive(true);
        large.SetActive(false);
    }
    public void Grow() {
        grown = true;
        small.SetActive(false);
        large.SetActive(true);
    }

    public bool IsGrown() {
        return grown;
    }
}
