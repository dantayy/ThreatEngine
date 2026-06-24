using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tesseract", menuName = "Scriptable Objects/Tesseract")]
public class Tesseract : ScenarioScript
{
    public Tesseract()
    {
        scenarioTitle = "The Tesseract";
        
        actionTitles.Add("Step forward in space, backward in time: ");

        actionEffects.Add("Play the previous scenario again.");

        lateGame = true;
    }

    protected override void ActionResolutions(List<PlayerScript> delversSortedScores, PlayerScript firstDelver)
    {
        // nothing happens in the tesseract!
        return;
    }
}