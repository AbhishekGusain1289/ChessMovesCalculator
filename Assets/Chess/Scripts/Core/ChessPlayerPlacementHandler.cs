using System;
using System.Collections;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {

        [SerializeField] public int row, column;
        private static string Piecename;
        static float[][] moves = new float[27][];



        private void Start()
        {
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
        }

        private void Update()
        {
            GettingPosition();
        }
        private void OnMouseDown()
        {
            Piecename = gameObject.name;
            Clearing();
            PawnRayCasting();
            QueenRayCasting();
            KingMovement();
            KnightMovement();
            Highlighting(moves);
            ClearMovesArray();
        }


        void ClearMovesArray()
        {
            for (int i = 0; i < moves.Length; i++)
            {
                moves[i] = null;
            }
        }


        private void GettingPosition()
        {
            float x = transform.position.x;
            float y = transform.position.y;
            if (x < 0)
            {
                x = 4 + Mathf.Floor(x);
            }
            else
                x = Mathf.Ceil(x) + 3;
            if (y < 0)
            {
                y = 4 + Mathf.Floor(y);
            }
            else
                y = Mathf.Ceil(y) + 3;

            row = (int)y;
            column = (int)x;
        }

        void Clearing()
        {
            ChessBoardPlacementHandler.Instance.ClearHighlights();
        }

        void PawnRayCasting()
        {
            if (Piecename == "Pawn")
            {
                RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.up, 2f);
                if (ray.collider != null)
                {
                    float hitDistance = ray.distance;
                    Debug.DrawRay(transform.position, Vector2.up * hitDistance, Color.green);
                    if (hitDistance > 2)
                    {

                        if (row == 1)
                        {
                            PawnRowOne();
                            return;
                        }
                    }
                    if (hitDistance < 2 && hitDistance >= 1)
                    {
                        PawnRowOther();
                    }
                }
                else
                {
                    float maxRayLength = 2f;
                    Debug.DrawRay(transform.position, Vector2.up * maxRayLength, Color.white);
                    if (row == 1)
                    {
                        PawnRowOne();
                    }
                    else if (row > 1 && row < 7)
                    {
                        PawnRowOther();
                    }
                }

            }

        }

        void PawnRowOne()
        {
            moves[0] = new float[] { row + 1, column };
            moves[1] = new float[] { row + 2, column };
        }

        void PawnRowOther()
        {
            moves[0] = new float[] { row + 1, column };
        }

        void Highlighting(float[][] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                float row;
                float column;
                if (arr[i] != null)
                {
                    float[] pair = arr[i];
                    row = pair[0];
                    column = pair[1];
                    ChessBoardPlacementHandler.Instance.Highlight((int)row, (int)column);
                }
            }

            
        }
        void QueenRayCasting()
        {
            switch (Piecename)
            {
                case "Queen":
                    MakingRays(8,0f);
                    break;
                case "Rook":
                    MakingRays(4,0f);
                    break;
                case "Bishop":
                    MakingRays(4, 45);
                    break;
            
            }

        }


        void MakingRays(int numOfRays,float StartingAngle)
        {
            float increaseBy = 360 / numOfRays;
            int index = 0;
            RaycastHit2D[] rays = new RaycastHit2D[numOfRays];
            for (int i = 0; i < numOfRays; i++)
            {

                float angleInRadians = StartingAngle * Mathf.Deg2Rad;
                Vector2 angle = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
                rays[i] = Physics2D.Raycast(transform.position, angle, 15f);
                float hitDistance = rays[i].distance;
                if (rays[i].collider != null)
                {
                    Debug.DrawRay(transform.position, angle * hitDistance, Color.green);
                    float loops = Mathf.Floor(hitDistance);
                    Vector2 dir = angle.normalized;
                    if (StartingAngle % 2 == 0)
                    {
                        
                        for (int k = 0; k < loops; k++)
                        {

                            AddingStraightsLines(dir,index,k);

                            index++;
                        }

                    }
                    else if(StartingAngle %2!=0)
                    {
                        float result = Mathf.Sin(angleInRadians) * hitDistance;
                        loops=Mathf.Floor(Mathf.Abs(result)); 
                        for(int k=0; k < loops; k++)
                        {
                            AddingDiagonals(dir, index, k);
                            index++;
                        }
                    }
                    StartingAngle += increaseBy;
                    

                }
            }
        }

        void AddingStraightsLines(Vector2 dir,int index,int k)
        {
            float epsilon = 0.00001f;

            if (Mathf.Abs(dir.x - 1f) < epsilon && Mathf.Abs(dir.y) < epsilon)
            {
                moves[index] = new float[] { row, column + k + 1 };
            }
            else if (Mathf.Abs(dir.x) < epsilon && Mathf.Abs(dir.y - 1f) < epsilon)
            {
                moves[index] = new float[] { row + k + 1, column };
            }
            else if (Mathf.Abs(dir.x + 1f) < epsilon && Mathf.Abs(dir.y) < epsilon)
            {
                moves[index] = new float[] { row, column - k - 1 };
            }
            else if (Mathf.Abs(dir.x) < epsilon && Mathf.Abs(dir.y + 1f) < epsilon)
            {
                moves[index] = new float[] { row - k - 1, column };
            }
            else
            {
                Debug.Log("dir is not one of the specified cases");
            }


        }
        void AddingDiagonals(Vector2 dir,int index, int k)
        {
            if (dir.x > 0)
            {
                if (dir.y > 0)
                    moves[index] = new float[] { row + k + 1, column + k + 1 };
                else
                    moves[index] = new float[] { row - k - 1, column + k + 1 };
            }
            else if (dir.x < 0)
            {
                if (dir.y > 0)
                    moves[index] = new float[] { row + k + 1, column - k - 1 };
                else
                {
                    moves[index] = new float[] { row - k - 1, column - k - 1 };
                }
            }
        }

        void KingMovement()
        {
            if (name == "King")
            {

                RaycastHit2D[] rays= new RaycastHit2D[8];
                for (int i=0;i<8;i++)
                {
                    float angleInRadians = (i*45) * Mathf.Deg2Rad;
                    Vector2 angle = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
                    rays[i] = Physics2D.Raycast(transform.position, angle, 1f);
                    Debug.DrawRay(transform.position, rays[i].point * 1f, Color.green);
                    if(rays[i].collider==null)
                    {
                        if((i*45)%2==0)
                        {
                            AddingStraightsLines(angle.normalized, i, 0);
                        }
                        if((i*45)%2!=0)
                        {
                            AddingDiagonals(angle.normalized, i, 0);
                        }
                    }
                
                }
            }

        }

        void KnightMovement()
        {
            if(name=="Knight")
            {
                int index = 0;
                for (int i = 0; i < 2; i++)
                {
                    int knightRow=row;
                    int knightCol=column;
                    
                    if(i%2==0)
                    {
                        index = ChangingKnightRows(index, knightRow, knightCol, 1,1);
                        index = ChangingKnightRows(index, knightRow, knightCol, 1,-1);
                    }
                    else
                    {
                        index = ChangingKnightRows(index, knightCol, knightRow, -1,1);
                        index = ChangingKnightRows(index, knightCol, knightRow, -1,-1);
                    }
                   
                    
                }

            }
        }

        int ChangingKnightRows(int index,int toIncreaseTwice,int toIncreaseOnce,int axis,int direction)
        {
            toIncreaseTwice += 2*direction;
            if(toIncreaseTwice <=7 && toIncreaseTwice>=0)
            {
                    toIncreaseOnce += 1;
                    if (toIncreaseOnce <= 7)
                    {
                        index=CheckingAxis(index,toIncreaseTwice,toIncreaseOnce,axis);
                    }
                    toIncreaseOnce -= 2;
                    if (toIncreaseOnce >= 0)
                    {
                        index=CheckingAxis(index, toIncreaseTwice, toIncreaseOnce, axis);
                    }
                    
                
            }
            return index;
        }

        int AddingMoves(int index,int toRow,int toCol)
        {
            GameObject Object=ChessBoardPlacementHandler.Instance.GetTile(toRow, toCol);
            Vector2 pos = Object.transform.position;
            Collider2D collider = Physics2D.OverlapPoint(pos);

            if (collider==null)
            {
                moves[index]= new float[] {toRow,toCol};
                index++;
                return index;
            }
            return index;
        }

        int CheckingAxis(int index,int row,int col,int axis)
        {
            if (axis == 1)
                index = AddingMoves(index, row, col);
            else
                index = AddingMoves(index, col, row);
            return index;
        }


    }




}
