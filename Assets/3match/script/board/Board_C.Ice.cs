using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public partial class Board_C : MonoBehaviour {

    [Header("Ice grow")]
    [SerializeField] int growEachTurns;
    int iceGrowturnCount;
    [SerializeField] float iceGrowEachSeconds;
    float iceGrowTargetTime;
    [Range(1, 10)]
    [SerializeField] int iceElementActiveAtEachGrow = 1;


    void UpdateTurnBasedIceLoop()
    {
        if (current_turn > 0)
        {
            if (iceGrowturnCount >= growEachTurns)
            {
                iceGrowturnCount = 1;
                IceGrow();
            }
            else
                iceGrowturnCount++;
        }
    }

    void UpdateRealtimeIceLoop()
    {
        if (iceGrowTargetTime == 0)
            iceGrowTargetTime = Time.timeSinceLevelLoad + iceGrowEachSeconds;

        if (Time.timeSinceLevelLoad >= iceGrowTargetTime)
        {
            iceGrowTargetTime = Time.timeSinceLevelLoad + iceGrowEachSeconds;
            IceGrow();
        }
    }

    void IceGrow()
    {

        //find all ices
        List<Vector2Int> icePositions = new List<Vector2Int>();

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 3] == 2)
                    icePositions.Add(new Vector2Int(x, y));
            }
        }

        if (icePositions.Count < 1)//if there is no ice, stop here
            return;


        while (iceElementActiveAtEachGrow < icePositions.Count)
        {
            icePositions.RemoveAt(Random.Range(0, icePositions.Count));
        }



        int randomStartPoint = Random.Range(0, icePositions.Count);
        for (int i = randomStartPoint; i < icePositions.Count; i++)
        {
            TryToGrowIce(icePositions[i]);
        }

        for (int i = 0; i < randomStartPoint; i++)
        {
            TryToGrowIce(icePositions[i]);
        }
    }

    void TryToGrowIce(Vector2Int icePosition)
    {
        //find potential grow location
        List<Vector2Int> avaiblePositions = new List<Vector2Int>();

        Vector2Int testPosition = Vector2Int.zero;


        //up
        testPosition = new Vector2Int(icePosition.x, icePosition.y - 1);
        if (testPosition.y > 0)
        {
            if (ThisGemCanFroze(testPosition.x, testPosition.y, board_array_master[icePosition.x, icePosition.y, 15]))
                avaiblePositions.Add(testPosition);
        }

        //down
        testPosition = new Vector2Int(icePosition.x, icePosition.y + 1);
        if (testPosition.y < _Y_tiles)
        {
            if (ThisGemCanFroze(testPosition.x, testPosition.y, board_array_master[icePosition.x, icePosition.y, 15]))
                avaiblePositions.Add(testPosition);
        }

        //left
        testPosition = new Vector2Int(icePosition.x - 1, icePosition.y);
        if (testPosition.x > 0)
        {
            if (ThisGemCanFroze(testPosition.x, testPosition.y, board_array_master[icePosition.x, icePosition.y, 15]))
                avaiblePositions.Add(testPosition);
        }


        //right
        testPosition = new Vector2Int(icePosition.x + 1, icePosition.y);
        if (testPosition.x < _X_tiles)
        {
            if (ThisGemCanFroze(testPosition.x, testPosition.y, board_array_master[icePosition.x, icePosition.y, 15]))
                avaiblePositions.Add(testPosition);
        }


        if (avaiblePositions.Count < 1)
        {
            //no adiacent avaible, so try to froze itself
            if (board_array_master[icePosition.x, icePosition.y, 15] < 3) //max hp not reached yet
            {
                board_array_master[icePosition.x, icePosition.y, 15]++;
                //update sprite
                if (script_tiles_array[icePosition.x, icePosition.y].my_ice == null)
                {
                    script_tiles_array[icePosition.x, icePosition.y].my_ice = garbageManager.RecycleIce(new Vector2(icePosition.x, -icePosition.y));
                    script_tiles_array[icePosition.x, icePosition.y].my_ice.name = "ice";
                    board_array_master[icePosition.x, icePosition.y, 10] = 0;//can't fall
                }

                SpriteRenderer mySprite = script_tiles_array[icePosition.x, icePosition.y].my_ice.GetComponent<SpriteRenderer>();
                mySprite.sprite = myTheme.ice_hp[board_array_master[icePosition.x, icePosition.y, 15] - 1];
            }


            return;
        }


        //grow 
        int pickTarget = Random.Range(0, avaiblePositions.Count);
        board_array_master[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y, 3] = 2;//became ice
        board_array_master[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y, 15]++;//grow ice
        //update sprite
        if (script_tiles_array[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y].my_ice == null)
        {
            script_tiles_array[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y].my_ice = garbageManager.RecycleIce(new Vector2(avaiblePositions[pickTarget].x, -avaiblePositions[pickTarget].y));
            script_tiles_array[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y].my_ice.name = "ice";
            board_array_master[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y, 10] = 0;//can't fall
        }


        SpriteRenderer sprite_restraint = script_tiles_array[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y].my_ice.GetComponent<SpriteRenderer>();
        sprite_restraint.sprite = myTheme.ice_hp[board_array_master[avaiblePositions[pickTarget].x, avaiblePositions[pickTarget].y, 15] - 1];
    }

    bool ThisGemCanFroze(int x, int y, int iceCap)
    {
        if (board_array_master[x, y, 0] < 0)//no tile
            return false;

        if (board_array_master[x, y, 1] < 0 || board_array_master[x, y, 1] > 6)//no gem
            return false;

        if (board_array_master[x, y, 11] != 0)//it is busy
            return false;

        if (board_array_master[x, y, 3] == 1)//padlock
            return false;

        if (board_array_master[x, y, 4] != 0)//it is special
            return false;

        if (board_array_master[x, y, 3] == 2)//ice
        {
            if (board_array_master[x, y, 15] >= iceCap) //have an ice greater of ice-striker
                return false;

            if (board_array_master[x, y, 15] >= 3) //max hp already reached
            return false;
        }


        return true;
    }


}
