using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationManagerScript : MonoBehaviour
{
    // Visual depiction of a treasure.
    [SerializeField] List<GameObject> treasures;
    [SerializeField] Canvas GameCanvas;
    [SerializeField] List<OptionScript> options;

    public List<GameObject> delverIcons;

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
        ScenarioScript.OnActionResolutionBegan += ResetPlayerAndChoice;
        ScenarioScript.OnActionResolutionBegan += HighlightPlayerAndChoice;
        ScenarioScript.OnTreasuresAdded += AddTreasures;
        ScenarioScript.OnTreasuresRemoved += RemoveTreasures;
        ManagerScript.OnActionResolutionCompleted += CleanUpActionResolution;
    }

    private void OnDisable()
    {
        ManagerScript.OnRevealChoices -= RevealPlayerChoices;
        ScenarioScript.OnActionResolutionBegan -= ResetPlayerAndChoice;
        ScenarioScript.OnActionResolutionBegan -= HighlightPlayerAndChoice;
        ScenarioScript.OnTreasuresAdded -= AddTreasures;
        ScenarioScript.OnTreasuresRemoved -= RemoveTreasures;
        ManagerScript.OnActionResolutionCompleted -= CleanUpActionResolution;
    }

    // Place player icons next to the choices they made.
    private async Task RevealPlayerChoices(ManagerScript gameManager)
    {
        List<PlayerScript> delvers = gameManager.delvers;
        delverIcons = gameManager.delverIcons;
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
    private async Task HighlightPlayerAndChoice(List<PlayerScript> delvers, PlayerScript currentDelver)
    {
        List<Task> highlightTasks = new List<Task>();
        Sequence highlightSequence = DOTween.Sequence();

        //Fade all other delvers
        highlightSequence.Append(currentDelver.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        foreach(PlayerScript delver in delvers)
        {
            if(delver != currentDelver)
            {
                highlightSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f));
            }
        }

        //Fade all other options
        highlightSequence.Append(options[currentDelver.actionIdx].GetComponent<RectTransform>().DOScale(0.7f, 0.5f));
        for(int i = 0; i < options.Count; i++)
        {
            if (i == currentDelver.actionIdx) { continue; }
            highlightSequence.Join(options[i].GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f));
        }

        highlightTasks.Add(highlightSequence.AsyncWaitForCompletion());

        //TODO: Fade other players' icon circles too.

        await Task.WhenAll(highlightTasks);
    }

    // Return all UI elements affected by HighlightPlayerAndChoice to normal.
    private async Task ResetPlayerAndChoice(List<PlayerScript> delvers, PlayerScript currentDelver = null)
    {
        List<Task> resetTasks = new List<Task>();
        Sequence resetSequence = DOTween.Sequence();

        foreach(PlayerScript delver in delvers)
        {
            resetSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f));
        }

        foreach(OptionScript option in options)
        {
            resetSequence.Join(option.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f));
            resetSequence.Join(option.GetComponent<RectTransform>().DOScale(0.5f, 0.5f));
        }

        resetTasks.Add(resetSequence.AsyncWaitForCompletion());
        await Task.WhenAll(resetTasks);
    }

    // Revert Ui to normal when action resolution is completed.
    private async Task CleanUpActionResolution(ManagerScript gameManager, List<PlayerScript> delvers)
    {
        await ResetPlayerAndChoice(delvers);

        List<Task> hideIconsTasks = new List<Task>();
        Sequence hideIconsSequence = DOTween.Sequence();
        delverIcons = gameManager.delverIcons;

        foreach(GameObject delverIcon in delverIcons)
        {
            hideIconsSequence.Join(delverIcon.GetComponent<CanvasGroup>().DOFade(0, 0.3f));
        }
        hideIconsTasks.Add(hideIconsSequence.AsyncWaitForCompletion());
    }

    // Visually add treasures to a player.
    private async Task AddTreasures(PlayerScript delver, int treasureDelta)
    {
        List<Task> addTreasuresTasks = new List<Task>();

        for(int i = 0; i < treasureDelta; i++)
        {
            Sequence addTreasureSequence = DOTween.Sequence();

            //Reuse treasure objects if necessary.
            int treasureIndex = i % treasures.Count;
            GameObject currentTreasure = treasures[treasureIndex];

            //Add a staggered delay to each treasure's animations so they don't completely overlap.
            addTreasureSequence.PrependInterval(i * 0.2f);

            currentTreasure.transform.position = new Vector2(delver.transform.position.x, delver.transform.position.y + 200f);
            addTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.2f));
            addTreasureSequence.Join(currentTreasure.transform.DOMoveY(delver.transform.position.y, 1f));
            addTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.2f));
           
            addTreasuresTasks.Add(addTreasureSequence.AsyncWaitForCompletion());
        }
        //TODO: animate the score increase
        delver.playerScoreText.text = (delver.treasures + treasureDelta).ToString();

        await Task.WhenAll(addTreasuresTasks);
    }

    // Visually remove treasures from a player.
    private async Task RemoveTreasures(PlayerScript delver, int treasureDelta)
    {
        List<Task> removeTreasuresTasks = new List<Task>();

        for(int i = 0; i < Mathf.Abs(treasureDelta); i++)
        {
            Sequence removeTreasureSequence = DOTween.Sequence();

            //Reuse treasure objects if necessary.
            int treasureIndex = i % treasures.Count;
            GameObject currentTreasure = treasures[treasureIndex];

            //Add a staggered delay to each treasure's animations so they don't completely overlap.
            removeTreasureSequence.PrependInterval(i * 0.2f);

            int randomXOffset = Random.Range(-100, 101);

            currentTreasure.transform.position = new Vector2(delver.transform.position.x, delver.transform.position.y);
            removeTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.2f));
            removeTreasureSequence.Join(currentTreasure.transform.DOMove(new Vector2(delver.transform.position.x + randomXOffset, delver.transform.position.y + 200f), 1f).SetEase(Ease.InOutCubic));
            removeTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.2f));

            removeTreasuresTasks.Add(removeTreasureSequence.AsyncWaitForCompletion());
        }
        //TODO: animate the score increase
        delver.playerScoreText.text = (delver.treasures + treasureDelta).ToString();

        await Task.WhenAll(removeTreasuresTasks);
    }
}
