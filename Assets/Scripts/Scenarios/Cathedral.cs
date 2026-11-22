using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Cathedral", menuName = "Scriptable Objects/Cathedral")]
public class Cathedral : ScenarioScript
{
    public Cathedral()
    {
        scenarioTitle = "The Cathedral";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Pray alone";
        actionEffects[(int)optionDirections.West] = "+5 [+3] IF only you choose this";
        
        actionTitles[(int)optionDirections.East] = "Take from the offering bin";
        actionEffects[(int)optionDirections.East] = "+1 [+2]";

        actionTitles[(int)optionDirections.North] = "Break the marble effigy together";
        actionEffects[(int)optionDirections.North] = "+4 [+2] IF everyone chooses this";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // take from the offering bin
            case optionDirections.West:
                {
                    // add to delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, 1);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // pray alone
            case optionDirections.East:
                {
                    // this delver IS the only one praying, dole out the treasures
                    if(westCount == 1)
                    {
                        // add to delver's treasures
                        await TreasureAdjustmentSingle(currentDelver, 5);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            await TreasureAdjustmentSingle(currentDelver, 3);
                        }
                    }
                    break;
                }
            // Break the marble effigy together
            case optionDirections.North:
                {
                    // EVERYONE is working to break the effigy together, dole out treasures
                    if(northCount == delversSortedScores.Count)
                    {
                        // add to delver's treasures
                        await TreasureAdjustmentSingle(currentDelver, 4);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
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
