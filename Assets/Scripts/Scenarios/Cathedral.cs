using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Cathedral", menuName = "Scriptable Objects/Cathedral")]
public class Cathedral : ScenarioScript
{
    public Cathedral()
    {
        scenarioTitle = "The Cathedral";
        
        actionTitles.Add("Pray alone");
        actionTitles.Add("Take from the offering bin");
        actionTitles.Add("Break the marble effigy together");

        actionEffects.Add("+5 [+3] IF only you choose this");
        actionEffects.Add("+1 [+2]");
        actionEffects.Add("+4 [+2] IF everyone chooses this");

        earlyGame = true;
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // pray alone
            case 0:
                {
                    // this delver IS the only one praying, dole out the treasures
                    if(aCount == 1)
                    {
                        // add to delver's treasures
                        await TreasureAdjustment(currentDelver, 5);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            await TreasureAdjustment(currentDelver, 3);
                        }
                    }
                    break;
                }
            // take from the offering bin
            case 1:
                {
                    // add to delver's treasures
                    await TreasureAdjustment(currentDelver, 1);
                    // favored bonus
                    if (currentDelver.favored)
                    {
                        await TreasureAdjustment(currentDelver, 2);
                    }
                    break;
                }
            // Break the marble effigy together
            case 2:
                {
                    // EVERYONE is working to break the effigy together, dole out treasures
                    if(cCount == delversSortedScores.Count)
                    {
                        // add to delver's treasures
                        await TreasureAdjustment(currentDelver, 4);
                        // favored bonus
                        if (currentDelver.favored)
                        {
                            // add to delver's treasures
                            await TreasureAdjustment(currentDelver, 2);
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