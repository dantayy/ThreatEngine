using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Clocktower", menuName = "Scriptable Objects/Clocktower")]
public class Clocktower : ScenarioScript
{
    public Clocktower()
    {
        scenarioTitle = "The Clocktower";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Knock the Bell";
        actionEffects[(int)optionDirections.West] = "+20 IF no one carries the spirit's favor. [Then +5]";

        actionTitles[(int)optionDirections.East] = "Align the Gears to Unlock a Path";
        actionEffects[(int)optionDirections.East] = "+treasures equal to the sum of digits in the minute portion of the time when the player resolves their turn. [Then +1]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // align the gears to unlock a path
            case optionDirections.West:
                {
                    // grab current time
                    currentTime = DateTime.Now;
                    // calculate sum of the two minute digits, then add to current delver's treasures
                    await TreasureAdjustmentSingle(currentDelver, currentTime.Minute % 10 + ((currentTime.Minute - currentTime.Minute % 10) / 10));
                    // add extra treasures if favored
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 1);
                    }
                    break;
                }
            // knock the bell
            case optionDirections.East:
                {
                    // only give points if no delver is favored
                    if(!aFavoredDelverExists)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 20);
                    }
                    // add extra treasures if favored
                    else if(currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 5);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
