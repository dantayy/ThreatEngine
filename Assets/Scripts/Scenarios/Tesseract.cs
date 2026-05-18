using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tesseract", menuName = "Scriptable Objects/Tesseract")]
public class Tesseract : ScenarioScript
{

    // reference to the scenario that came before this one, to be set by the manager when this scenario is pulled
    public ScenarioScript prev;

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