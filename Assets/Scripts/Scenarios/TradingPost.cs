using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "TradingPost", menuName = "Scriptable Objects/TradingPost")]
public class TradingPost : ScenarioScript
{
    public TradingPost()
    {
        scenarioTitle = "The Trading Post";

        actionTitles.Add("Pillage");
        actionTitles.Add("Trade");

        actionEffects.Add("+5 [+3]");
        actionEffects.Add("Pick a delver. Swap points with them. [Then +4]");
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // pillage
            case 0:
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
            case 1:
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