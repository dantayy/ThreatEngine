using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Hive", menuName = "Scriptable Objects/Hive")]
public class Hive : ScenarioScript
{
    public Hive()
    {
        scenarioTitle = "The Hive";
        
        actionTitles.Add("Sneak left");
        actionTitles.Add("Sneak down the middle");
        actionTitles.Add("Sneak right");

        actionEffects.Add("+2 IF B AND C are also picked. [+1]");
        actionEffects.Add("+2 IF A AND C are also picked [+1]");
        actionEffects.Add("+2 IF A AND B are also picked [+1]");
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // sneak left
            case 0:
                {
                    // give treasures if others went down the other paths
                    if(bCount > 0 && cCount > 0)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 1);
                    }
                    break;
                }
            // sneak down the middle
            case 1:
                {
                    // give treasures if others went down the other paths
                    if(aCount > 0 && cCount > 0)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 1);
                    }
                    break;
                }
            // sneak right
            case 2:
                {
                    // give treasures if others went down the other paths
                    if(aCount > 0 && bCount > 0)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    // favored bonus
                    if(currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 1);
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