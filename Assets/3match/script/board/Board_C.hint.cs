using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour
{

    bool use_hint;

    void Show_hint()//call from Annotate_potential_moves()
    {
        if (!player_can_move)
            return;

        if (use_hint)
        {
            if (number_of_moves_possible > 0)//show a gem move
            {
                //Debug.Log("move hint");
                int random_hint = UnityEngine.Random.Range(0, number_of_gems_moveable - 1);

                globalRules.my_hint.position = script_tiles_array[list_of_moves_possible[random_hint, 1], list_of_moves_possible[random_hint, 2]].transform.position;
                globalRules.my_hint.GetComponent<Animation>().Play("hint_anim");
                globalRules.my_hint.gameObject.SetActive(true);
                for (int i = 4; i <= 7; i++)
                {
                    if (list_of_moves_possible[random_hint, i] > 0)
                    {
                        globalRules.my_hint.GetChild(i - 4).gameObject.SetActive(true);
                    }
                    else
                        globalRules.my_hint.GetChild(i - 4).gameObject.SetActive(false);
                }
            }
            else //show a clickable bonus
            {
                if (number_of_bonus_on_board > 0)
                {
                    Locate_all_bonus_on_board();
                    int random_temp = UnityEngine.Random.Range(0, number_of_bonus_on_board - 1);
                    globalRules.my_hint.position = new Vector3(bonus_coordinate[random_temp].x, -bonus_coordinate[random_temp].y, globalRules.my_hint.position.z);
                    globalRules.my_hint.GetComponent<Animation>().Play("hint_anim_click_here");
                    globalRules.my_hint.gameObject.SetActive(true);

                    for (int i = 0; i < 4; i++)
                    {
                        globalRules.my_hint.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                    {
                    print("no hint avaible on board");
                    use_hint = false;
                    if (shuffle_ongoing)
                        Invoke("Show_hint", globalRules.show_hint_after_n_seconds);
                    }
            }

        }
    }

    void Cancell_hint()
    {
        CancelInvoke("Show_hint");
        use_hint = false;
        globalRules.my_hint.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            globalRules.my_hint.GetChild(i).gameObject.SetActive(false);
        }
    }


    void Check_if_show_hint()//call from Board_C.realtimeLoop.BoardUpdate()
    {
        if (globalRules.show_hint)
        {
            if (use_hint)
            {
                if ((number_of_elements_to_damage_with_SwitchingGems + number_of_elements_to_damage) > 0)
                    Cancell_hint();
            }
            else
            {
                use_hint = true;
                Invoke("Show_hint", globalRules.show_hint_after_n_seconds);
            }
        }
    }
}
