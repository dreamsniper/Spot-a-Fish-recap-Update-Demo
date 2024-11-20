using UnityEngine;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;

public partial class Board_C : MonoBehaviour
{
    //read board
    [HideInInspector] public int number_of_moves_possible;
    int[,] list_of_moves_possible;
    [HideInInspector] public int number_of_gems_moveable;

    public void Check_ALL_possible_moves()
    {

        if (current_moveStatus != moveStatus.waitingNewMove)
            return;

        //reset count
        number_of_moves_possible = 0;
        clickable_bonus_on_boad = false;
        //switchable_bonus_on_boad = false;
        free_switchable_bonus_on_boad = false;

        if (myRuleset.allow2x2Explosions)
        {
                thisGemCan2x2Explode.Clear();
            /*
            for (int y = 0; y < _Y_tiles; y++)
            {
                for (int x = 0; x < _X_tiles; x++)
                    thisGemCan2x2Explode[x, y].ResetMe();

            }
            */
        }

        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
                Check_moves_of_this_gem(x, y);

        }

        if ((number_of_moves_possible <= 0) && (!clickable_bonus_on_boad))
            Check_if_shuffle();
        else
        {
            shuffleAttemptsCount = 0;
            elements_to_damage_list.Clear();
            ListOfPotentialMoves();
        }

    }


class PossibleMove
    {
        int maxGemsThatWillExplodeWithThisMove;
        public Vector2Int position;
        public int color;

        public int downMoveExplosion;
        public int upMoveExplosion;
        public int rightMoveExplosion;
        public int leftMoveExplosion;

        public bool upIs2x2Explosion;
        public bool downIs2x2Explosion;
        public bool rightIs2x2Explosion;
        public bool leftIs2x2Explosion;


        int mostBigExplosionMoveDirection;

        public void SetMostBigExplosion(int explosion, int direction)
        {
            if (explosion > maxGemsThatWillExplodeWithThisMove)
            {
                maxGemsThatWillExplodeWithThisMove = explosion;
                mostBigExplosionMoveDirection = direction;
            }
        }

        public int GetMaxGemsThatWillExplodeWithThisMove()
        {
            return maxGemsThatWillExplodeWithThisMove;
        }

        public int GetMostBigExplosionDirection()
        {
            return mostBigExplosionMoveDirection;
        }

       
    }

    
    //public Square2x2ExplosionInfo[,] thisGemCan2x2Explode;
    public Dictionary<Vector2Int, Square2x2ExplosionInfo> thisGemCan2x2Explode;
    void AddExplosionTothisGemCan2x2ExplodeDictionary(Vector2Int myPosition, MyDirections thisDiretion, Vector2Int valueToAdd)
    {
        if (!thisGemCan2x2Explode.ContainsKey(myPosition))
            thisGemCan2x2Explode.Add(myPosition, new Square2x2ExplosionInfo());

        thisGemCan2x2Explode[myPosition].GetExplosionByDirection(thisDiretion).Add(valueToAdd);


    }
    public List<Vector2Int> GetSquare2x2ExplosionInfoList(int x, int y, MyDirections thisDiretion)
    {
        Vector2Int myVector = new Vector2Int(x, y); 

        if (thisGemCan2x2Explode.ContainsKey(myVector))
            return thisGemCan2x2Explode[myVector].GetExplosionByDirection(thisDiretion);


        return new List<Vector2Int>();

    }

    public class Square2x2ExplosionInfo
    {
        public List<Vector2Int> explosionPositionsUp = new List<Vector2Int>();
        public List<Vector2Int> explosionPositionsDown = new List<Vector2Int>();
        public List<Vector2Int> explosionPositionsLeft = new List<Vector2Int>();
        public List<Vector2Int> explosionPositionsRight = new List<Vector2Int>();

        public void ResetMe()
        {
            explosionPositionsUp.Clear();
            explosionPositionsDown.Clear();
            explosionPositionsLeft.Clear();
            explosionPositionsRight.Clear();
        }

        public List<Vector2Int> GetExplosionByDirection(MyDirections direction)
        {
            if (direction == MyDirections.Up)
                return explosionPositionsUp;

            if (direction == MyDirections.Down)
                return explosionPositionsDown;

            if (direction == MyDirections.Right)
                return explosionPositionsRight;

            if (direction == MyDirections.Left)
                return explosionPositionsLeft;

            return new List<Vector2Int>();

        }
    }

