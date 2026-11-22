using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Tesseract", menuName = "Scriptable Objects/Tesseract")]
public class Tesseract : ScenarioScript
{
    public Tesseract()
    {
        scenarioTitle = "The Tesseract";
        SetActionCount(1);

        actionTitles[(int)optionDirections.West] = "Step forward in space, backward in time: ";
        actionEffects[(int)optionDirections.West] = "Play the previous scenario again.";
    }

    protected override async Task ActionResolutions(List<PlayerScript> delversSortedScores)
    {
        // nothing happens in the tesseract!
        await Task.CompletedTask;
    }
}
