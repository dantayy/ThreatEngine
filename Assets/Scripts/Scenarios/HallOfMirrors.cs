using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "HallOfMirrors", menuName = "Scriptable Objects/HallOfMirrors")]
public class HallOfMirrors : ScenarioScript
{
    public HallOfMirrors()
    {
        scenarioTitle = "The Hall Of Mirrors";
        
        actionTitles.Add("Listen for the spirit's guidance");
        actionTitles.Add("Take your time");
        actionTitles.Add("Look for a trail to follow");
        actionTitles.Add("Cannonball run");

        actionEffects.Add("+1 IF only you pick this [+3]");
        actionEffects.Add("+2 IF only you pick this [+2]");
        actionEffects.Add("+3 IF only you pick this [+1]");
        actionEffects.Add("+4 IF only you pick this [+0]");

        earlyGame = true;
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // listen for the spirit's guidance
            case 0:
                {
                    // give treasures if exclusive
                    if(aCount == 1)
                    {
                        await TreasureAdjustment(currentDelver, 1);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 3);
                    }
                    break;
                }
            // take your time
            case 1:
                {
                    // give treasures if exclusive
                    if(bCount == 1)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    break;
                }
            // look for a trail to follow
            case 2:
                {
                    // give treasures if exclusive
                    if(cCount == 1)
                    {
                        await TreasureAdjustment(currentDelver, 3);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 1);
                    }
                    break;
                }
            // cannonball run
            case 3:
                {
                    // give treasures if exclusive
                    if(dCount == 1)
                    {
                        await TreasureAdjustment(currentDelver, 4);
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