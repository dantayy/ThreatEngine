using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SuspensionBridge", menuName = "Scriptable Objects/SuspensionBridge")]
public class SuspensionBridge : ScenarioScript
{
    public SuspensionBridge()
    {
        scenarioTitle = "The Suspension Bridge";
        
        actionTitles.Add("Walk on the left");
        actionTitles.Add("Walk on the right");

        actionEffects.Add("+1 per delver who picked B [+1 OR +4 IF every other delver picks B]");
        actionEffects.Add("+1 per delver who picked A [+1 OR +4 IF every other delver picks A]");

        earlyGame = true;
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // vars for tracking how many delvers went each way
        int leftCount = 0;
        int rightCount = 0;

        // perform initial pass of delver choices to determine how many went left and how many went right
        foreach(PlayerScript delver in delversSortedScores)
        {
            if(delver.actionIdx == 0)
            {
                leftCount++;
            }
            else if(delver.actionIdx == 1)
            {
                rightCount++;
            }
        }

        // look at each delver's action choice
        PlayerScript currentDelver = firstDelver;
        do
        {
            // handle each possible action choice
            switch (currentDelver.actionIdx)
            {
                // walk on the left
                case 0:
                    {
                        // treasures for every delver going on the other side
                        TreasureAdjustment(currentDelver, rightCount);
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            // extra bonus if everyone else went right
                            if(leftCount == 1)
                            {
                                TreasureAdjustment(currentDelver, 4);
                            }
                            // default bonus
                            else
                            {
                                TreasureAdjustment(currentDelver, 1);
                            }
                        }
                        break;
                    }
                // walk on the right
                case 1:
                    {
                        // treasures for every delver going on the other side
                        TreasureAdjustment(currentDelver, leftCount);
                        // favored bonus
                        if(currentDelver.favored)
                        {
                            // extra bonus if everyone else went left
                            if(rightCount == 1)
                            {
                                TreasureAdjustment(currentDelver, 4);
                            }
                            // default bonus
                            else
                            {
                                TreasureAdjustment(currentDelver, 1);
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            // re-sort delver scores list
            delversSortedScores.Sort((a,b) => a.treasures.CompareTo(b.treasures));

            // move to next delver in turn order
            currentDelver = currentDelver.rightDelver;
        } while (currentDelver != firstDelver);
    }
}