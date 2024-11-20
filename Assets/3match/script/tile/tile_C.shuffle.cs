using UnityEngine;
using System.Collections;

public partial class tile_C : MonoBehaviour
{

    public IEnumerator Shuffle_update()//call from Board_C.Shuffle(), Gems_teleport()
    {
        //minimize gem
        myContent.PlayAnimation(TileContent.CurrentAnimation.ShuffleIn);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());

        //update gem
        myContent.mySpriteRenderer.sprite = board.myTheme.gem_colors[board.board_array_master[_x, _y, 1]];

        //return to normal size
        myContent.PlayAnimation(TileContent.CurrentAnimation.ShuffleOut);
        yield return new WaitForSeconds(myContent.GetCurrentAnimationDuration());

        myContent.ResetAvatarTranform();
        Check_if_shuffle_is_done();
    }

    void Check_if_shuffle_is_done()
    {
        board.number_of_gems_to_mix--;
        if (board.number_of_gems_to_mix == 0)
        {
            board.shuffle_ongoing = false;
            if (board.bonus_select != Bonus.SwitchGemTeleport)
            {
                if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                    board.Check_ALL_possible_moves();
            }
            else
            {
                board.uIManager.Reset_charge_fill();
                board.uIManager.Update_inventory_bonus(2, -1);
                if (board.myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
                    board.Check_secondary_explosions();
            }
        }
    }
}
