using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountGUI : MonoBehaviour
{
    private TextMeshProUGUI tmProEleml;
    public string itemName;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        tmProEleml = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCount()
    {
        count++;
        tmProEleml.text = itemName + ": " + count;
    }
}
