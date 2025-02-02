using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Vault : MonoBehaviour
{
    [SerializeField] GameObject inner;
    [SerializeField] GameObject outer;
    [SerializeField] Text textDisplay;
    bool canBeOpened = false;
    bool opened = false;
    
    private List<int> numbers = new List<int>();

    public void Reveal() {
        Debug.LogWarning("vault can be opened!");
        canBeOpened = true;
    }
    void Start()
    {
        outer.SetActive(true);
        inner.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNumber(int number) {
        Debug.Log("add number: " + number);
        numbers.Add(number);

        if (numbers.Count == 4) {
            if (ValidatePassword()) {
                opened = true;
                outer.SetActive(false);
                inner.SetActive(true);
                Debug.LogWarning("vault is opened!");
            } else {
                // pass
                Debug.LogWarning("vault is closed!");
            }
            numbers.Clear();
            textDisplay.text = "";
        } else {
            textDisplay.text = System.String.Join("", numbers.Select(x => x.ToString()).ToArray());
        }
    }

    private bool ValidatePassword() {
        if (!canBeOpened) {
                Debug.LogWarning("vault can be opened yet!");
            return false;
        }
        return numbers[0] == 3 && numbers[1] == 9 && numbers[2] == 6 && numbers[3] == 1;
    }
}
