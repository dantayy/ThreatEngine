using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManagerScript : MonoBehaviour
{
    // game manager to reference delver assets with
    [SerializeField]ManagerScript manager;
    // Visual depiction of the spirit
    [SerializeField] GameObject spiritPrefab;
    // Visual depiction of treasures for each delver
    [SerializeField] List<GameObject> treasures0;
    [SerializeField] List<GameObject> treasures1;
    [SerializeField] List<GameObject> treasures2;
    [SerializeField] List<GameObject> treasures3;
    public List<List<GameObject>> treasureLists;
    // action options for scenarios
    [SerializeField] List<OptionScript> options;
    // image assets
    [SerializeField] Image check;
    [SerializeField] Image cross;
    [SerializeField] Image randomSelectionArrow;
    [SerializeField] GameObject coin;
    [SerializeField] Sprite coinHeads;
    [SerializeField] Sprite coinSide;
    [SerializeField] Sprite coinTails;

    // delver assets
    public List<PlayerScript> delvers;
    public List<GameObject> delverIcons;

     // multiplier for all animation speeds
     [SerializeField] float animSpeedMultiplier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // create spirit prefab
        Instantiate(spiritPrefab);
        // add delver assets from the manager
        delvers = manager.delvers;
        delverIcons = manager.delverIcons;
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
        ManagerScript.OnActionResolutionCompleted += CleanUpActionResolution;
        
        ScenarioScript.OnResetDelvers += ResetPlayerAndChoice;
        ScenarioScript.OnSpiritBribingBegan += RevealSpiritBribes;
        ScenarioScript.OnActionResolutionBegan += HighlightPlayerAndChoice;
        ScenarioScript.OnTreasuresAdded += AddTreasures;
        ScenarioScript.OnTreasuresRemoved += RemoveTreasures;
        ScenarioScript.OnTreasuresStolen += StealTreasures;
        ScenarioScript.OnChangePlayerInvolvement += ChangePlayerInvolvement;
        ScenarioScript.OnChangeIconInvolvement += ChangeIconInvolvement;
        ScenarioScript.OnConditionResult += DisplayConditionResult;
        ScenarioScript.OnTreasuresRotated += RotateTreasures;
        ScenarioScript.OnTreasuresSwapped += SwapTreasures;
        ScenarioScript.OnRandomDelver += ChooseRandomDelver;
        ScenarioScript.OnCoinFlip += FlipCoin;
    }

    private void OnDisable()
    {
        ManagerScript.OnRevealChoices -= RevealPlayerChoices;
        ManagerScript.OnActionResolutionCompleted -= CleanUpActionResolution;

        ScenarioScript.OnResetDelvers -= ResetPlayerAndChoice;
        ScenarioScript.OnSpiritBribingBegan -= RevealSpiritBribes;
        ScenarioScript.OnActionResolutionBegan -= HighlightPlayerAndChoice;
        ScenarioScript.OnTreasuresAdded -= AddTreasures;
        ScenarioScript.OnTreasuresRemoved -= RemoveTreasures;
        ScenarioScript.OnTreasuresStolen -= StealTreasures;
        ScenarioScript.OnChangePlayerInvolvement -= ChangePlayerInvolvement;
        ScenarioScript.OnChangeIconInvolvement -= ChangeIconInvolvement;
        ScenarioScript.OnConditionResult -= DisplayConditionResult;
        ScenarioScript.OnTreasuresRotated -= RotateTreasures;
        ScenarioScript.OnTreasuresSwapped -= SwapTreasures;
        ScenarioScript.OnRandomDelver -= ChooseRandomDelver;
        ScenarioScript.OnCoinFlip -= FlipCoin;
    }

    // Center players who are interacting with the spirit
    private async Task RevealSpiritBribes()
    {
        // animation task list for spirit calling
        List<Task> callingTasks = new List<Task>();
        // temps for looping through icons
        GameObject currentPlayerIcon;
        // loop through delvers
        foreach(PlayerScript delver in delvers)
        {
            currentPlayerIcon = delverIcons[delver.delverID - 1];
            // if bribed anything
            if(delver.spiritBribe > 0)
            {
                // Animate the icons moving upward and becoming visible.
                callingTasks.Add(currentPlayerIcon.GetComponent<CanvasGroup>().DOFade(1, 0.3f * animSpeedMultiplier).AsyncWaitForCompletion());
                callingTasks.Add(currentPlayerIcon.transform.DOMoveY(currentPlayerIcon.transform.position.y + 100f, Random.Range(0.5f, 1f) * animSpeedMultiplier).AsyncWaitForCompletion());
                // TODO: animate a spirit icon
            }
        }

        await Task.WhenAll(callingTasks);
    }

    // Place player icons next to the choices they made.
    private async Task RevealPlayerChoices()
    {
        int[] optionChosenCount = new int[options.Count];

        List<Task> choicesTasks = new List<Task>();
        foreach(PlayerScript delver in delvers)
        {
            int iconId = delver.delverID - 1;
            GameObject currentPlayerIcon = delverIcons[iconId];
            GameObject chosenOption = options[(int)delver.actionIdx].gameObject;

            // Shift icons to the right so they don't overlap when multiple players choose the same option.
            optionChosenCount[(int)delver.actionIdx] += 1;
            float offset = optionChosenCount[(int)delver.actionIdx] * currentPlayerIcon.GetComponent<RectTransform>().rect.width + 5f;
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
    private async Task HighlightPlayerAndChoice(PlayerScript currentDelver)
    {
        List<Task> highlightTasks = new List<Task>();
        Sequence highlightSequence = DOTween.Sequence();

        //Fade all other delvers and their choice icons
        highlightSequence.Append(currentDelver.GetComponent<CanvasGroup>().DOFade(1, 0.5f * animSpeedMultiplier));
        highlightSequence.Append(currentDelver.playerPlaque.GetComponent<CanvasGroup>().DOFade(1, 0.5f * animSpeedMultiplier));
        foreach(PlayerScript delver in delvers)
        {
            if(delver != currentDelver)
            {
                highlightSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
                highlightSequence.Join(delver.playerPlaque.GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
                highlightSequence.Join(delverIcons[delver.delverID - 1].GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
                highlightSequence.Join(delverIcons[delver.delverID - 1].GetComponent<RectTransform>().DOScale(0.5f, 0.5f * animSpeedMultiplier));
            }
        }

        //Fade all other options
        highlightSequence.Append(options[(int)currentDelver.actionIdx].GetComponent<RectTransform>().DOScale(0.7f, 0.5f * animSpeedMultiplier));
        for(int i = 0; i < options.Count; i++)
        {
            if (i == (int)currentDelver.actionIdx) { continue; }
            highlightSequence.Join(options[i].GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f * animSpeedMultiplier));
        }

        highlightTasks.Add(highlightSequence.AsyncWaitForCompletion());
        await Task.WhenAll(highlightTasks);
    }

    // Return all player plaques, player choice icons, and option cards to normal.
    private async Task ResetPlayerAndChoice()
    {
        List<Task> resetTasks = new List<Task>();
        Sequence resetSequence = DOTween.Sequence();

        foreach(PlayerScript delver in delvers)
        {
            resetSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
            resetSequence.Join(delver.playerPlaque.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f * animSpeedMultiplier));
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

        resetSequence.Append(randomSelectionArrow.GetComponent<CanvasGroup>().DOFade(0f, 1.0f * animSpeedMultiplier));
        resetTasks.Add(resetSequence.AsyncWaitForCompletion());
        await Task.WhenAll(resetTasks);
    }

    // Highlight / unhighlight player plaque(s).
    private async Task ChangePlayerInvolvement(List<PlayerScript> involvedDelvers, bool involve = true)
    {
        List<Task> involveTasks = new List<Task>();
        Sequence involveSequence = DOTween.Sequence();

        float fadeLevel = involve ? 1.0f : 0.5f;
        foreach(PlayerScript delver in involvedDelvers)
        {
            involveSequence.Join(delver.GetComponent<CanvasGroup>().DOFade(fadeLevel, 0.5f * animSpeedMultiplier));
            involveSequence.Join(delver.playerPlaque.GetComponent<CanvasGroup>().DOFade(fadeLevel, 0.5f * animSpeedMultiplier));
        }

        involveTasks.Add(involveSequence.AsyncWaitForCompletion());
        await Task.WhenAll(involveTasks);
    }

    // Highlight / unhighlight player choice icon(s).
    private async Task ChangeIconInvolvement(List<PlayerScript> involvedDelvers, bool involve = true)
    {
        List<Task> involveTasks = new List<Task>();
        Sequence involveSequence = DOTween.Sequence();

        float fadeLevel = involve ? 1.0f : 0.5f;
        foreach (PlayerScript delver in involvedDelvers)
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
    private async Task CleanUpActionResolution()
    {
        await ResetPlayerAndChoice();

        List<Task> hideIconsTasks = new List<Task>();
        Sequence hideIconsSequence = DOTween.Sequence();

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
                    new Vector2((thievingDelver.transform.position.x + delver.transform.position.x) / 2, thievingDelver.transform.position.y + 100),
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

    // Rotate all delvers' treasure totals left or right.
    private async Task RotateTreasures(List<PlayerScript> delvers, bool rotateRight)
    {
        List<Task> rotateTreasuresTasks = new List<Task>();
        Sequence simultaneousRotateSequence = DOTween.Sequence();

        Vector2[] scoreTextPositions = new Vector2[delvers.Count];
        foreach (PlayerScript delver in delvers)
        {
            GameObject delverScoreTextObject = delver.playerScoreText.gameObject;
            PlayerScript targetDelver = rotateRight ? delver.rightDelver : delver.leftDelver;
            GameObject targetDelverScoreTextObject = targetDelver.playerScoreText.gameObject;

            // Store the position of each delver's scoreText.
            scoreTextPositions[delver.delverID - 1] = delverScoreTextObject.transform.position;

            // Move each scoreText to the new delver.
            if ((rotateRight && delver.delverID != delvers.Count) || !rotateRight && delver.delverID != 1)
            {
                simultaneousRotateSequence.Join(delverScoreTextObject.transform.DOMove(targetDelverScoreTextObject.transform.position, 1f * animSpeedMultiplier));
                continue;
            }

            // Unique behavior for the text that is wrapping around the screen.
            Vector3[] travelPoints = new Vector3[]
                {
                    new Vector2((targetDelverScoreTextObject.transform.position.x + delverScoreTextObject.transform.position.x) / 2, targetDelverScoreTextObject.transform.position.y + 100),
                    targetDelverScoreTextObject.transform.position,
                };

            simultaneousRotateSequence.Join(delverScoreTextObject.transform.DOPath(travelPoints, 1f * animSpeedMultiplier, PathType.CatmullRom));
        }

        rotateTreasuresTasks.Add(simultaneousRotateSequence.AsyncWaitForCompletion());
        await Task.WhenAll(rotateTreasuresTasks);

        // Reset all scoreText positions and update them to the new values for a seamless transition.
        foreach (PlayerScript delver in delvers)
        {
            TextMeshProUGUI delverScoreText = delver.playerScoreText;
            delverScoreText.gameObject.transform.position = scoreTextPositions[delver.delverID - 1];
            delverScoreText.text = delver.treasures.ToString();
        }
    }

    // Switch the treasure totals of two delvers.
    private async Task SwapTreasures(PlayerScript delver1, PlayerScript delver2)
    {
        List<Task> swapTreasuresTasks = new List<Task>();
        Sequence simultaneousSwapSequence = DOTween.Sequence();

        GameObject delverScoreTextObject = delver1.playerScoreText.gameObject;
        GameObject targetDelverScoreTextObject = delver2.playerScoreText.gameObject;

        Vector2[] scoreTextPositions = new Vector2[2];
        scoreTextPositions[0] = delverScoreTextObject.transform.position;
        scoreTextPositions[1] = targetDelverScoreTextObject.transform.position;

        // Move one scoreText in a straight line
        simultaneousSwapSequence.Join(delverScoreTextObject.transform.DOMove(targetDelverScoreTextObject.transform.position, 1f * animSpeedMultiplier));
        
        // Move the other scoreText in an arc
        Vector3[] travelPoints = new Vector3[]
                {
                    new Vector2((targetDelverScoreTextObject.transform.position.x + delverScoreTextObject.transform.position.x) / 2, delverScoreTextObject.transform.position.y + 100),
                    delverScoreTextObject.transform.position,
                };
        simultaneousSwapSequence.Join(targetDelverScoreTextObject.transform.DOPath(travelPoints, 1f * animSpeedMultiplier, PathType.CatmullRom));
        
        swapTreasuresTasks.Add(simultaneousSwapSequence.AsyncWaitForCompletion());
        await Task.WhenAll(swapTreasuresTasks);

        // Reset both scoreText positions and update them to the new values for a seamless transition.
        delverScoreTextObject.transform.position = scoreTextPositions[0];
        targetDelverScoreTextObject.transform.position = scoreTextPositions[1];
        delver1.playerScoreText.text = delver1.treasures.ToString();
        delver2.playerScoreText.text = delver2.treasures.ToString();
    }

    // Toss a coin that lands on heads or tails.
    private async Task FlipCoin(bool heads)
    {
        List<Task> coinFlipTasks = new List<Task>();
        Sequence coinFlipSequence = DOTween.Sequence();

        SpriteRenderer coinSpriteRenderer = coin.GetComponent<SpriteRenderer>();
        float totalDuration = 2.0f;
        int halfSpins = 10;
        float singleHalfSpinTime = totalDuration / halfSpins;

        // Coin appears
        coinFlipSequence.Join(coin.GetComponent<SpriteRenderer>().DOFade(1.0f, 0.25f * animSpeedMultiplier));

        // Coin vertical movement
        coinFlipSequence.Join(
            coin.transform.DOLocalMoveY(transform.localPosition.y + 2.5f, totalDuration / 2f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );

        // Coin flipping while in the air
        for (int i = 0; i < halfSpins; i++)
        {
            int step = i;
            coinFlipSequence.Insert(
                step * singleHalfSpinTime,
                coin.transform.DORotate(new Vector3(0, 90f, 0), singleHalfSpinTime / 2f).SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // Alternate sprites on each half-spin. Guarantee the final result.
                        if (step == halfSpins - 1)
                        {
                            coinSpriteRenderer.sprite = heads ? coinHeads: coinTails;
                        }
                        else
                        {
                            coinSpriteRenderer.sprite = coinSpriteRenderer.sprite == coinHeads ? coinTails : coinHeads;
                        }
                        coin.transform.eulerAngles = new Vector3(0, -90f, 0);
                    })
            );

            coinFlipSequence.Insert(
                (step * singleHalfSpinTime) + (singleHalfSpinTime / 2f),
                coin.transform.DORotate(new Vector3(0, 0f, 0), singleHalfSpinTime / 2f).SetEase(Ease.Linear)
            );
        }

        // Coin disappears
        coinFlipSequence.AppendInterval(1f);
        coinFlipSequence.Append(coin.GetComponent<SpriteRenderer>().DOFade(0.0f, 0.25f * animSpeedMultiplier));

        coinFlipTasks.Add(coinFlipSequence.AsyncWaitForCompletion());
        await Task.WhenAll(coinFlipTasks);
    }

    // Select one delver at random from a provided list for the purpose of another effect.
    private async Task ChooseRandomDelver(List<PlayerScript> affectedDelvers, PlayerScript chosenDelver)
    {
        List<Task> randomDelverTasks = new List<Task>();
        Sequence randomDelverSequence = DOTween.Sequence();

        // Choose a random delver above which to initially position the selection arrow.
        int currentIndex = Random.Range(0, affectedDelvers.Count - 1);
        PlayerScript currentDelver = affectedDelvers[currentIndex];

        randomSelectionArrow.transform.position = new Vector2(currentDelver.gameObject.transform.position.x, currentDelver.gameObject.transform.position.y + 180);
        randomDelverSequence.Append(randomSelectionArrow.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f * animSpeedMultiplier));

        // Choose a number of jumps for the arrow to make before stopping.
        int numberOfJumps = 12 + Random.Range(0, affectedDelvers.Count * 3);
        float randomSelectionSpeed = 0.25f;
        Debug.Log("Jump count: " + numberOfJumps);

        for(int i = 0; i < numberOfJumps; i++)
        {
            PlayerScript nextDelver = currentDelver.rightDelver;
            Vector3[] travelPoints = new Vector3[]
                {
                    new Vector2((currentDelver.transform.position.x + nextDelver.transform.position.x) / 2, nextDelver.transform.position.y + 300),
                     new Vector2(nextDelver.transform.position.x, nextDelver.transform.position.y + 180),
                };
            randomDelverSequence.Append(randomSelectionArrow.transform.DOPath(travelPoints, randomSelectionSpeed * animSpeedMultiplier, PathType.CatmullRom));

            // When aren't very many jumps left, decrease animation speed.
            if(i > numberOfJumps - 8)
            {
                randomSelectionSpeed += 0.05f;
            }

            if (i == (numberOfJumps - 1))
            {
                // Continue jumping until the chosen delver is reached.
                if (nextDelver.delverID != chosenDelver.delverID)
                {
                    i--;
                }

                // Surprise final jump happens 1/10 times.
                if(nextDelver.delverID == chosenDelver.delverID - 1 && (Random.Range(0, 9) == 0))
                {
                    randomDelverSequence.AppendInterval(2f * animSpeedMultiplier);
                    randomSelectionSpeed = 3f;
                }
            }

            currentDelver = nextDelver;
        }

        randomDelverTasks.Add(randomDelverSequence.AsyncWaitForCompletion());
        await Task.WhenAll(randomDelverTasks);
    }
}
