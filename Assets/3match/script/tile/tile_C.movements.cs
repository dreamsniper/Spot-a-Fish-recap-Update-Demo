using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour {


    public void Update_gem_position()
    {
        board.gem_falling_ongoing = true;
        StartFalling();
    }


    public void DebugClick()
    {

            Debug.Log("debug click! " + _x + "," + _y + " " + myContent + " " + board.board_array_master[_x, _y, 13]);

        
    }

    public bool isFalling;
    public void Falling()//original animation code
    {

        //if (board.board_array_master[_x, _y, 11] != 333) //falling false
        //return;

        if (myContent == null)
        {
            board.fallingTiles.Remove(this);
            Debug.LogWarning(_x + "," + _y + "Falling(): myContent == null!!!!");
            return;
        }

        if (board.board_array_master[_x, _y, 11] == 6)
        {
            board.fallingTiles.Remove(this);
            Debug.LogWarning(_x + "," + _y + "Falling(): is swipe !!!!");
            return;
        }


        //myContent.transform.Translate(((transform.position - myContent.transform.position).normalized) * board.globalRules.falling_speed * Time.deltaTime, Space.World);
        //myContent.transform.Translate(((transform.position - myContent.transform.position).normalized) * board.globalRules.falling_speed * Time.smoothDeltaTime , Space.World);//move contebt

        float tempSpeed = (board.globalRules.falling_speed * Time.smoothDeltaTime);
        if (tempSpeed > 0.5f)
            tempSpeed = 0.5f;
        myContent.transform.Translate(((transform.position - myContent.transform.position).normalized) * tempSpeed, Space.World);

        isFalling = true;

        if (Vector3.Distance(transform.position, myContent.transform.position) <= board.globalRules.accuracy)
        {
            if (Time.timeSinceLevelLoad > board.audioManager.latest_sfx_time + 0.01f)
            {
                Play_sfx(board.audioManager.end_fall_sfx);
                board.audioManager.latest_sfx_time = Time.timeSinceLevelLoad;
            }

            if (Vector3.Distance(transform.position, myContent.transform.position) != 0)
                myContent.transform.position = transform.position;

            if (!ThereIsAnEmptyTileUnderMe())
                myContent.PlayAnimation(TileContent.CurrentAnimation.EndFall);

            isFalling = false;
            End_falling_gems();


        }
    }

    void StartFalling()
    {
        //Debug.Log("StartFalling() " + board.board_array_master[_x, _y, 11]);
       if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased || (  board.board_array_master[_x, _y, 11] >= 3 && board.board_array_master[_x, _y, 11] <= 5))//falling
        {
            //Debug.Log(" StartFalling " + _x + "," + _y + " = " + board.board_array_master[_x, _y, 11]);
            board.board_array_master[_x, _y, 11] = 333;//falling ongoing
            board.fallingTiles.Add(this);
        }
    }


    public void End_falling_gems()
    {
        if (board.board_array_master[_x, _y, 11] == 6)
        {
            Debug.LogError("End_falling_gems == 6");
            Debug.Break();

            
        }

       
        board.fallingTiles.Remove(this);
        myContent.ResetAvatarTranform();


        //reset status
        board.board_array_master[_x, _y, 11] = 0;
        board.board_array_master[_x, _y, 13] = 0;

        board.number_of_gems_to_move--;



        Check_if_gem_movements_are_all_done();
    }
    

    void Check_if_gem_movements_are_all_done()
    {
        if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.Realtime)
            return;

        if (board.number_of_gems_to_move + board.number_of_new_gems_to_create == 0)
        {
            board.gem_falling_ongoing = false;

            if (board.myRuleset.diagonal_falling)
            {
                board.Start_update_board();
            }
            else
            {
                board.Start_update_board();
                //board.Check_secondary_explosions();//OLD solution
            }

        }

    }

    public IEnumerator Switch_gem_animation()
    {
        print(_x + "," + _y + " Switch_gem_animation ");
        LeanTween.move(myContent.gameObject, transform.position, 0.25f);

        yield return new WaitForSeconds(0.25f);

        myContent.transform.position = transform.position;
        board.Execute_primary_explosions();


        /*
        while (Vector3.Distance(transform.position, myContent.transform.position) > board.globalRules.accuracy)
        {

            yield return new WaitForSeconds(0.015f);

            if (myContent == null)
            {
                Debug.LogWarning(_x + "," + _y + " Abort: Switch_gem_animation()");
                yield break;
            }

            myContent.transform.Translate(((transform.position - myContent.transform.position).normalized) * board.globalRules.switch_speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, myContent.transform.position) <= board.globalRules.accuracy)//when animation end
            {
                if (Vector3.Distance(transform.position, myContent.transform.position) != 0)
                    myContent.transform.position = transform.position;

                board.Execute_primary_explosions();
            }
        }
        */
    }



}
