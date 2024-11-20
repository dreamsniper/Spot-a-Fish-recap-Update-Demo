using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageManager : MonoBehaviour {

    public Board_C board;
    public BoardManager boardManager;

    List<GameObject> tileContent_garbage;
    List<TileContent> garbageSpriteRenderer;

    List<GameObject> garbage_tiles;
    List<GameObject> garbage_padlocks;
    List<GameObject> garbage_ice;
    List<GameObject> garbage_fallingPadlocks;
    List<GameObject> garbage_cage;

    //fx
    public enum fxGarbageType
    {
        GemExplosion,
        BigExplosion,

        BonusDestroyOne,
        BonusDestroy3x3,
        BonusDestroyHorizontal,
        BonusDestroyVertical,
        BonusDestroyHorizontalAndVertical
    }

    [HideInInspector] public List<GameObject>[] garbage_recycle_gem_explosion_fx;//7
    [HideInInspector] public List<GameObject>[] garbage_recycle_big_explosion_fx;//4

    [HideInInspector] public List<GameObject> garbage_recycle_bonus_destroy_one_fx;
    [HideInInspector] public List<GameObject> garbage_recycle_bonus_destroy_3x3_fx;
    [HideInInspector] public List<GameObject> garbage_recycle_bonus_destroy_horizontal_fx;
    [HideInInspector] public List<GameObject> garbage_recycle_bonus_destroy_vertical_fx;
    [HideInInspector] public List<GameObject> garbage_recycle_bonus_destroy_horizontal_and_vertical_fx;

    private void Awake()
    {
        garbageSpriteRenderer = new List<TileContent>();
        tileContent_garbage = new List<GameObject>();
        garbage_tiles = new List<GameObject>();
        garbage_padlocks = new List<GameObject>();
        garbage_ice = new List<GameObject>();
        garbage_fallingPadlocks = new List<GameObject>();
        garbage_cage = new List<GameObject>();


        garbage_recycle_gem_explosion_fx = new List<GameObject>[7];
        for (int i = 0; i < garbage_recycle_gem_explosion_fx.Length; i++)
            garbage_recycle_gem_explosion_fx[i] = new List<GameObject>();

        garbage_recycle_big_explosion_fx = new List<GameObject>[4];
        for (int i = 0; i < garbage_recycle_big_explosion_fx.Length; i++)
            garbage_recycle_big_explosion_fx[i] = new List<GameObject>();


        garbage_recycle_bonus_destroy_one_fx = new List<GameObject>();
        garbage_recycle_bonus_destroy_3x3_fx = new List<GameObject>();
        garbage_recycle_bonus_destroy_horizontal_fx = new List<GameObject>();
        garbage_recycle_bonus_destroy_vertical_fx = new List<GameObject>();
        garbage_recycle_bonus_destroy_horizontal_and_vertical_fx = new List<GameObject>();
    }

    public void RecycleBonusDestroyOneFX(int x, int y)
    {
        if (!board.myTheme.bonus_have_explosion_fx)
            return;

        if (garbage_recycle_bonus_destroy_one_fx.Count > 0) //if you can recycle a previous fx
        {
            garbage_recycle_bonus_destroy_one_fx[0].GetComponent<explosion_fx>().Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_bonus_destroy_one_fx.RemoveAt(0);
        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.destroy_one_fx,
                                                       new Vector3(x, -y, -1.5f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BonusDestroyOne);
        }
    }

    public void RecycleBonusDestroy3x3FX(int x, int y)
    {
        if (!board.myTheme.bonus_have_explosion_fx)
            return;

        if (garbage_recycle_bonus_destroy_3x3_fx.Count > 0) //if you can recycle a previous fx
        {
            garbage_recycle_bonus_destroy_3x3_fx[0].GetComponent<explosion_fx>().Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_bonus_destroy_3x3_fx.RemoveAt(0);

        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.destroy_3x3_fx,
                                                       new Vector3(x, -y, -1.5f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BonusDestroy3x3);
        }
    }

    public void RecycleBonusDestroyHorizontalFX(int x, int y, int fx_end_r, int fx_end_l)
    {
        if (!board.myTheme.bonus_have_explosion_fx)
            return;

        if (garbage_recycle_bonus_destroy_horizontal_fx.Count > 0) //if you can recycle a previous fx
        {
            explosion_fx fx_temp = garbage_recycle_bonus_destroy_horizontal_fx[0].GetComponent<explosion_fx>();
            fx_temp.Setup_horizontal(fx_end_r, fx_end_l);
            fx_temp.Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_bonus_destroy_horizontal_fx.RemoveAt(0);

        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.destroy_horizontal_fx,
                                                       new Vector3(x, -y, -1.5f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Setup_horizontal(fx_end_r, fx_end_l);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BonusDestroyHorizontal);
        }
    }

    public void RecycleBonusDestroyVerticalFX(int x, int y, int fx_end_up, int fx_end_down)
    {
        if (!board.myTheme.bonus_have_explosion_fx)
            return;

        if (garbage_recycle_bonus_destroy_vertical_fx.Count > 0) //if you can recycle a previous fx
        {
            explosion_fx fx_temp = garbage_recycle_bonus_destroy_vertical_fx[0].GetComponent<explosion_fx>();
            fx_temp.Setup_vertical(fx_end_up, fx_end_down);
            fx_temp.Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_bonus_destroy_vertical_fx.RemoveAt(0);

        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.destroy_vertical_fx,
                                                       new Vector3(x, -y, -1.5f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Setup_vertical(fx_end_up, fx_end_down);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BonusDestroyVertical);
        }
    }

    public void RecycleBonusDestroyHorizontalAndVerticalFX(int x, int y, int fx_end_up, int fx_end_down, int fx_end_r, int fx_end_l)
    {
        if (!board.myTheme.bonus_have_explosion_fx)
            return;

        if (garbage_recycle_bonus_destroy_horizontal_and_vertical_fx.Count > 0) //if you can recycle a previous fx
        {
            explosion_fx fx_temp = garbage_recycle_bonus_destroy_horizontal_and_vertical_fx[0].GetComponent<explosion_fx>();
            fx_temp.Setup_horizontal_and_vertical(fx_end_up, fx_end_down, fx_end_r, fx_end_l);
            fx_temp.Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_bonus_destroy_horizontal_and_vertical_fx.RemoveAt(0);

        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.destroy_horizontal_and_vertical_fx,
                                                       new Vector3(x, -y, -1.5f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Setup_horizontal_and_vertical(fx_end_up, fx_end_down, fx_end_r, fx_end_l);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BonusDestroyHorizontalAndVertical);
        }
    }


    public void StoreAuxiliaryWhenGenrateBoard(GameObject tile_content, int x, int y)
    {
        //auxiliary gem in garbage that will be use when this tile will be free
        GameObject temp =((GameObject) Instantiate(tile_content, new Vector2(x, -y), Quaternion.identity));
        tileContent_garbage.Add(temp);
        temp.SetActive(false);
    }

    public void Put_in_garbage(TileContent target)
    {
        garbageSpriteRenderer.Add(target);
        tileContent_garbage.Add(target.gameObject);
        target.gameObject.SetActive(false);
    }

    public TileContent Take_from_garbage(int x, int y, TypeOfContent typeOfContent)
    {
        //Debug.Log("--Take_from_garbage: " + typeOfContent + " " + garbageSpriteRenderer.Count);
        //Debug.Log(garbageSpriteRenderer[0].name);

        TileContent item = garbageSpriteRenderer[0];
        item.transform.position = new Vector3(x, -y, 0);

        if (typeOfContent == TypeOfContent.Gem)
            garbageSpriteRenderer[0].mySpriteRenderer.sprite = board.myTheme.gem_colors[board.board_array_master[x, y, 1]];

        garbageSpriteRenderer.RemoveAt(0);

        item.gameObject.SetActive(true);
        return item;
    }

    public void RecycleGemExplosionFX(int x, int y)
    {
        if (garbage_recycle_gem_explosion_fx[board.board_array_master[x, y, 1]].Count > 0) //if you can recycle a previous fx
        {
            garbage_recycle_gem_explosion_fx[board.board_array_master[x, y, 1]][0].GetComponent<explosion_fx>().Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_gem_explosion_fx[board.board_array_master[x, y, 1]].RemoveAt(0);
        }
        else
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.gem_explosion_fx[board.board_array_master[x, y, 1]],
                                    new Vector3(x, -y, -1f),
                                Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.GemExplosion, board.board_array_master[x, y, 1]);
        }
    }

    public void RecycleBigExplosionFX(int x, int y, int use_fx_big_explosion_here)
    {
        if (garbage_recycle_big_explosion_fx[use_fx_big_explosion_here - 4].Count > 0) //if you can recycle a previous fx
        {
            garbage_recycle_big_explosion_fx[use_fx_big_explosion_here - 4][0].GetComponent<explosion_fx>().Show_me(new Vector3(x, -y, -1f));
            garbage_recycle_big_explosion_fx[use_fx_big_explosion_here - 4].RemoveAt(0);
        }
        else //create a new fx
        {
            Transform fx_temp = (Transform)Instantiate(board.myTheme.gem_big_explosion_fx[use_fx_big_explosion_here - 4],
                                                       new Vector3(x, -y, -1f),
                                                       Quaternion.identity);
            fx_temp.GetComponent<explosion_fx>().Activate_me(this, fxGarbageType.BigExplosion,(use_fx_big_explosion_here - 4));
        }
    }

    public void GarbageTile(tile_C tile)
    {
        if (tile == null)
            return;

        if (tile.myContent != null)
        {
            Put_in_garbage(tile.myContent);
            tile.myContent = null;
        }

        if (tile.my_padlock != null)
            GarbagePadlock(tile);


        if (tile.my_ice != null)
            GarbageIce(tile);

        garbage_tiles.Add(tile.gameObject);
        tile.gameObject.SetActive(false);
    }

    public void GarbagePadlock(tile_C tile)
    {
            garbage_padlocks.Add(tile.my_padlock.gameObject);
            tile.my_padlock.gameObject.SetActive(false);
            tile.my_padlock = null;
    }

    public void GarbageFallingPadlock(tile_C tile)
    {
        garbage_fallingPadlocks.Add(tile.my_padlock.gameObject);
        tile.my_padlock.gameObject.SetActive(false);
        tile.my_padlock.transform.SetParent(null);
        tile.my_padlock = null;
    }

    public void GarbageCage(tile_C tile)
    {
        garbage_cage.Add(tile.my_padlock.gameObject);
        tile.my_padlock.gameObject.SetActive(false);
        tile.my_padlock = null;
    }

    public void GarbageIce(tile_C tile)
    {
        garbage_ice.Add(tile.my_ice.gameObject);
        tile.my_ice.gameObject.SetActive(false);
        tile.my_ice = null;
    }

    public GameObject RecycleTile(Vector3 position)
    {
        
        GameObject temp = null;

        if (garbage_tiles.Count < 1)
            temp = (GameObject)Instantiate(boardManager.tile_obj, position, Quaternion.identity);
        else
        {
            temp = garbage_tiles[0];
            garbage_tiles.RemoveAt(0);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        return temp;
    }

    public GameObject RecyclePadlock(Vector3 position)
    {
        GameObject temp = null;

        if (garbage_padlocks.Count < 1)
            temp = (GameObject)Instantiate(boardManager.over_gem, position, Quaternion.identity);
        else
        {
            temp = garbage_padlocks[0];
            garbage_padlocks.RemoveAt(0);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        return temp;
    }

    public GameObject RecycleFallingPadlock(Vector3 position)
    {
        GameObject temp = null;

        if (garbage_fallingPadlocks.Count < 1)
            temp = (GameObject)Instantiate(boardManager.over_gem, position, Quaternion.identity);
        else
        {
            temp = garbage_fallingPadlocks[0];
            garbage_fallingPadlocks.RemoveAt(0);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        return temp;
    }

    public GameObject RecycleCage(Vector3 position)
    {
        GameObject temp = null;

        if (garbage_cage.Count < 1)
            temp = (GameObject)Instantiate(boardManager.over_gem, position, Quaternion.identity);
        else
        {
            temp = garbage_cage[0];
            garbage_cage.RemoveAt(0);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        return temp;
    }

    public GameObject RecycleIce(Vector3 position)
    {
        GameObject temp = null;

        if (garbage_ice.Count < 1)
            temp = (GameObject)Instantiate(boardManager.over_gem, position, Quaternion.identity);
        else
        {
            temp = garbage_ice[0];
            garbage_ice.RemoveAt(0);
            temp.transform.position = position;
            temp.SetActive(true);
        }

        return temp;
    }
}
