using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Dunes", menuName = "Scriptable Objects/Dunes")]
public class Dunes : ScenarioScript
{
    public Dunes()
    {
        scenarioTitle = "The Dunes";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Cut through the valleys";
        actionEffects[(int)optionDirections.West] = "+1 for each delver who picked this. [Then +2 for each delver who did NOT pick this.]";
        
        actionTitles[(int)optionDirections.East] = "Dig for treasure";
        actionEffects[(int)optionDirections.East] = "+2 [Then +2 for each OTHER delver who picked this.]";

        actionTitles[(int)optionDirections.North] = "Summit the highest peak to identify hidden troves:";
        actionEffects[(int)optionDirections.North] = "+2 for each OTHER delver who picked this. [Then +6 IF ALL delvers picked this.]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // cut through
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, westCount);

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, delversSortedScores.Count - westCount);
                    }
                    break;
                }
            // dig
            case optionDirections.East:
                {
                    await TreasureAdjustmentSingle(currentDelver, 3);

                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, eastCount - 1);
                    }
                    break;
                }
            // summit
            case optionDirections.North:
                {
                    await TreasureAdjustmentSingle(currentDelver, 2 * (northCount - 1));

                    if (currentDelver.favored && northCount == delversSortedScores.Count)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 6);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
