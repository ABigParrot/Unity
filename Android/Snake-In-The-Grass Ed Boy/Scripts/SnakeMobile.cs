/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class SnakeMobile : MonoBehaviour {

    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }

    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;

    public float swipeRange;
    public float tapRange;
    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
    }

    private void Awake() {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = .25f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }

    private void Update()
    {

        //Implementing the alive and dead states of the snake
        switch (state) {
            case State.Alive:
                //HandleInput();
                Swipes();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }

    //Applies input. Alter code for keys or touch
    private void HandleInput() {
        if (SwipeManager.swipeUp) {
            if (gridMoveDirection != Direction.Down) {
                gridMoveDirection = Direction.Up;
            }
        }
        if (SwipeManager.swipeDown) {
            if (gridMoveDirection != Direction.Up) {
                gridMoveDirection = Direction.Down;
            }
        }
        if (SwipeManager.swipeLeft) {
            if (gridMoveDirection != Direction.Right) {
                gridMoveDirection = Direction.Left;
            }
        }
        if (SwipeManager.swipeRight) {
            if (gridMoveDirection != Direction.Left) {
                gridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement() {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;

            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);
            
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0) {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection) {
            default:
            case Direction.Right:   gridMoveDirectionVector = new Vector2Int(+1, 0); break;
            case Direction.Left:    gridMoveDirectionVector = new Vector2Int(-1, 0); break;
            case Direction.Up:      gridMoveDirectionVector = new Vector2Int(0, +1); break;
            case Direction.Down:    gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood) {
                // Snake ate food, grow body
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound((SoundManager.Sound)Random.Range(4,9));
                //SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            UpdateSnakeBodyParts();

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList) {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartGridPosition) {
                    //Game Over
                    state = State.Dead;
                    GameHandler.SnakeDied();
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                    
                }
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

        }
    }

    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts() {
        for (int i = 0; i < snakeBodyPartList.Count; i++) {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }


    private float GetAngleFromVector(Vector2Int dir) {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    // Return the full list of positions occupied by the snake: Head + Body
    public List<Vector2Int> GetFullSnakeGridPositionList() {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList) {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }
    

    /*
     * Handles a Single Snake Body Part
     * */
    private class SnakeBodyPart {

        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;

            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection()) {
            default:
            case Direction.Up: // Currently going Up
                switch (snakeMovePosition.GetPreviousDirection()) {
                default: 
                    angle = 0; 
                    break;
                case Direction.Left: // Previously was going Left
                    angle = 0 + 45; 
                    transform.position += new Vector3(.2f, .2f);
                    break;
                case Direction.Right: // Previously was going Right
                    angle = 0 - 45; 
                    transform.position += new Vector3(-.2f, .2f);
                    break;
                }
                break;
            case Direction.Down: // Currently going Down
                switch (snakeMovePosition.GetPreviousDirection()) {
                default: 
                    angle = 180; 
                    break;
                case Direction.Left: // Previously was going Left
                    angle = 180 - 45;
                    transform.position += new Vector3(.2f, -.2f);
                    break;
                case Direction.Right: // Previously was going Right
                    angle = 180 + 45; 
                    transform.position += new Vector3(-.2f, -.2f);
                    break;
                }
                break;
            case Direction.Left: // Currently going to the Left
                switch (snakeMovePosition.GetPreviousDirection()) {
                default: 
                    angle = +90; 
                    break;
                case Direction.Down: // Previously was going Down
                    angle = 180 - 45; 
                    transform.position += new Vector3(-.2f, .2f);
                    break;
                case Direction.Up: // Previously was going Up
                    angle = 45; 
                    transform.position += new Vector3(-.2f, -.2f);
                    break;
                }
                break;
            case Direction.Right: // Currently going to the Right
                switch (snakeMovePosition.GetPreviousDirection()) {
                default: 
                    angle = -90; 
                    break;
                case Direction.Down: // Previously was going Down
                    angle = 180 + 45; 
                    transform.position += new Vector3(.2f, .2f);
                    break;
                case Direction.Up: // Previously was going Up
                    angle = -45; 
                    transform.position += new Vector3(.2f, -.2f);
                    break;
                }
                break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }

    }
    /*
     * Handles one Move Position from the Snake
     * */
    private class SnakeMovePosition {

        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction) {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() {
            return gridPosition;
        }

        public Direction GetDirection() {
            return direction;
        }

        public Direction GetPreviousDirection() {
            if (previousSnakeMovePosition == null) {
                return Direction.Right;
            } else {
                return previousSnakeMovePosition.direction;
            }
        }

    }

    private void Swipes()
    {
        //0 refers to the first input on the screen. If there is a touch and the touch phase is beginning,
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //then set the start touch position to where the finger touched the screen 
            startTouchPosition = Input.GetTouch(0).position;
        }

        //If there is a touch and the finger has moved
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            //Get the current position of the finger
            //and subtract that by the fingers start position
            //to get the distance traveled
            currentPosition = Input.GetTouch(0).position;
            Vector2 Distance = currentPosition - startTouchPosition;

            if (!stopTouch)
            {
                if (Distance.x < -swipeRange)
                {
                    if (gridMoveDirection != Direction.Right) {
                        gridMoveDirection = Direction.Left;
                        stopTouch = true;
                        Debug.Log("Left");
                    }
                }
                if (Distance.x > swipeRange)
                {
                    if (gridMoveDirection != Direction.Left) {
                        gridMoveDirection = Direction.Right;
                        stopTouch = true;
                        Debug.Log("Right");
                    }
                }
                if (Distance.y > swipeRange)
                {
                    if (gridMoveDirection != Direction.Down) {
                         gridMoveDirection = Direction.Up;
                         stopTouch = true;
                         Debug.Log("Up");
                    }
                }
                if (Distance.y < -swipeRange)
                {
                    if (gridMoveDirection != Direction.Up) {
                        gridMoveDirection = Direction.Down;
                        stopTouch = true;
                        Debug.Log("Down");
                    }
                }
            }
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            stopTouch = false;

            endTouchPosition = Input.GetTouch(0).position;

            Vector2 Distance = endTouchPosition - startTouchPosition;

            if (Mathf.Abs(Distance.x) < tapRange && Mathf.Abs(Distance.y) < tapRange)
            {
                Debug.Log("Tap;");
            }
        }
    }
}
/*
 if (SwipeManager.swipeUp) {
    if (gridMoveDirection != Direction.Down) {
        gridMoveDirection = Direction.Up;
    }
}
if (SwipeManager.swipeDown) {
    if (gridMoveDirection != Direction.Up) {
        gridMoveDirection = Direction.Down;
    }
}
if (SwipeManager.swipeLeft) {
    if (gridMoveDirection != Direction.Right) {
        gridMoveDirection = Direction.Left;
    }
}
if (SwipeManager.swipeRight) {
    if (gridMoveDirection != Direction.Left) {
        gridMoveDirection = Direction.Right;
    }
}
        */