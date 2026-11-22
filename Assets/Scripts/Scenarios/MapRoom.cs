using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "MapRoom", menuName = "Scriptable Objects/MapRoom")]
public class MapRoom : ScenarioScript
{
    public MapRoom()
    {
        scenarioTitle = "The Map Room";
        SetActionCount(3);

        actionTitles[(int)optionDirections.West] = "Take a map";
        actionEffects[(int)optionDirections.West] = "+3 [Then +3]";
        
        actionTitles[(int)optionDirections.East] = "Take the treasure that was sitting behind the maps";
        actionEffects[(int)optionDirections.East] = "+2 for each delver who didn't pick this. [Then +3]";
        
        actionTitles[(int)optionDirections.North] = "Follow the spirit's favored to more treasure";
        actionEffects[(int)optionDirections.North] = "+8 IF there is a spirit's favored, they already took their turn, AND they didn't pick this. [Then +1]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // ignore
            case optionDirections.West:
                {
                    await TreasureAdjustmentSingle(currentDelver, 3);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }
                    break;
                }
            // topple
            case optionDirections.East:
                {
                    await TreasureAdjustmentSingle(currentDelver, delversSortedScores.Count - eastCount);

                    if (currentDelver.favored)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                    }

                    break;
                }
            // read
            case optionDirections.North:
                {
                    if (aFavoredDelverExists)
                    {
                        PlayerScript favoredDelver = delversSortedScores.Find(x => x.favored);

                        // determine if the favored delver didn't pick this option and already took their turn
                        if (favoredDelver.actionIdx != optionDirections.North && delversInTurnOrder.IndexOf(favoredDelver) < delversInTurnOrder.IndexOf(currentDelver))
                        {
                            await TreasureAdjustmentSingle(currentDelver, 8);
                        }
                    }

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
