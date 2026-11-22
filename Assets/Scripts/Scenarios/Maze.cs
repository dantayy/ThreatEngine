using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Maze", menuName = "Scriptable Objects/Maze")]
public class Maze : ScenarioScript
{
    public Maze()
    {
        scenarioTitle = "The Maze";
        SetActionCount(2);

        actionTitles[(int)optionDirections.West] = "Go left";
        actionEffects[(int)optionDirections.West] = "+3 IF the delver on your LEFT also picked this. [Then +1 for each adjacent delver who also picked this.]";
        
        actionTitles[(int)optionDirections.East] = "Go right";
        actionEffects[(int)optionDirections.East] = "+3 IF the delver on your RIGHT also picked this. [Then +1 for each adjacent delver who also picked this.]";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // handle each possible action choice
        switch (currentDelver.actionIdx)
        {
            // left
            case optionDirections.West:
                {
                    int adjacentCount = 0;

                    if(currentDelver.leftDelver.actionIdx == optionDirections.West)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                        adjacentCount++;
                    }

                    if(currentDelver.favored)
                    {
                        if(currentDelver.rightDelver.actionIdx == optionDirections.West)
                        {
                            adjacentCount++;
                        }
                        await TreasureAdjustmentSingle(currentDelver, adjacentCount);
                    }
                    break;
                }
            // right
            case optionDirections.East:
                {
                    int adjacentCount = 0;

                    if (currentDelver.rightDelver.actionIdx == optionDirections.East)
                    {
                        await TreasureAdjustmentSingle(currentDelver, 3);
                        adjacentCount++;
                    }

                    if (currentDelver.favored)
                    {
                        if (currentDelver.leftDelver.actionIdx == optionDirections.East)
                        {
                            adjacentCount++;
                        }
                        await TreasureAdjustmentSingle(currentDelver, adjacentCount);
                    }
                    break;
                }
            default:
                break;
        }
    }
}
