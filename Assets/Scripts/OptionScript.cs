using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// contains logic for displaying scenario option information onscreen
public class OptionScript : MonoBehaviour
{
    public string title = "";
    public string effect = "";
    public string id = "";

    public TextMeshProUGUI titleElement;
    public TextMeshProUGUI effectElement;
    public TextMeshProUGUI optionIDElement;

    // Awake is called before Start
    void Awake()
    {
        titleElement = transform.Find("OptionTitle").GetComponent<TextMeshProUGUI>();
        effectElement = transform.Find("OptionEffect").GetComponent<TextMeshProUGUI>();
        optionIDElement = transform.Find("OptionID").GetComponent<TextMeshProUGUI>();
    }

    //Update UI elements and display the option
    public void DisplayOption()
    {
        titleElement.text = title;
        effectElement.text = effect;
        optionIDElement.text = id;
    }
}
