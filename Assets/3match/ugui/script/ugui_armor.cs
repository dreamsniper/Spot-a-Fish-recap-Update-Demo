using UnityEngine;
using System.Collections;
using  UnityEngine.UI;


    public class ugui_armor : MonoBehaviour {

	public bool player;
	public Text[] my_text;
    public Image[] my_icons;
    public Board_C board;


	// Use this for initialization
	public void MyStart () {
                 
		for (int n = 0; n < board.myRuleset.gem_length ; n++)
		{
			if (player)
				my_text[n].text = board.player.myCharacter.armor[n].ToString() ;
			else
				my_text[n].text = board.enemy.myCharacter.armor[n].ToString();

            my_icons[n].sprite = board.myTheme.gem_colors[n];

        }

	}
	

}

