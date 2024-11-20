using UnityEngine;
using System.Collections;

public partial class Board_C : MonoBehaviour{

    //public float timer;
    [HideInInspector] public float time_left;
    [HideInInspector] public float time_bonus;
    [HideInInspector] public bool stage_started;
    [HideInInspector] public float start_time;
    //float[] starTimeGoals;

    void ResetTimer()
    {
        time_left = 0;
        time_bonus = 0;
        stage_started = false;
        start_time = 0;
    }

    void Timer()//call from update
    {

        if (stage_started && (game_end != true))
        {
            if (time_bonus < 0)
                Debug.LogError(time_bonus);
            

            time_left = (myRuleset.timer + start_time + time_bonus) - Time.timeSinceLevelLoad;

            uIManager.gui_timer_slider.value = time_left;
            if (time_left <= 0)
            {
                time_left = 0;
                Player_lose();
            }
            else if (time_left > myRuleset.timer)
                time_left = myRuleset.timer;
        }
    }

    public void Add_time_bonus(float add_this)// call from Check_secondary_explosions(), tile_C.Check_if_shuffle_is_done(), tile_C.Check_if_gem_movements_are_all_done()
    {
        
        if ((time_left + add_this) > myRuleset.timer)
        {
            time_bonus += myRuleset.timer - time_left;
        }
        else
        {
            time_bonus += add_this;
        }

    }


}
