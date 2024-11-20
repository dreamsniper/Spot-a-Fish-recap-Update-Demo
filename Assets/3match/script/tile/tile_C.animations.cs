using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour {


    #region Create
    public void CreationStart()// call from: Board_C.turnLoop.Update_board_by_one_step(), Board_C.realtimeLoop Generate_new_gems()
    {
        if (board.board_array_master[_x, _y, 11] == 6)
        {
            Debug.LogError("6");
        }


        board.board_array_master[_x, _y, 11] = 222;

        Decide_what_create();

        StartCoroutine(CreateAnimation());

    }

    public IEnumerator CreateAnimation()// call from: Board_C.Update_board_by_one_step(), Board_C.realtimeLoop Generate_new_gems()
    {
        myContent.PlayAnimation(TileContent.CurrentAnimation.Create);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());
        CreationEnd();
    }

    public void CreationEnd()
    {
        myContent.ResetAvatarTranform();
        board.number_of_new_gems_to_create--;
        //reset status

        if (board.board_array_master[_x, _y, 11] == 6)
            Debug.LogError("6");

        board.board_array_master[_x, _y, 11] = 0;
        board.board_array_master[_x, _y, 13] = 0;

        Check_if_gem_movements_are_all_done();
    }

    
    #endregion

    #region Create_falling_gem

    public IEnumerator Create_falling_gem()
    {
        yield return new WaitForSeconds(board.globalRules.animationDelay  /*0.03f*/);//= wait that the previous gem is fall before create a new gem

        Decide_what_create();

        //show animation
        myContent.PlayAnimation(TileContent.CurrentAnimation.Create);

        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());
        CreatingFallingGemEnd();

    }


    public void CreatingFallingGemEnd()
    {
        board.number_of_new_gems_to_create--;

        //look at the tile under me to see if this gem must fall
        if (ThereIsAnEmptyTileUnderMe())  //and it is empty
        {

            board.number_of_gems_to_move++;
            board.gem_falling_ongoing = true;
            Search_last_empty_tile_under_me();
            if (board.board_array_master[_x, _y, 1] == -99)//if this leader-tile is empty again
            {
                //repeat procedure
                StartCoroutine("Create_falling_gem");
            }


        }
        else //this gem don't fall
        {
            myContent.ResetAvatarTranform();
            if (board.number_of_new_gems_to_create == 0)
                Check_if_gem_movements_are_all_done();
        }
    }

    #endregion

    #region destroy

    void DestroyAnimationStart()
    {
        StartCoroutine(Destroy_animation());
    }

    IEnumerator Destroy_animation()
    {
        myContent.PlayAnimation(TileContent.CurrentAnimation.Destroy);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());
        DestroyAnimationEnd();
    }

    public void DestroyAnimationEnd()
    {
        //print(_x + "," + _y + " = " + "DestroyAnimationEnd");
        if (after_explosion_I_will_become_this_bonus == 0)
            Destroy_gem();
        else
            Change_gem_in_bonus();
    }

    #endregion

    public IEnumerator Return_to_normal_size()
    {
        //print(_x + "," + _y + " = " + "Return_to_normal_size()");
        //return to normal size
        myContent.PlayAnimation(TileContent.CurrentAnimation.ShuffleOut);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());
        myContent.ResetAvatarTranform();

        board.number_of_elements_to_damage--;
        Debug.Log(" ------ destruction is over " + _x + "," + _y);

        if (board.board_array_master[_x, _y, 11] == 6)
            Debug.LogError("6");

        board.board_array_master[_x, _y, 11] = 0;//destruction is over
        If_all_explosion_are_completed();

    }

    public IEnumerator Show_token(bool tokenGenerateByAbortingAnExplosion)
    {
        //print("Show_token " + tokenGenerateByAbortingAnExplosion);

        //minimize gem
        myContent.PlayAnimation(TileContent.CurrentAnimation.ShuffleIn);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());

        //update gem
        myContent.mySpriteRenderer.sprite = board.myTheme.token;
        board.number_of_token_on_board++;

        //return to normal size
        myContent.PlayAnimation(TileContent.CurrentAnimation.ShuffleOut);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());

        if (myContent)
            myContent.ResetAvatarTranform();

        if (tokenGenerateByAbortingAnExplosion)
        {
            board.number_of_elements_to_damage--;
            If_all_explosion_are_completed();
        }

    }

}
