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

        actionTitles.Add("Knock the Bell");
        actionTitles.Add("Align the Gears to Unlock a Path");

        actionEffects.Add("+20 IF no one carries the spirit's favor. [+5]");
        actionEffects.Add("+ points equal to the sum of digits in the minute portion of the time when the player resolves their turn. [+1]");

        lateGame = true;
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // knock the bell
            case 0:
                {
                    // only give points if no delver is favored
                    if(!delverFavored)
                    {
                        await TreasureAdjustment(currentDelver, 20);
                    }
                    // add extra treasures if favored
                    else if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 5);
                    }
                    break;
                }
            // align the gears to unlock a path
            case 1:
                {
                    // grab current time
                    currentTime = DateTime.Now;
                    // calculate sum of digits in gear, then add to current delver's treasures
                    await TreasureAdjustment(currentDelver, currentTime.Minute % 10 + ((currentTime.Minute - currentTime.Minute % 10) / 10));
                    // add extra treasures if favored
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 1);
                    }
                    break;
                }
        }

        // re-sort delver scores list
        delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));

        // move to next delver in turn order
        currentDelver = currentDelver.rightDelver;
    }
}
