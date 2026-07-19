using DG.Tweening;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationManagerScript : MonoBehaviour
{
    // Visual depiction of a treasure.
    [SerializeField] GameObject treasurePrefab;
    [SerializeField] Canvas GameCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ManagerScript.OnRevealChoices += RevealPlayerChoices;
        ScenarioScript.OnTreasuresAdded += AddTreasures;
        ScenarioScript.OnTreasuresRemoved += RemoveTreasures;
    }

    private void OnDisable()
    {
        ManagerScript.OnRevealChoices -= RevealPlayerChoices;
        ScenarioScript.OnTreasuresAdded -= AddTreasures;
        ScenarioScript.OnTreasuresRemoved -= RemoveTreasures;
    }

    // Place player icons next to the choices they made.
    private async Task RevealPlayerChoices(ManagerScript gameManager)
    {
        List<PlayerScript> delvers = gameManager.delvers;
        List<GameObject> delverIcons = gameManager.delverIcons;
        List<OptionScript> options = gameManager.optionDisplays;
        int[] optionChosenCount = new int[options.Count];

        List<Task> choicesTasks = new List<Task>();
        foreach(PlayerScript delver in delvers)
        {
            int iconId = delver.delverID - 1;
            GameObject currentPlayerIcon = delverIcons[iconId];
            GameObject chosenOption = options[delver.actionIdx].gameObject;

            // Shift icons to the right so they don't overlap when multiple players choose the same option.
            optionChosenCount[delver.actionIdx] += 1;
            float offset = optionChosenCount[delver.actionIdx] * currentPlayerIcon.GetComponent<RectTransform>().rect.width + 5f;
            Rect optionRect = chosenOption.GetComponent<RectTransform>().rect;

            // Move the invisible icons to a location near the option chosen by the player.
            currentPlayerIcon.GetComponent<CanvasGroup>().alpha = 0;
            currentPlayerIcon.transform.position = new Vector2(chosenOption.transform.position.x - (optionRect.width * 0.3f) + offset, chosenOption.transform.position.y - (optionRect.height * 0.2f) - 100f);

            // Animate the icons moving upward and becoming visible.
            choicesTasks.Add(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(1, 0.3f).AsyncWaitForCompletion());
            choicesTasks.Add(currentPlayerIcon.transform.DOMoveY(currentPlayerIcon.transform.position.y + 100f, Random.Range(0.5f, 1f)).AsyncWaitForCompletion());
        }

        await Task.WhenAll(choicesTasks);
    }

    // Highlight a single player plaque and a single option card to indicate whose turn is being resolved.
    private async Task HighlightPlayerAndChoice(ScenarioScript scenario)
    {
        await Task.CompletedTask;
    }

    // Visually add treasures to a player.
    private async Task AddTreasures(PlayerScript delver, int treasureDelta)
    {
        List<Task> addTreasuresTasks = new List<Task>();

        for (int i = 0; i < treasureDelta; i++)
        {
            Sequence addTreasureSequence = DOTween.Sequence();

            //Add a staggered delay to each treasure's animations so they don't completely overlap.
            addTreasureSequence.PrependInterval(i * 0.2f);

            GameObject newTreasure = Instantiate(treasurePrefab, new Vector2(delver.transform.position.x, delver.transform.position.y + 100f), Quaternion.identity, GameCanvas.transform); 
            addTreasureSequence.Append(newTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.3f));
            addTreasureSequence.Append(newTreasure.transform.DOMoveY(delver.transform.position.y, 1f));
            addTreasureSequence.Append(newTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.3f));
           
            addTreasuresTasks.Add(addTreasureSequence.AsyncWaitForCompletion());
        }
        //TODO: animate the score increase
        delver.playerScoreText.text = (delver.treasures + treasureDelta).ToString();

        await Task.WhenAll(addTreasuresTasks);
        
    }

    // Visually remove treasures from a player.
    private async Task RemoveTreasures(PlayerScript delver, int treasureDelta)
    {
        await Task.CompletedTask;
        //for(int i = 0; i < -treasureDelta; i++)
        //{

        //}
    }
}
