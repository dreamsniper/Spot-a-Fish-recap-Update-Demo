using UnityEngine;
using System.Collections;
using UnityEngine.UI;


    public class ugui_gem_count : MonoBehaviour {

	public bool player;
	public Text[] my_text;
    public Image[] my_icons;
    public Board_C board;

	// Use this for initialization
	void Start () {

    }

    public void MyStart()
    {

        for (int n = 0; n < board.myRuleset.gem_length; n++)
            my_icons[n].sprite = board.myTheme.gem_colors[n];

    }

    // Update is called once per frame
    void Update () {

        if (!board.boardGenerated)
            return;

		for (int n = 0; n < board.myRuleset.gem_length ; n++)
		{
            if (player)
                {
                my_text[n].text = board.player.myCharacter.numberOfGemsCollect[n]
                + " / " + board.player.myCharacter.number_of_gems_to_destroy_to_win[n];
                }
            else
                {
                    my_text[n].text = board.enemy.myCharacter.numberOfGemsCollect[n]
                + " / " + board.enemy.myCharacter.number_of_gems_to_destroy_to_win[n];
                }

        }

        if (player)
            board.player.myUI.gui_progressBar_slider[2].value = board.player.myCharacter.totalNumberOfGemsRequiredColletted + board.player.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired;
        else
            board.enemy.myUI.gui_progressBar_slider[2].value = board.enemy.myCharacter.totalNumberOfGemsRequiredColletted + board.enemy.myCharacter.totalNumberOfExtraGemsCollettedAfterTheRequired;

    }
}
