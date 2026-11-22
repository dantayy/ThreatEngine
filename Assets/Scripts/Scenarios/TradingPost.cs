using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "TradingPost", menuName = "Scriptable Objects/TradingPost")]
public class TradingPost : ScenarioScript
{
    public TradingPost()
    {
        scenarioTitle = "The Trading Post";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Pillage";
        actionEffects[(int)optionDirections.West] = "+5 [Then +3]";

        actionTitles[(int)optionDirections.East] = "Trade";
        actionEffects[(int)optionDirections.East] = "Pick a delver. Swap points with them. [Then +4]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // pillage
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 5);

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // trade
            case optionDirections.East:
                {
                    await SwapTreasures(currentDelver, currentDelver.target);

                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 4);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
