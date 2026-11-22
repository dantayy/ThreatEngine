using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Kaleidoscope", menuName = "Scriptable Objects/Kaleidoscope")]
public class Kaleidoscope : ScenarioScript
{
    public Kaleidoscope()
    {
        scenarioTitle = "The Kaleidoscope";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Ignore the illusions";
        actionEffects[(int)optionDirections.West] = "+1 [Then +1 for every OTHER delver who picks this.]";
        
        actionTitles[(int)optionDirections.East] = "Attempt to break down the walls";
        actionEffects[(int)optionDirections.East] = "+3 [then +2] IF exactly three delvers pick this.";
        
        actionTitles[(int)optionDirections.North] = "Uncover the source of the projections together";
        actionEffects[(int)optionDirections.North] = "+2 [then +2] IF exactly two delvers pick this.";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // ignore
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 1);

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, westCount - 1);
                    }
                    break;
                }
            // break down
            case optionDirections.East:
                {
                    if(eastCount == 3)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);

                        if (currentDelver.favored)
                        {
                            await TreasureAdjustmentSingle(currentDelver, 2);
                        }
                    }
                    break;
                }
            // uncover
            case optionDirections.North:
                {
                    if (northCount == 2)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);

                        if (currentDelver.favored)
                        {
                            await TreasureAdjustmentSingle(currentDelver, 2);
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }
}
