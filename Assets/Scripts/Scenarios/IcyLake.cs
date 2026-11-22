using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "IcyLake", menuName = "Scriptable Objects/IcyLake")]
public class IcyLake : ScenarioScript
{
    public IcyLake()
    {
        scenarioTitle = "The Icy Lake";
        SetActionCount(4);

        actionTitles[(int)optionDirections.West] = "Go fishing";
        actionEffects[(int)optionDirections.West] = "+5 IF you are in last place. [Then +4]";
        
        actionTitles[(int)optionDirections.East] = "Search around the shoreline";
        actionEffects[(int)optionDirections.East] = "+8 IF no single action has been picked three or more times. [Then +1]";

        actionTitles[(int)optionDirections.North] = "Venture onto thin ice";
        actionEffects[(int)optionDirections.North] = "+5 MINUS 1 for each OTHER delver who picked this. [Then +2]";

        actionTitles[(int)optionDirections.South] = "Stay on thick ice";
        actionEffects[(int)optionDirections.South] = "+2 for each delver who picked this. [Then +1] ";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // fishing
            case optionDirections.West:
                {
                    if (currentDelver.delverID == delversSortedScores[delversSortedScores.Count - 1].delverID)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 5);
                    }

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 4);
                    }
                    break;
                }
            // shoreline
            case optionDirections.East:
                {
                    if (westCount < 3 && eastCount < 3 && northCount < 3 && southCount < 3)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 8);
                    }

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // thin ice
            case optionDirections.North:
                {
                    await TreasureAdjustmentSingle(currentDelver, 5 - (northCount - 1));

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // thick ice
            case optionDirections.South:
                {
                    await TreasureAdjustmentSingle(currentDelver, 2 * southCount);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
