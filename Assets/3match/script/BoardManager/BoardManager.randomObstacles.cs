using UnityEngine;
using System.Collections.Generic;

public partial class BoardManager : MonoBehaviour
{

    void PlaceRandomJunk(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles-1; y++)//don't place on the bottom line
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                //don't place just over an hole
                BoardElementFile tempUnderTile = fileMap.GetTile(x, y+1);
                if (tempUnderTile.tile.type == TypeOfTile.None)
                    continue;

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 1] = 9;//don't 3 match
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 4] = -100;//is junk

            avaibleGems.RemoveAt(randomPick);

        }
    }

    void PlaceRandomPadlocks(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 3] = 1;//is a padlock
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 15] = 1;//hp

            avaibleGems.RemoveAt(randomPick);

        }
    }

    void PlaceRandomIce(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 3] = 2;//is ice
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 15] = 1;//hp

            avaibleGems.RemoveAt(randomPick);

        }
    }

    void PlaceRandomBlocks(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 1] = 41;//is a block
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 14] = 1;//hp

            avaibleGems.RemoveAt(randomPick);

        }
    }

    void PlaceRandomFallingBlock(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 1] = 51;//is a falling block
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 14] = 1;//hp

            avaibleGems.RemoveAt(randomPick);

        }
    }

    void PlaceRandomGenerativeBlock(int elements, StageFile fileMap)
    {
        //find were you can place the element
        List<Vector2Int> avaibleGems = new List<Vector2Int>();
        for (int y = 0; y < board._Y_tiles; y++)
        {
            for (int x = 0; x < board._X_tiles; x++)
            {
                BoardElementFile temp = fileMap.GetTile(x, y);

                if (temp.content.type == TypeOfContent.Gem && temp.restrain.type == TypeOfRestrain.None)
                {
                    avaibleGems.Add(new Vector2Int(x, y));

                }
            }
        }


        for (int i = 0; i < elements; i++)
        {
            if (avaibleGems.Count < 1)
                return;

            int randomPick = UnityEngine.Random.Range(0, avaibleGems.Count);

            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 1] = 61;//is a falling block
            board.board_array_master[avaibleGems[randomPick].x, avaibleGems[randomPick].y, 14] = 1;//hp

            avaibleGems.RemoveAt(randomPick);

        }
    }
}
