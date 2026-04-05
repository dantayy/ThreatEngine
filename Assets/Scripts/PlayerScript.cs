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
    // target(s) delver picks when taking certain actions
    public List<PlayerScript> targets;

    private GameObject playerCanvas;
    public TextMeshProUGUI playerTitleText;
    public TextMeshProUGUI playerScoreText;

    // Awake is called before Start
    void Awake()
    {
        playerCanvas = transform.Find("PlayerCanvas").gameObject;
        playerTitleText = playerCanvas.transform.Find("PlayerTitleText").GetComponent<TextMeshProUGUI>();
        playerScoreText = playerCanvas.transform.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
