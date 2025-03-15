using ArtificialIntelligence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Text>().text = new Class1().Test();
        // Debug.Log();
    }
}
