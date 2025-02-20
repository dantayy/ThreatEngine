using UnityEngine;
using UnityEngine.UI;
using TMPro;

// class holding stats for a card in our threat engine game
// should be attached to the Card prefab and set its visual attributes based on the associated fields in this script
// includes a pure virtual function to be implemented by children that will be called by the game manager to determine score changes for players that pick the card
public class CardScript : MonoBehaviour
{
    // ID of card, used to bind the prefab to a particular backend 
    [Header("ID")]
    [field: SerializeField] public int CardID { get; private set; } = 0;

    /**
    General Properties used for card text editing
    */
    [Header("Visual Elements")]
    // image used as background for the card
    [field: SerializeField] private Image CardBackground; // set with prefab child element of same name in editor
    [field: SerializeField] private Sprite CardBackgroundSprite;
    // image used as primary visual aid attached to card (not sure if we'll actually be using this going forward but its here for now)
    [field: SerializeField] private Image CardImage; // set with prefab child element of same name in editor
    [field: SerializeField] private Sprite CardImageSprite;
    // name of card (hand it belongs to? just a letter like in the docs?)
    [field: SerializeField] private TextMeshProUGUI CardTitle; // set with prefab child element of same name in editor
    [field: SerializeField] private string CardTitleText;
    // description of what selecting this card will do for all players that pick it
    [field: SerializeField] private TextMeshProUGUI CardEffect; // set with prefab child element of same name in editor
    [field: SerializeField, TextArea] private string CardEffectText;
    // description of what selecting this card will do for a threat that picks it
    [field: SerializeField] private TextMeshProUGUI ThreatEffect; // set with prefab child element of same name in editor
    [field: SerializeField, TextArea] private string ThreatEffectText;

    /**
    Properties tied to effects a player will take on by selecting this card
    */
    [Header("Player Effects")]
    // EXACT number of players that must select this card for the points to become the bonus value
    [field: SerializeField] public int SelectionTarget { get; private set; } = 0;
    // number of players that can select this card before the points become the default value
    [field: SerializeField] public int SelectionMax { get; private set; } = 0;
    // number of players that must select this card before the points become the bonus value
    [field: SerializeField] public int SelectionMin { get; private set; } = 0;
    // number of points a player will get from this card when a special condition has NOT been met
    [field: SerializeField] public int PointsDefault { get; private set; } = 0;
    // number of points a player will get from this card when a special condition HAS been met
    [field: SerializeField] public int PointsSpecial { get; private set; } = 0;
    // damage dealt to threat by this card when a special condition has NOT been met
    [field: SerializeField] public int DamageDefault { get; private set; } = 0;
    // damage dealt to threat by this card when a special condition HAS been met
    [field: SerializeField] public int DamageSpecial { get; private set; } = 0;
    // health healed to threat by this card when a special condition has NOT been met
    [field: SerializeField] public int HealDefault { get; private set; } = 0;
    // health healed to threat by this card when a special condition HAS been met
    [field: SerializeField] public int HealSpecial { get; private set; } = 0;
    // number of points a non-threat player will "gift" to the threat when a special condition has NOT been met
    [field: SerializeField] public int GiftDefault { get; private set; } = 0;
    // number of points a non-threat player will "gift" to the threat when a special condition HAS been met
    [field: SerializeField] public int GiftSpecial { get; private set; } = 0;
    // number of points a player will take away from one or more other players when a special condition has NOT been met
    [field: SerializeField] public int TaxDefault { get; private set; } = 0;
    // number of points a player will take away from one or more other players when a special condition HAS been met
    [field: SerializeField] public int TaxSpecial { get; private set; } = 0;
    // number of other players that will be targeted by this card's effect when a special condition has NOT been met
    [field: SerializeField] public int TargetDefault { get; private set; } = 0;
    // number of other players that will be targeted by this card's effect when a special condition HAS been met
    [field: SerializeField] public int TargetSpecial { get; private set; } = 0;
    
    /**
    Properties tied to extra effects that a threat will take on by selecting this card
    */
    [Header("Threat Effects")]
    // EXACT number of players that must select this card for the threat points to become the bonus value
    [field: SerializeField] public int ThreatSelectionTarget { get; private set; } = 0;
    // number of players that can select this card before the threat points become the default value
    [field: SerializeField] public int ThreatSelectionMax { get; private set; } = 0;
    // number of players that need to select this card before the threat points become the bonus value
    [field: SerializeField] public int ThreatSelectionMin { get; private set; } = 0;
    // number of EXTRA points a threat will get from this card when a special condition has NOT been met
    [field: SerializeField] public int ThreatPointsDefault { get; private set; } = 0;
    // number of EXTRA points a threat will get from this card when a special condition HAS been met
    [field: SerializeField] public int ThreatPointsSpecial { get; private set; } = 0;
    // EXTRA damage dealt to threat by themselves with this card when a special condition has NOT been met
    [field: SerializeField] public int ThreatDamageDefault { get; private set; } = 0;
    // EXTRA damage dealt to threat by themselves with this card when a special condition HAS been met
    [field: SerializeField] public int ThreatDamageSpecial { get; private set; } = 0;
    // EXTRA health healed to threat by themselves with this card when a special condition has NOT been met
    [field: SerializeField] public int ThreatHealDefault { get; private set; } = 0;
    // EXTRA health healed to threat by themselves with this card when a special condition HAS been met
    [field: SerializeField] public int ThreatHealSpecial { get; private set; } = 0;
    // EXTRA points a threat will take away from one or more other players when a special condition has NOT been met
    [field: SerializeField] public int ThreatTaxDefault { get; private set; } = 0;
    // EXTRA points a threat will take away from one or more other players when a special condition HAS been met
    [field: SerializeField] public int ThreatTaxSpecial { get; private set; } = 0;
    // number of other players that will be targeted by this card's threat effect when a special condition has NOT been met
    [field: SerializeField] public int ThreatTargetDefault { get; private set; } = 0;
    // number of other players that will be targeted by this card's threat effect when a special condition HAS been met
    [field: SerializeField] public int ThreatTargetSpecial { get; private set; } = 0;

    // manager will take players decisions in turn order
    // look at player set to be going first in the round
    // look at their points
    // look at their card choice
    // look at other players points
    // look at other players card choices
    // resolve card-specific conditionals, plugging in player points/choices when needed
    // how do we write code to manage a potentially expanding pool of cards with variable conditionals in an easily main?
    // map of scripts? key = hand name, value = set of lambdas, one for each card in the hand that runs the score/health work

    private void Awake()
    {
        CardBackground.sprite = CardBackgroundSprite;
        CardImage.sprite = CardImageSprite;
        CardTitle.text = CardTitleText;
        CardEffect.text = CardEffectText;
        ThreatEffect.text = ThreatEffectText;    
    }

    private void OnValidate()
    {
        Awake();
    }
}
