using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Casino", menuName = "Scriptable Objects/Casino")]
public class Casino : ScenarioScript
{
    public Casino()
    {
        scenarioTitle = "The Casino";

        actionTitles.Add("Blackjack by the books");
        actionTitles.Add("Roulette");
        actionTitles.Add("All in on poker");

        actionEffects.Add("+5 [+3]");
        actionEffects.Add("Random delver [Pick a delver instead] loses all treasures.");
        actionEffects.Add("Flip a coin. [Flip two coins instead and use either result.] Heads: Double your treasures. Tails: Lose all your treasures.");
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // blackjack
            case 0:
                {
                    // add to delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, 5);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        // add to delver's treasures
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // roulette
            case 1:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        // target delver loses all treasures
                        await TreasureAdjustmentSingle(currentDelver.target, -currentDelver.target.treasures);
                        break;
                    }

                    // random delver loses all treasures
                    PlayerScript randomDelver = delversSortedScores[Random.Range(0, delversSortedScores.Count)];
                    await ChooseRandomDelver(delversSortedScores, randomDelver);
                    await TreasureAdjustmentSingle(randomDelver, -randomDelver.treasures);
                    break;
                }
            // poker
            case 2:
                {
                    // flip a coin
                    bool wonCoinFlip = Random.value > 0.5f;
                    await FlipCoin(wonCoinFlip);

                    // favored bonus
                    bool wonBonusFlip = false;
                    if (currentDelver.favored && !wonCoinFlip)
                    {
                        wonBonusFlip = Random.value > 0.5f;
                        await FlipCoin(wonBonusFlip);
                    }

                    // double treasures
                    if (wonCoinFlip || wonBonusFlip)
                    {
                        await TreasureAdjustmentSingle(currentDelver, currentDelver.treasures);
                        break;
                    }
                    // lose all treasures
                    await TreasureAdjustmentSingle(currentDelver, -currentDelver.treasures);
                    break;

                }
            default:
                break;
        }
    }
}
