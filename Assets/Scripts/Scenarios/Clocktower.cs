using System;
using System.Collections.Generic;
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
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // flag set when any delver is favored this round
        bool favored = false;
        // flag set when we've completed our single check of delvers for favored status
        bool favoredChecked = false;
        // current time var
        DateTime currentTime;
        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // knock the bell
                case 0:
                    {
                        // only need to check all delvers once
                        if(!favoredChecked)
                        {
                            foreach (PlayerScript target in delversSortedScores)
                            {
                                // a delver is favored, set flag and break out
                                if(target.favored)
                                {
                                    favored = true;
                                    break;
                                }
                            }
                            favoredChecked = true;
                        }
                        // only give points if no delver is favored
                        if(!favored)
                        {
                            TreasureAdjustment(currentDelver, 20);
                        }
                        // add extra treasures if favored
                        else if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 5);
                        }
                        break;
                    }
                // align the gears to unlock a path
                case 1:
                    {
                        // grab current time
                        currentTime = DateTime.Now;
                        // calculate sum of digits in gear, then add to current delver's treasures
                        TreasureAdjustment(currentDelver, currentTime.Minute % 10 + ((currentTime.Minute - currentTime.Minute % 10) / 10));
                        // add extra treasures if favored
                        if(currentDelver.favored)
                        {
                            TreasureAdjustment(currentDelver, 1);
                        }
                        break;
                    }
            }

            // re-sort delver scores list
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));

            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}
