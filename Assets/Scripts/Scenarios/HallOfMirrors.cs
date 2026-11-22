using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "HallOfMirrors", menuName = "Scriptable Objects/HallOfMirrors")]
public class HallOfMirrors : ScenarioScript
{
    public HallOfMirrors()
    {
        scenarioTitle = "The Hall Of Mirrors";
        SetActionCount(4);

        // West
        actionTitles[(int)optionDirections.West] = "Listen for the spirit's guidance";
        actionEffects[(int)optionDirections.West] = "+1 IF only you pick this [Then +3]";

        // East
        actionTitles[(int)optionDirections.East] = "Take your time";
        actionEffects[(int)optionDirections.East] = "+2 IF only you pick this [Then +2]";

        // North
        actionTitles[(int)optionDirections.North] = "Look for a trail to follow";
        actionEffects[(int)optionDirections.North] = "+3 IF only you pick this [Then +1]";

        // South
        actionTitles[(int)optionDirections.South] = "Cannonball run";
        actionEffects[(int)optionDirections.South] = "+4 IF only you pick this [Then +0]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // listen for the spirit's guidance
            case optionDirections.West:
                {
                    // give treasures if exclusive
                    if(westCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // take your time
            case optionDirections.East:
                {
                    // give treasures if exclusive
                    if(eastCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 2);
                    }
                    break;
                }
            // look for a trail to follow
            case optionDirections.North:
                {
                    // give treasures if exclusive
                    if(northCount == 1)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // cannonball run
            case optionDirections.South:
                {
                    // give treasures if exclusive
                    if(southCount == 1)
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