void ListOfPotentialMoves()
    {
        //Debug.LogWarning("------------------------ListOfPotentialMoves() " + number_of_moves_possible);
        List<PossibleMove> possibleMoves = new List<PossibleMove>();
        //list_of_moves_possible = new int[number_of_moves_possible, 8];
        /*
        0 = max gems that will explode with this move
        1 = x
        2 = y
        3 = color
            4= down [n. how many gem explode if this gem go down]
            5= up [n. how many gem explode if this gem go up]
            6= right [n. how many gem explode if this gem go right]
            7= left [n. how many gem explode if this gem go left]
            --8 = most big explosion
        8 = is 2x2 explosion 0 false 1 true
        */
        number_of_gems_moveable = 0;
        for (int y = 0; y < _Y_tiles; y++)
        {
            for (int x = 0; x < _X_tiles; x++)
            {
                if (board_array_master[x, y, 5] < 1)
                    continue;

                if (board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
                {

                    //Debug.LogWarning("[" + number_of_gems_moveable + "]    " + x + "," + y + " status = " + board_array_master[x, y, 11]);
                    continue;
                }

                PossibleMove possibleMoveTemp = new PossibleMove();

                //if (board_array_master[x, y, 5] > 0) //if this gem have at least one move
                //{
                    //Debug.LogWarning("["+ number_of_gems_moveable+"]    "+ x + "," + y + " moves = " + board_array_master[x, y, 5]);


                    //gem color
                    //list_of_moves_possible[number_of_gems_moveable, 3] = board_array_master[x, y, 1];
                    possibleMoveTemp.color = board_array_master[x, y, 1];

                    //gem position
                    //list_of_moves_possible[number_of_gems_moveable, 1] = x;
                    //list_of_moves_possible[number_of_gems_moveable, 2] = y;
                    possibleMoveTemp.position.x = x;
                    possibleMoveTemp.position.y = y;


                //moves
                if (board_array_master[x, y, 6] > 0)
                    {
                    possibleMoveTemp.downMoveExplosion = board_array_master[x, y, 6];
                    possibleMoveTemp.SetMostBigExplosion(board_array_master[x, y, 6],4);

                    if (myRuleset.allow2x2Explosions && GetSquare2x2ExplosionInfoList(x,y,MyDirections.Down).Count  > 0 )
                        possibleMoveTemp.downIs2x2Explosion = true;
                    /*
                    list_of_moves_possible[number_of_gems_moveable, 4] = board_array_master[x, y, 6];
                        if (board_array_master[x, y, 6] > list_of_moves_possible[number_of_gems_moveable, 0])
                            list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 6];*/
                }
                    if (board_array_master[x, y, 7] > 0)
                    {
                    possibleMoveTemp.upMoveExplosion = board_array_master[x, y, 7];
                    possibleMoveTemp.SetMostBigExplosion(board_array_master[x, y, 7],5);

                    if (myRuleset.allow2x2Explosions && GetSquare2x2ExplosionInfoList(x, y, MyDirections.Up).Count > 0)
                        possibleMoveTemp.upIs2x2Explosion = true;
                    /*
                    list_of_moves_possible[number_of_gems_moveable, 5] = board_array_master[x, y, 7];
                        if (board_array_master[x, y, 7] > list_of_moves_possible[number_of_gems_moveable, 0])
                            list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 7];*/
                }
                    if (board_array_master[x, y, 8] > 0)
                    {
                    possibleMoveTemp.rightMoveExplosion = board_array_master[x, y, 8];
                    possibleMoveTemp.SetMostBigExplosion(board_array_master[x, y, 8],6);

                    if (myRuleset.allow2x2Explosions && GetSquare2x2ExplosionInfoList(x, y, MyDirections.Right).Count > 0)
                        possibleMoveTemp.rightIs2x2Explosion = true;


                    /*
                    list_of_moves_possible[number_of_gems_moveable, 6] = board_array_master[x, y, 8];
                        if (board_array_master[x, y, 8] > list_of_moves_possible[number_of_gems_moveable, 0])
                            list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 8];*/
                    }
                    if (board_array_master[x, y, 9] > 0)
                    {
                    possibleMoveTemp.leftMoveExplosion = board_array_master[x, y, 9];
                    possibleMoveTemp.SetMostBigExplosion(board_array_master[x, y, 9],7);

                    if (myRuleset.allow2x2Explosions && GetSquare2x2ExplosionInfoList(x, y, MyDirections.Left).Count > 0)
                        possibleMoveTemp.leftIs2x2Explosion = true;

                    /*
                    list_of_moves_possible[number_of_gems_moveable, 7] = board_array_master[x, y, 9];
                        if (board_array_master[x, y, 9] > list_of_moves_possible[number_of_gems_moveable, 0])
                            list_of_moves_possible[number_of_gems_moveable, 0] = board_array_master[x, y, 9];
                        */
                }

                    if (possibleMoveTemp.GetMaxGemsThatWillExplodeWithThisMove() > 0)
                        possibleMoves.Add(possibleMoveTemp);


                    //DEBUG show all moves
                    /*
                        tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
                        if  (list_of_moves_possible[number_of_gems_moveable,0] >= 3) //)&& (_show_all_moves) )
                            tile_script.Debug_show_available_moves(list_of_moves_possible[number_of_gems_moveable,0]);
                        else
                            tile_script.Debug_show_available_moves(0);
                    */

                    if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] <= 8))
                        number_of_gems_moveable++;
                //}
                /*
                else //DEBUG
                    {
                    if (board_array_master[x,y,0] != -1)
                        {
                        tile_C tile_script = (tile_C)tiles_array[x,y].GetComponent("tile_C");
                            tile_script.Debug_show_available_moves(0);
                        }
                    }
                    */
            }
        }

        number_of_moves_possible = possibleMoves.Count;
        list_of_moves_possible = new int[possibleMoves.Count, 9];
        for (int m = 0; m < list_of_moves_possible.GetLength(0); m++)
        {
            list_of_moves_possible[m, 0] = possibleMoves[m].GetMaxGemsThatWillExplodeWithThisMove();
            

            list_of_moves_possible[m, 1] = possibleMoves[m].position.x;
            list_of_moves_possible[m, 2] = possibleMoves[m].position.y;

            list_of_moves_possible[m, 3] = possibleMoves[m].color;

            list_of_moves_possible[m, 4] = possibleMoves[m].downMoveExplosion;
            list_of_moves_possible[m, 5] = possibleMoves[m].upMoveExplosion;
            list_of_moves_possible[m, 6] = possibleMoves[m].rightMoveExplosion;
            list_of_moves_possible[m, 7] = possibleMoves[m].leftMoveExplosion;

            list_of_moves_possible[m, 8] = 0;
            if (possibleMoves[m].downIs2x2Explosion || possibleMoves[m].leftIs2x2Explosion || possibleMoves[m].upIs2x2Explosion || possibleMoves[m].rightIs2x2Explosion)
            {
                list_of_moves_possible[m, 8] = 1;
            }

            //list_of_moves_possible[m, 8] = possibleMoves[m].GetMostBigExplosionDirection();

        }


        //debug
        int totalCount = 0;
        for (int m = 0; m < list_of_moves_possible.GetLength(0); m++)
        {
            int directionCount = 0;
            for (int i = 4; i <= 7; i++)//search big explosion
            {
                if (list_of_moves_possible[m, i] <= 0)
                    continue;

                directionCount++;
                totalCount++;
            }

            if (directionCount == 0)
                Debug.LogError("__[" + m + "] " + list_of_moves_possible[m, 1] + "," + list_of_moves_possible[m, 2] + " explosion:  " + list_of_moves_possible[m, 0] + "   BUT  list_of_moves_possible   NO DIRECTION!!!!");
        }




        if (myRuleset.gameLoop_selected == Ruleset.gameLoop.TurnBased)
            UpdateTurn();

    }

    void CheckLose_player_have_zero_moves()
    {
        //print("CheckLose_player_have_zero_moves");
        if (myRuleset.lose_requirement_selected == Ruleset.lose_requirement.player_have_zero_moves)
        {
            if (player.myCharacter.currentMovesLeft <= 0)
            {
                player_can_move = false;
                game_end = true;
            }
        }
    }


    /*
    public class MoveInfo
    {
        public List<Vector2Int> upExplosions = new List<Vector2Int>();
        public List<Vector2Int> downExplosions = new List<Vector2Int>();

        public List<Vector2Int> leftExplosions = new List<Vector2Int>();
        public List<Vector2Int> rightExplosions = new List<Vector2Int>();

        public void ClearInfos()
        {
            upExplosions.Clear();
            downExplosions.Clear();
            leftExplosions.Clear();
            rightExplosions.Clear();
        }
    }
    public Dictionary<Vector2Int, MoveInfo> moveInfos = new Dictionary<Vector2Int, MoveInfo>();//tile position, move information
    */

    public bool ThereIsATileHere(int x, int y)
    {
        if (x >= _X_tiles || x < 0)
            return false;

        if (y >= _Y_tiles || y < 0)
            return false;

        if (board_array_master[x, y, 0] > -1)
            return true;

        return false;
    }



    public void Check_moves_of_this_gem(int x, int y) //call from  Check_ALL_possible_moves()
    {
        if (board_array_master[x, y, 11] == 6 || board_array_master[x, y, 11] == 111 || board_array_master[x, y, 11] == 222 || board_array_master[x, y, 11] == 333)//explosion or creation falling animation ongoing
        {
            return;
        }

        //reset array
        //Vector2Int moveInfoPosition = new Vector2Int(x, y);
        //moveInfos[moveInfoPosition].ClearInfos();
        board_array_master[x, y, 5] = 0; //number of useful moves of this gem [from 0 = none, to 4 = all directions]
        board_array_master[x, y, 6] = 0; //up
        board_array_master[x, y, 7] = 0; //down
        board_array_master[x, y, 8] = 0; //right
        board_array_master[x, y, 9] = 0; //left


        if ((board_array_master[x, y, 1] >= 0) && (board_array_master[x, y, 1] <= 8) && (board_array_master[x, y, 3] == 0))//this gem exist and can move
        {

            #region 2x2 explosion
            //Check 2x2 explosions
            if (myRuleset.allow2x2Explosions)
            {
                //Debug.LogWarning("TEST 2x2");
                bool invalid2x2Explosion = false; //if the gem can explode in a line, it can't explode in a 2x2 square 

                // X*
                // **
                #region 2x2 move R to leftPositionUp
                // >*
                // **

                if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 0] > -1) //there is a tile
                    && (board_array_master[x + 1, y, 3] == 0) //no padlock
                    && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] <= 9))// there is a gem
                    {
                        if ((y + 1) < _Y_tiles && (x + 2) < _X_tiles)
                        {
                            Vector2Int aTemp = new Vector2Int(x + 2, y);
                            Vector2Int bTemp = new Vector2Int(x + 1, y + 1);
                            Vector2Int cTemp = new Vector2Int(x + 2, y + 1);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                            {
                                //these must be of a different color
                                invalid2x2Explosion = false;
                                if (x + 3 < _X_tiles)
                                {
                                    if (board_array_master[x, y, 1] == board_array_master[x + 3, y, 1])
                                        invalid2x2Explosion = true;
                                }

                                if (y + 2 < _Y_tiles)
                                {
                                    if (board_array_master[x, y, 1] == board_array_master[x + 1,y + 2,1])
                                        invalid2x2Explosion = true;
                                }

                                if (y -1 >= 0)
                                {
                                    if (board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1])
                                        invalid2x2Explosion = true;
                                }

                                if (!invalid2x2Explosion)
                                {
                                    //none of them is busy
                                    if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                    {
                                        //annotate this move
                                        number_of_moves_possible++;
                                    
                                        board_array_master[x, y, 5] += 1;
                                        board_array_master[x, y, 8] += 3;//this move RIGHT will make explode +3 gem


                                        Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                        AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(aTemp.x, aTemp.y));
                                        AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(bTemp.x, bTemp.y));
                                        AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(cTemp.x, cTemp.y));

                                    /*
                                    thisGemCan2x2Explode[dictionaryPosition].explosionPositionsRight.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[dictionaryPosition].explosionPositionsRight.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[dictionaryPosition].explosionPositionsRight.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */

                                    //Debug.Log(x + "," + y + "    2x2 move Right to leftPositionUp");

                                    }
                                }
                            }

                        }
                    }
                #endregion
                
                #region 2x2 move Down to leftPositionUp
                // v*
                // **

                if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 0] > -1)
                && (board_array_master[x, y + 1, 3] == 0) //no padlock
                && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] <= 9))// there is a gem
                {
                    if ((y + 2) < _Y_tiles && (x + 1) < _X_tiles)
                    {
                        Vector2Int aTemp = new Vector2Int(x , y + 2);
                        Vector2Int bTemp = new Vector2Int(x + 1, y + 1);
                        Vector2Int cTemp = new Vector2Int(x + 1, y + 2);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;
  
                            if (y + 3 < _Y_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x, y + 3, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (x + 2 < _X_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 2, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (x - 1 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x -1, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 6] += 3;//this move DOWN will make explode +3 gem


                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */

                                    //Debug.Log(x + "," + y + "    2x2 move Down to leftPositionUp! ");

                                }
                            }
                        }
                    }
                }
                #endregion


                // *X
                // **
                #region 2x2 move Down to rightPositionUp
                // *v
                // **
                if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 0] > -1)
                && (board_array_master[x, y + 1, 3] == 0) //no padlock
                && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] <= 9))// there is a gem
                {
                    if ((y + 2) < _Y_tiles && (x - 1) >= 0)
                    {
                        Vector2Int aTemp = new Vector2Int(x, y + 2);
                        Vector2Int bTemp = new Vector2Int(x - 1, y + 1);
                        Vector2Int cTemp = new Vector2Int(x - 1, y + 2);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;
                            
                            if (y + 3 < _Y_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x, y + 3, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (x + 1 < _X_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (x - 2 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 2, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }

                            
                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 6] += 3;//this move DOWN will make explode +3 gem


                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Down, new Vector2Int(cTemp.x, cTemp.y));

                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsDown.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move Down to rightPositionUp! ");

                                }
                            }
                        }
                    }
                }
                #endregion

                #region 2x2 move R to rightPositionUp
                // *<
                // **
                if (((x - 1) >= 0) && ((board_array_master[x - 1, y, 0] > -1))  //there is a tile
                        && (board_array_master[x - 1, y, 3] == 0) //no padlock
                        && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] <= 9))// there is a gem
                {
                    if ((y + 1) < _Y_tiles && (x - 2) >= 0)
                    {
                        Vector2Int aTemp = new Vector2Int(x - 2, y);
                        Vector2Int bTemp = new Vector2Int(x - 1, y + 1);
                        Vector2Int cTemp = new Vector2Int(x - 2, y + 1);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;
                            
                            if (x - 3  >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x -3, y, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (y - 1 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (y + 2 < _Y_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 1, y + 2, 1])
                                    invalid2x2Explosion = true;
                            }
                            

                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 9] += 3;//this move LEFT will make explode +3 gem


                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move L to rightPositionUp! ");

                                }
                            }
                        }
                    }
                }
                #endregion


                // **
                // *X
                #region 2x2 move L to rightPositionDown
                // **
                // *<
                if (((x - 1) >= 0) && ((board_array_master[x - 1, y, 0] > -1))  //there is a tile
                        && (board_array_master[x - 1, y, 3] == 0) //no padlock
                        && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] <= 9))// there is a gem
                {
                    if ((y - 1) >= 0 && (x - 2) >= 0)
                    {
                        Vector2Int aTemp = new Vector2Int(x - 2 , y);
                        Vector2Int bTemp = new Vector2Int(x - 1, y - 1 );
                        Vector2Int cTemp = new Vector2Int(x - 2, y - 1 );

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;
                            
                            if (x - 3 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 3, y, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (y - 2 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 1, y - 2, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (y + 1 < _Y_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 1, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            

                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 9] += 3;//this move LEFT will make explode +3 gem


                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Left, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsLeft.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move L rightPositionDown! ");

                                }
                            }
                        }
                    }
                }
                #endregion

                #region 2x2 move Up to rightPositionDown
                // **
                // *A

                if (((y - 1) >= 0) && (board_array_master[x, y - 1, 0] > -1)
                   && (board_array_master[x, y - 1, 3] == 0) //no padlock
                   && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] <= 9))// there is a gem
                {
                    if ((y - 2) >= 0 && (x - 1) >= 0)
                    {
                        Vector2Int aTemp = new Vector2Int(x, y - 2);
                        Vector2Int bTemp = new Vector2Int(x - 1, y - 1);
                        Vector2Int cTemp = new Vector2Int(x - 1, y - 2);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;

                            
                            if (y - 3 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x, y - 3, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (x - 2 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 2, y - 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (x + 1 < _X_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1])
                                    invalid2x2Explosion = true;
                            }
                         

                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 7] += 3;//this move UP will make explode +3 gem


                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move Up! rightPositionDown");

                                }
                            }
                        }

                    }
                }
                #endregion


                // **
                // X*
                #region 2x2 move R to leftPositionDown
                // **
                // >*

                if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 0] > -1) //there is a tile
                    && (board_array_master[x + 1, y, 3] == 0) //no padlock
                    && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] <= 9))// there is a gem
                {
                    if ((y - 1) >= 0 && (x + 2) < _X_tiles)
                    {
                        Vector2Int aTemp = new Vector2Int(x + 2, y);
                        Vector2Int bTemp = new Vector2Int(x + 1, y - 1);
                        Vector2Int cTemp = new Vector2Int(x + 2, y - 1);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;
                            
                            if (x + 3 < _X_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 3, y, 1])
                                    invalid2x2Explosion = true;
                            }

                            if (y + 1 < _Y_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (y - 2 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 1, y - 2, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 8] += 3;//this move RIGHT will make explode +3 gem



                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Right, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsRight.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsRight.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsRight.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move R! leftPositionDown");

                                }
                            }
                        }

                    }
                }
                #endregion

                #region 2x2 move Up to leftPositionDown
                // **
                // A*

                if (((y - 1) >= 0) && (board_array_master[x, y - 1, 0] > -1)
                   && (board_array_master[x, y - 1, 3] == 0) //no padlock
                   && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] <= 9))// there is a gem
                {
                    if ((y - 2) >= 0 && (x + 1) < _X_tiles)
                    {
                        Vector2Int aTemp = new Vector2Int(x , y - 2);
                        Vector2Int bTemp = new Vector2Int(x + 1, y - 1);
                        Vector2Int cTemp = new Vector2Int(x + 1, y - 2);

                        //same color in L formation
                        if ((board_array_master[x, y, 1] == board_array_master[aTemp.x, aTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[bTemp.x, bTemp.y, 1])
                            &&
                            (board_array_master[x, y, 1] == board_array_master[cTemp.x, cTemp.y, 1]))
                        {
                            //these must be of a different color
                            invalid2x2Explosion = false;

                            
                            if (y - 3 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x, y - 3, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (x - 1 >= 0)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            
                            if (x + 2 < _X_tiles)
                            {
                                if (board_array_master[x, y, 1] == board_array_master[x + 2, y - 1, 1])
                                    invalid2x2Explosion = true;
                            }
                            

                            if (!invalid2x2Explosion)
                            {
                                //none of them is busy
                                if (board_array_master[aTemp.x, aTemp.y, 11] == 0 && board_array_master[bTemp.x, bTemp.y, 11] == 0 && board_array_master[cTemp.x, cTemp.y, 11] == 0)
                                {
                                    //annotate this move
                                    number_of_moves_possible++;

                                    board_array_master[x, y, 5] += 1;
                                    board_array_master[x, y, 7] += 3;//this move UP will make explode +3 gem



                                    Vector2Int dictionaryPosition = new Vector2Int(x, y);
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(aTemp.x, aTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(bTemp.x, bTemp.y));
                                    AddExplosionTothisGemCan2x2ExplodeDictionary(dictionaryPosition, MyDirections.Up, new Vector2Int(cTemp.x, cTemp.y));
                                    /*
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(aTemp.x, aTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(bTemp.x, bTemp.y));
                                    thisGemCan2x2Explode[x, y].explosionPositionsUp.Add(new Vector2Int(cTemp.x, cTemp.y));
                                    */
                                    //Debug.Log(x + "," + y + "    2x2 move Up! leftPositionDown");

                                }
                            }
                        }

                    }
                }
                #endregion
               

            }
            #endregion

            
                #region move to right (x+1,y) if... 
                //it is feasible move to right
                if (((x + 1) < _X_tiles) && (board_array_master[x + 1, y, 0] > -1) //there is a tile
                && (board_array_master[x + 1, y, 3] == 0) //no padlock
                && (board_array_master[x + 1, y, 1] >= 0) && (board_array_master[x + 1, y, 1] <= 9))// there is a gem
                {

                    //2 up
                    if ((y + 2) < _Y_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x + 1, y + 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1]))
                        {
                            if (board_array_master[x + 1, y + 2, 11] == 0 && board_array_master[x + 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 8] == 0)//annotate this move
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 8] += 2;//this move will make explode +2 gem
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 1, y + 2));
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 1, y + 1));
                            }
                        }
                    }
                    //2 down
                    if ((y - 2) >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x + 1, y - 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1]))
                        {
                            if (board_array_master[x + 1, y - 2, 11] == 0 && board_array_master[x + 1, y - 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 8] == 0)//annotate this move
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 8] += 2;//this move will make explode +2 gem
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 1, y - 2));
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 1, y - 1));
                            }
                        }
                    }
                    //1 up and 1 down
                    if (((y - 1) >= 0) && ((y + 1) < _Y_tiles))
                    {
                        //Debug.Log("R //1 up and 1 down");
                        if ((board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1]))
                        {
                            if (board_array_master[x + 1, y - 1, 11] == 0 && board_array_master[x + 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 8] == 0)//annotate this move
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                //count explosions of this move
                                if ((y - 2) < 0)
                                {
                                    board_array_master[x, y, 8] += 1;

                                }
                                else if ((board_array_master[x, y, 1] != board_array_master[x + 1, y - 2, 1]))
                                {
                                    board_array_master[x, y, 8] += 1;
                                }

                                if ((y + 2) >= _Y_tiles)
                                {
                                    board_array_master[x, y, 8] += 1;
                                }
                                else if (board_array_master[x, y, 1] != board_array_master[x + 1, y + 2, 1])
                                {
                                    board_array_master[x, y, 8] += 1;
                                }
                            }
                        }
                    }
                    //2 to right
                    if ((x + 3) < _X_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x + 2, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 3, y, 1]))
                        {
                            if (board_array_master[x + 2, y, 11] == 0 && board_array_master[x + 3, y, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 8] == 0)//annotate this move
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;

                                }
                                board_array_master[x, y, 8] += 2;//this move will make explode +2 gem
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 2, y));
                                                                 //moveInfos[moveInfoPosition].rightExplosions.Add(new Vector2Int(x + 3, y));
                            }
                        }
                    }
                }
                #endregion
                #region move to left (x-1,y) if...
                if (((x - 1) >= 0) && ((board_array_master[x - 1, y, 0] > -1))  //there is a tile
                        && (board_array_master[x - 1, y, 3] == 0) //no padlock
                        && (board_array_master[x - 1, y, 1] >= 0) && (board_array_master[x - 1, y, 1] <= 9))// there is a gem
                {
                    //2 up
                    if ((y + 2) < _Y_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 1, y + 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y + 1, 1]))
                        {
                            if (board_array_master[x - 1, y + 2, 11] == 0 && board_array_master[x - 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 9] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 9] += 2;
                            }
                        }
                    }
                    //2 down
                    if ((y - 2) >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 1, y - 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1]))
                        {
                            if (board_array_master[x - 1, y - 2, 11] == 0 && board_array_master[x - 1, y - 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 9] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 9] += 2;
                            }
                        }
                    }
                    //1 up and 1 down
                    if (((y - 1) >= 0) && ((y + 1) < _Y_tiles))
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y + 1, 1]))
                        {
                            if (board_array_master[x - 1, y - 1, 11] == 0 && board_array_master[x - 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 9] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                //count explosions of this move
                                if ((y - 2) < 0)
                                    board_array_master[x, y, 9] += 1;
                                else if ((board_array_master[x, y, 1] != board_array_master[x - 1, y - 2, 1]))
                                    board_array_master[x, y, 9] += 1;

                                if ((y + 2) >= _Y_tiles)
                                    board_array_master[x, y, 9] += 1;
                                else if (board_array_master[x, y, 1] != board_array_master[x - 1, y + 2, 1])
                                    board_array_master[x, y, 9] += 1;

                            }
                        }
                    }
                    //2 right
                    if (x - 3 >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 2, y, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 3, y, 1]))
                        {
                            if (board_array_master[x - 2, y, 11] == 0 && board_array_master[x - 3, y, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 9] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 9] += 2;
                            }
                        }
                    }
                }
                #endregion
                #region move up (x,y+1) if...
                if (((y + 1) < _Y_tiles) && (board_array_master[x, y + 1, 0] > -1)
                   && (board_array_master[x, y + 1, 3] == 0) //no padlock
                   && (board_array_master[x, y + 1, 1] >= 0) && (board_array_master[x, y + 1, 1] <= 9))// there is a gem
                {
                    //2 up left
                    if ((x + 2) < _X_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x + 2, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1]))
                        {
                            if (board_array_master[x + 2, y + 1, 11] == 0 && board_array_master[x + 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 6] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 6] += 2;
                            }
                        }
                    }
                    //2 up right
                    if ((x - 2) >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 2, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y + 1, 1]))
                        {
                            if (board_array_master[x - 2, y + 1, 11] == 0 && board_array_master[x - 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 6] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 6] += 2;
                            }
                        }
                    }
                    //due up (1 right and 1 left)
                    if (((x - 1) >= 0) && ((x + 1) < _X_tiles))
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 1, y + 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y + 1, 1]))
                        {
                            if (board_array_master[x - 1, y + 1, 11] == 0 && board_array_master[x + 1, y + 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 6] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }

                                if ((x - 2) < 0)
                                    board_array_master[x, y, 6] += 1;
                                else if (board_array_master[x, y, 1] != board_array_master[x - 2, y + 1, 1])
                                    board_array_master[x, y, 6] += 1;

                                if ((x + 2) >= _X_tiles)
                                    board_array_master[x, y, 6] += 1;
                                else if (board_array_master[x, y, 1] != board_array_master[x + 2, y + 1, 1])
                                    board_array_master[x, y, 6] += 1;

                            }
                        }
                    }
                    //2 up
                    if (y + 3 < _Y_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x, y + 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y + 3, 1]))
                        {
                            if (board_array_master[x, y + 2, 11] == 0 && board_array_master[x, y + 3, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 6] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 6] += 2;
                            }
                        }
                    }
                }
                #endregion
                #region move down (x,y-1) if...
                if (((y - 1) >= 0) && (board_array_master[x, y - 1, 0] > -1)
                   && (board_array_master[x, y - 1, 3] == 0) //no padlock
                   && (board_array_master[x, y - 1, 1] >= 0) && (board_array_master[x, y - 1, 1] <= 9))// there is a gem

                {
                    if ((x + 2) < _X_tiles)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x + 2, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1]))
                        {
                            if (board_array_master[x + 2, y - 1, 11] == 0 && board_array_master[x + 1, y - 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 7] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 7] += 2;
                            }
                        }
                    }
                    //2 down right
                    if ((x - 2) >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 2, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1]))
                        {
                            if (board_array_master[x - 2, y - 1, 11] == 0 && board_array_master[x - 1, y - 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 7] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 7] += 2;
                            }
                        }
                    }
                    //2 down (1 right and 1 left)
                    if (((x - 1) >= 0) && ((x + 1) < _X_tiles))
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x - 1, y - 1, 1]) && (board_array_master[x, y, 1] == board_array_master[x + 1, y - 1, 1]))
                        {
                            if (board_array_master[x - 1, y - 1, 11] == 0 && board_array_master[x + 1, y - 1, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 7] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }


                                if ((x - 2) < 0)
                                    board_array_master[x, y, 7] += 1;
                                else if (board_array_master[x, y, 1] != board_array_master[x - 2, y - 1, 1])
                                    board_array_master[x, y, 7] += 1;

                                if ((x + 2) >= _X_tiles)
                                    board_array_master[x, y, 7] += 1;
                                else if (board_array_master[x, y, 1] != board_array_master[x + 2, y - 1, 1])
                                    board_array_master[x, y, 7] += 1;

                            }
                        }
                    }
                    //2 down
                    if (y - 3 >= 0)
                    {
                        if ((board_array_master[x, y, 1] == board_array_master[x, y - 2, 1]) && (board_array_master[x, y, 1] == board_array_master[x, y - 3, 1]))
                        {
                            if (board_array_master[x, y - 2, 11] == 0 && board_array_master[x, y - 3, 11] == 0)//is not busy
                            {
                                if (board_array_master[x, y, 7] == 0)
                                {
                                    number_of_moves_possible++;
                                    board_array_master[x, y, 5] += 1;
                                }
                                board_array_master[x, y, 7] += 2;
                            }
                        }
                    }
                }
                #endregion
            


            //count myself in the explosion
            if (board_array_master[x, y, 6] > 0)
                board_array_master[x, y, 6]++;
            if (board_array_master[x, y, 7] > 0)
                board_array_master[x, y, 7]++;
            if (board_array_master[x, y, 8] > 0)
                board_array_master[x, y, 8]++;
            if (board_array_master[x, y, 9] > 0)
                board_array_master[x, y, 9]++;
        }
        else if (board_array_master[x, y, 4] > 0) //if there is a bonus on the board
        {
            if (myRuleset.trigger_by_select == Ruleset.trigger_by.click)
                clickable_bonus_on_boad = true;
            //else if (trigger_by_select == trigger_by.switch_adjacent_gem)
            //{
                //if this bonus can be trigger by a gem
            //    switchable_bonus_on_boad = true;
            //}
            else if (myRuleset.trigger_by_select == Ruleset.trigger_by.free_switch)
            {
                //if this bonus can be move
                free_switchable_bonus_on_boad = true;
            }
        }


    }




}
