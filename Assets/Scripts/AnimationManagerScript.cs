using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManagerScript : MonoBehaviour
{
    // Visual depiction of a treasure.
    [SerializeField] List<GameObject> treasures0;
    [SerializeField] List<GameObject> treasures1;
    [SerializeField] List<GameObject> treasures2;
    [SerializeField] List<GameObject> treasures3;
    [SerializeField] Canvas GameCanvas;
    [SerializeField] List<OptionScript> options;
    [SerializeField] Image check;
    [SerializeField] Image cross;
    [SerializeField] float animSpeedMultiplier; // Multiplier for all animation speeds

    public List<GameObject> delverIcons;

    public List<List<GameObject>> treasureLists; // Each delver gets their own list of treasure objects to be used for animations

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Add each delver's list of treasure objects to the main list
        treasureLists = new List<List<GameObject>>();
        treasureLists.AddRange(new List<List<GameObject>> { treasures0, treasures1, treasures2, treasures3 });
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
        ScenarioScript.OnTreasuresStolen += StealTreasures;
        ScenarioScript.OnChangePlayerInvolvement += ChangePlayerInvolvement;
        ScenarioScript.OnChangeIconInvolvement += ChangeIconInvolvement;
        ScenarioScript.OnConditionResult += DisplayConditionResult;
        ManagerScript.OnActionResolutionCompleted += CleanUpActionResolution;
    }

    private void OnDisable()
    {
        ManagerScript.OnRevealChoices -= RevealPlayerChoices;
        ScenarioScript.OnActionResolutionBegan -= ResetPlayerAndChoice;
        ScenarioScript.OnActionResolutionBegan -= HighlightPlayerAndChoice;
        ScenarioScript.OnTreasuresAdded -= AddTreasures;
        ScenarioScript.OnTreasuresRemoved -= RemoveTreasures;
        ScenarioScript.OnTreasuresStolen -= StealTreasures;
        ScenarioScript.OnChangePlayerInvolvement -= ChangePlayerInvolvement;
        ScenarioScript.OnChangeIconInvolvement -= ChangeIconInvolvement;
        ScenarioScript.OnConditionResult -= DisplayConditionResult;
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
            choicesTasks.Add(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(1, 0.3f * animSpeedMultiplier).AsyncWaitForCompletion());
            choicesTasks.Add(currentPlayerIcon.transform.DOMoveY(currentPlayerIcon.transform.position.y + 100f, Random.Range(0.5f, 1f) * animSpeedMultiplier).AsyncWaitForCompletion());
        }

        await Task.WhenAll(choicesTasks);
    }

    // Highlight a player plaque and an option card.
    private async Task HighlightPlayerAndChoice(List<PlayerScript> delvers, PlayerScript currentDelver)
    {
        List<Task> highlightTasks = new List<Task>();
        Sequence highlightSequence = DOTween.Sequence();

        //Fade all other delvers and their choice icons
        highlightSequence.Append(currentDelver.GetComponent<CanvasGroup>().DOFade(1, 0.5f * animSpeedMultiplier));
        foreach(PlayerScript delver in delvers)
        {
            if(delver != currentDelver)
            {
                highlightSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
                int iconId = delver.delverID - 1;
                GameObject currentPlayerIcon = delverIcons[iconId];
                highlightSequence.Join(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
                highlightSequence.Join(currentPlayerIcon.GetComponent<RectTransform>().DOScale(0.5f, 0.5f * animSpeedMultiplier));
            }
        }

        //Fade all other options
        highlightSequence.Append(options[currentDelver.actionIdx].GetComponent<RectTransform>().DOScale(0.7f, 0.5f * animSpeedMultiplier));
        for(int i = 0; i < options.Count; i++)
        {
            if (i == currentDelver.actionIdx) { continue; }
            highlightSequence.Join(options[i].GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
        }

        highlightTasks.Add(highlightSequence.AsyncWaitForCompletion());
        await Task.WhenAll(highlightTasks);
    }

    // Return all player plaques, player choice icons, and option cards to normal.
    private async Task ResetPlayerAndChoice(List<PlayerScript> delvers, PlayerScript currentDelver = null)
    {
        List<Task> resetTasks = new List<Task>();
        Sequence resetSequence = DOTween.Sequence();

        foreach(PlayerScript delver in delvers)
        {
            resetSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
            int iconId = delver.delverID - 1;
            GameObject currentPlayerIcon = delverIcons[iconId];
            resetSequence.Join(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
            resetSequence.Join(currentPlayerIcon.GetComponent<RectTransform>().DOScale(1.0f, 0.5f * animSpeedMultiplier));
        }

        foreach(OptionScript option in options)
        {
            resetSequence.Join(option.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
            resetSequence.Join(option.GetComponent<RectTransform>().DOScale(0.5f, 0.5f * animSpeedMultiplier));
        }

        resetTasks.Add(resetSequence.AsyncWaitForCompletion());
        await Task.WhenAll(resetTasks);
    }

    // Highlight / unhighlight player plaque(s).
    private async Task ChangePlayerInvolvement(List<PlayerScript> delvers, bool involve = true)
    {
        List<Task> involveTasks = new List<Task>();
        Sequence involveSequence = DOTween.Sequence();

        float fadeLevel = involve ? 1.0f : 0.5f;
        foreach(PlayerScript delver in delvers)
        {
            involveSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(fadeLevel, 0.5f * animSpeedMultiplier));
        }

        involveTasks.Add(involveSequence.AsyncWaitForCompletion());
        await Task.WhenAll(involveTasks);
    }

    // Highlight / unhighlight player choice icon(s).
    private async Task ChangeIconInvolvement(List<PlayerScript> delvers, bool involve = true)
    {
        List<Task> involveTasks = new List<Task>();
        Sequence involveSequence = DOTween.Sequence();

        float fadeLevel = involve ? 1.0f : 0.5f;
        foreach (PlayerScript delver in delvers)
        {
            int iconId = delver.delverID - 1;
            GameObject currentPlayerIcon = delverIcons[iconId];
            involveSequence.Join(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(fadeLevel, 0.5f * animSpeedMultiplier));
            involveSequence.Join(currentPlayerIcon.GetComponent<RectTransform>().DOScale(fadeLevel, 0.5f * animSpeedMultiplier));
        }

        involveTasks.Add(involveSequence.AsyncWaitForCompletion());
        await Task.WhenAll(involveTasks);
    }

    // Revert UI to normal when action resolution is completed.
    private async Task CleanUpActionResolution(ManagerScript gameManager, List<PlayerScript> delvers)
    {
        await ResetPlayerAndChoice(delvers);

        List<Task> hideIconsTasks = new List<Task>();
        Sequence hideIconsSequence = DOTween.Sequence();
        delverIcons = gameManager.delverIcons;

        foreach(GameObject delverIcon in delverIcons)
        {
            hideIconsSequence.Join(delverIcon.GetComponent<CanvasGroup>().DOFade(0, 0.3f * animSpeedMultiplier));
        }
        hideIconsTasks.Add(hideIconsSequence.AsyncWaitForCompletion());
        await Task.WhenAll(hideIconsTasks);
    }

    // Display an indicator when a condition check succeeds / fails.
    private async Task DisplayConditionResult(bool success = true)
    {
        List<Task> displayTasks = new List<Task>();
        Sequence displaySequence = DOTween.Sequence();

        Image displayedImage = success ? check : cross;
        displaySequence.Join(displayedImage.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
        displaySequence.Join(displayedImage.gameObject.GetComponent<RectTransform>().DOScale(1.5f, 0.5f * animSpeedMultiplier).SetEase(Ease.InBounce));
        displaySequence.Append(displayedImage.gameObject.GetComponent<RectTransform>().DOScale(1.0f, 0.5f * animSpeedMultiplier).SetEase(Ease.InBounce));
        displaySequence.Join(displayedImage.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f * animSpeedMultiplier));

        displayTasks.Add(displaySequence.AsyncWaitForCompletion());
        await Task.WhenAll(displayTasks);
    }

    // Visually add treasures to a player.
    private async Task AddTreasures(PlayerScript delver, int treasureDelta)
    {
        List<Task> addTreasuresTasks = new List<Task>();

        for(int i = 0; i < treasureDelta; i++)
        {
            Sequence addTreasureSequence = DOTween.Sequence();

            //Reuse treasure objects if necessary.
            List<GameObject> treasuresList = treasureLists[delver.delverID - 1];
            int treasureIndex = i % treasuresList.Count;
            GameObject currentTreasure = treasuresList[treasureIndex];

            //Add a staggered delay to each treasure's animations so they don't completely overlap.
            addTreasureSequence.PrependInterval(i * 0.2f * animSpeedMultiplier);

            currentTreasure.transform.position = new Vector2(delver.transform.position.x, delver.transform.position.y + 200f);
            addTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.2f * animSpeedMultiplier));
            addTreasureSequence.Join(currentTreasure.transform.DOMoveY(delver.transform.position.y, 1f * animSpeedMultiplier));
            addTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.2f * animSpeedMultiplier));
           
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
            List<GameObject> treasuresList = treasureLists[delver.delverID - 1];
            int treasureIndex = i % treasuresList.Count;
            GameObject currentTreasure = treasuresList[treasureIndex];

            //Add a staggered delay to each treasure's animations so they don't completely overlap.
            removeTreasureSequence.PrependInterval(i * 0.2f * animSpeedMultiplier);

            int randomXOffset = Random.Range(-100, 101);

            currentTreasure.transform.position = new Vector2(delver.transform.position.x, delver.transform.position.y);
            removeTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.2f * animSpeedMultiplier));
            removeTreasureSequence.Join(currentTreasure.transform.DOMove(new Vector2(delver.transform.position.x + randomXOffset, delver.transform.position.y + 200f), 1f * animSpeedMultiplier).SetEase(Ease.InOutCubic));
            removeTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.2f * animSpeedMultiplier));

            removeTreasuresTasks.Add(removeTreasureSequence.AsyncWaitForCompletion());
        }
        //TODO: animate the score increase
        delver.playerScoreText.text = (delver.treasures + treasureDelta).ToString();

        await Task.WhenAll(removeTreasuresTasks);
    }

    // Visually remove treasures from one or more players and add them to a theiving player.
    private async Task StealTreasures(PlayerScript thievingDelver, List<PlayerScript> delvers, int treasureDelta)
    {
        List<Task> stealTreasuresTasks = new List<Task>();

        for(int i = 0; i < Mathf.Abs(treasureDelta); i++)
        {
            Sequence simultaneousStealSequence = DOTween.Sequence();

            foreach(PlayerScript delver in delvers)
            {
                Sequence stealTreasureSequence = DOTween.Sequence();

                //Reuse treasure objects if necessary.
                List<GameObject> treasuresList = treasureLists[delver.delverID - 1];
                int treasureIndex = i % treasuresList.Count;
                GameObject currentTreasure = treasuresList[treasureIndex];

                //Add a staggered delay to each treasure's animations so they don't completely overlap.
                stealTreasureSequence.PrependInterval(i * 0.2f * animSpeedMultiplier);

                Vector3[] travelPoints = new Vector3[]
                {
                    new Vector2((thievingDelver.transform.position.x + delver.transform.position.x) / 2, thievingDelver.transform.position.y + 200),
                    new Vector2(thievingDelver.transform.position.x, thievingDelver.transform.position.y),
                };

                currentTreasure.transform.position = new Vector2(delver.transform.position.x, delver.transform.position.y);
                stealTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(1, 0.2f * animSpeedMultiplier));
                stealTreasureSequence.Join(currentTreasure.transform.DOPath(travelPoints, 1f * animSpeedMultiplier, PathType.CatmullRom));
                stealTreasureSequence.Append(currentTreasure.GetComponent<CanvasGroup>().DOFade(0, 0.2f * animSpeedMultiplier));

                simultaneousStealSequence.Join(stealTreasureSequence);
            }

            stealTreasuresTasks.Add(simultaneousStealSequence.AsyncWaitForCompletion());
        }

        // Update scores.
        thievingDelver.playerScoreText.text = (thievingDelver.treasures + treasureDelta * delvers.Count).ToString();
        foreach(PlayerScript delver in delvers)
        {
            delver.playerScoreText.text = (delver.treasures - treasureDelta).ToString();
        }

        await Task.WhenAll(stealTreasuresTasks);
    }
}
