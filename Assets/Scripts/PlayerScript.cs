using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // ID of delver
    public int delverID = -1;
    // number of treasures delver currently has
    public int treasures = 0;
    // flag set when a delver calls to the spirit
    public bool callToSpirit = false;
    // flag set when the delver is favored by the spirit
    public bool favored = false;
    // action delver is choosing in a scenario
    public int actionIdx = -1;
    // reference to previous delver in turn order
    public PlayerScript leftDelver;
    // reference to next delver in turn order
    public PlayerScript rightDelver;
    // target delver picks when taking certain actions
    public PlayerScript target;
    // flag set when the delver runs out of time when making a choice
    public bool choseRandomly = false;

    public TextMeshProUGUI playerTitleText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI playerDebugText;
    public GameObject playerInputIcons;

    // Awake is called before Start
    void Awake()
    {
        playerTitleText = transform.Find("PlayerTitleText").GetComponent<TextMeshProUGUI>();
        playerScoreText = transform.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
        playerDebugText = transform.Find("PlayerDebugText").GetComponent<TextMeshProUGUI>();
        playerInputIcons = transform.Find("PlayerInputIcons").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Reset all values related to choices a player makes during a round
    public void ClearChoices()
    {
        actionIdx = -1;
        target = null;
        callToSpirit = false;
        choseRandomly = false;
        playerDebugText.text = string.Empty;
        playerInputIcons.SetActive(false);
    }
}
