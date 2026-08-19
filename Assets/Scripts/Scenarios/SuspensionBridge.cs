using System.Collections.Generic;
using System.Threading.Tasks;
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

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // walk on the left
            case 0:
                {
                    // treasures for every delver going on the other side
                    await TreasureAdjustment(currentDelver, bCount);
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        // extra bonus if everyone else went right
                        if(aCount == 1)
                        {
                            await TreasureAdjustment(currentDelver, 4);
                        }
                        // default bonus
                        else
                        {
                            await TreasureAdjustment(currentDelver, 1);
                        }
                    }
                    break;
                }
            // walk on the right
            case 1:
                {
                    // treasures for every delver going on the other side
                    await TreasureAdjustment(currentDelver, aCount);
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        // extra bonus if everyone else went left
                        if(bCount == 1)
                        {
                            await TreasureAdjustment(currentDelver, 4);
                        }
                        // default bonus
                        else
                        {
                            await TreasureAdjustment(currentDelver, 1);
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
    }
}