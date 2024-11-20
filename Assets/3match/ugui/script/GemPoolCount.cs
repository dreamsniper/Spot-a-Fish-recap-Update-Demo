using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemPoolCount : MonoBehaviour {

    public Image[] countIcons;
    public Text[] countText;
    Board_C board;
    bool player;

	public void StartMe (Board_C _board, bool _player) {

        board = _board;
        player = _player;

        for (int i = 0; i < countText.Length; i++)
        {
            countIcons[i].sprite = board.myTheme.gem_colors[i];

            if (i >= board.myRuleset.gem_length)
                countText[i].transform.parent.gameObject.SetActive(false);

        }

        UpdateMe();

    }

    public void ShowOnlyUsefulIcons(bool[] showThisIcon)
    {
        for (int i = 0; i < countIcons.Length; i++)
        {
            countIcons[i].gameObject.SetActive(showThisIcon[i]);
        }
    }

    public void UpdateMe() {

        if (player)
            {
            for (int i = 0; i < board.myRuleset.gem_length; i++)
                countText[i].text = board.player.myCharacter.gemColorAdvancedChargeBonusPool[i].ToString();
            }
        else
            {
            for (int i = 0; i < board.myRuleset.gem_length; i++)
                countText[i].text = board.enemy.myCharacter.gemColorAdvancedChargeBonusPool[i].ToString();
            }

    }
}
