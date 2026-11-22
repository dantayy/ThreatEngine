using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AuctionHouse", menuName = "Scriptable Objects/AuctionHouse")]
public class AuctionHouse : ScenarioScript
{
    public AuctionHouse()
    {
        scenarioTitle = "The Casino";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Sell";
        actionEffects[(int)optionDirections.West] = "+3 [Also Pick a delver. They get -3.]";

        actionTitles[(int)optionDirections.East] = "Buy";
        actionEffects[(int)optionDirections.East] = "-2 [to all delvers.]";

        actionTitles[(int)optionDirections.North] = "Bargain";
        actionEffects[(int)optionDirections.North] = "Gain/lose 2x [3x instead] the treasures that the previous delver has gained/lost from this locations's actions. (0 points if going first)";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // sell
            case optionDirections.West:
                {
                    // add to delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, 3);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        // remove target delver's treasures
                        await TreasureAdjustmentSingle(currentDelver.target, -3);
                    }
                    break;
                }
            // buy
            case optionDirections.East:
                {
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustments(delversSortedScores, -2);
                        break;
                    }
                    await TreasureAdjustmentSingle(currentDelver, -2);
                    break;
                }
            // bargain
            case optionDirections.North:
                {
                    int multiplier = 2;

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        multiplier = 3;
                    }

                    // if going first, nothing happens.
                    if(currentDelver == delverGoingFirst)
                    {
                        break;
                    }

                    int previousDelverChange = 0;

                    // if this is the first round, compare against the starting treasure count. (Hardcoded 10)
                    if(gameStateHistory.Count == 0)
                    {
                        previousDelverChange = currentDelver.leftDelver.treasures - 10;
                    }
                    // otherise, compare the delver's score from last round with their current treasures.
                    else if(gameStateHistory[gameStateHistory.Count - 1].delverScores.TryGetValue(currentDelver.leftDelver, out int initialTreasures))
                    {
                        previousDelverChange = currentDelver.leftDelver.treasures - initialTreasures;
                    }
                    else
                    {
                        Debug.Log("Unable to find previous delver in game history.");
                    }
                    // gain/lose a multiple of the previous delver's treasure gain/loss.
                    await TreasureAdjustmentSingle(currentDelver, previousDelverChange * multiplier);

                    break;
                }
            default:
                break;
        }
    }
}
