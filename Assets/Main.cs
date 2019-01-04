using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    const float GRID_SPACING = 7f;
    const float PLAYER_HEIGHT = 5f;
    const float PLAYER_WIDTH = 5f;
    const float PLAYER_SPEED = 10f;
    const float TAIL_MIN_DISTANCE = 3f;
    const float WALL_HEIGHT = 2.5f;
    const bool IS_SHOW_TURNING_POINTS = false;
    const bool IS_SHOW_CELL_TRIGGERS = false;

    AudioController audioController;
    CameraController cameraController;
    GameObject levels;
    GameObject turningPoints;
    GameObject arena;
    GameObject player;
    GameObject food;
    GameObject hud;

    bool isShowTitleScreen;
    bool isGameOver;
    bool isGameCompleted;

    int currentLevelNum;
    bool isPaused;

    GameObject titleScreen;

    void Start() {
        currentLevelNum = 1;
        isPaused = false;

        // -- Audio Controller --
        var audio = new GameObject();
        audio.name = "Audio Controller";
        audio.AddComponent<AudioController>();
        audioController = audio.GetComponent<AudioController>();

        // -- Camera Controller
        Camera camera = Camera.main;
        cameraController = camera.gameObject.AddComponent<CameraController>();

        // Uncomment this to Zoom in
        // camera.orthographicSize = 15;

        // -- Setup Title Screen --
        titleScreen = new GameObject();
        titleScreen.name = "Title Screen";
        titleScreen.AddComponent<TitleScreen>();
        titleScreen.GetComponent<TitleScreen>().Init(this);

        // -- Show Title Screen --
        ShowTitleScreen();
    }

    void ShowTitleScreen()
    {
        isShowTitleScreen = true;
        titleScreen.GetComponent<TitleScreen>().Show();

        // Play Music
        audioController.PlayMusic();
    }

    void HideTitleScreen() {
        isShowTitleScreen = false;
        titleScreen.GetComponent<TitleScreen>().Hide();
    }

    void StartLevel()
    {
        Time.timeScale = 1;
        isGameOver = false;
        isGameCompleted = false;
        isPaused = false;

        // Play Music
        audioController.PlayMusic();

        // -- Levels --
        levels = new GameObject();
        levels.name = "Levels";
        levels.AddComponent<Levels>();
        levels.GetComponent<Levels>().Init(currentLevelNum);

        // -- Turning Points --
        turningPoints = new GameObject();
        turningPoints.name = "Turning Points";
        turningPoints.AddComponent<TurningPoints>();
        turningPoints.GetComponent<TurningPoints>().Init(this);

        // -- Arena --
        arena = new GameObject();
        arena.name = "Arena";
        arena.AddComponent<Arena>();
        arena.GetComponent<Arena>().Init(this);

        // -- Player --
        player = new GameObject();
        player.name = "Player";
        player.AddComponent<Player>();
        player.GetComponent<Player>().Init(this);

        // -- Food Container --
        food = new GameObject();
        food.name = "Food Container";
        food.AddComponent<Food>();
        food.GetComponent<Food>().Init(this);

        // -- HUD --
        hud = new GameObject();
        hud.name = "HUD";
        hud.AddComponent<HUD>();
        hud.GetComponent<HUD>().Init(this);
    }

    void Update()
    {
        if (!isShowTitleScreen) {
            // Toggle View when 'V' pressed
            if (Input.GetKeyDown(KeyCode.V)) 
            {
                ToggleView();
            }
            // Quit Game when 'ESC' pressed
            if (Input.GetKeyDown(KeyCode.Escape)) 
            {
                QuitGame();
            }
            if (isGameOver) {
                // -- Game Over --
                // Reset when 'Space' pressed
                if (Input.GetKeyDown(KeyCode.Space)) 
                {
                    Reset();
                }
            } else if (isGameCompleted) {
                // -- Game Completed --
                // Reset when 'Space' pressed
                if (Input.GetKeyDown(KeyCode.Space)) 
                {
                    currentLevelNum++;
                    Reset();
                }
            } else {
                // -- Game In Progress --
                // Reset when 'R' pressed
                if (Input.GetKeyDown(KeyCode.R)) 
                {
                    Reset();
                } 
                // Toggle Pause when 'P' pressed
                else if (Input.GetKeyDown(KeyCode.P)) 
                {
                    TogglePause();
                }
            }
        }

    }

    void Reset() {
        DestroyAllCreatedObjects();
        StartLevel();
    }

    void DestroyAllCreatedObjects() 
    {
        Object.Destroy(hud);
        Object.Destroy(food);
        Object.Destroy(player);
        Object.Destroy(arena);
        Object.Destroy(turningPoints);
        Object.Destroy(levels);
    }

    void QuitGame() {
        DestroyAllCreatedObjects();
        ShowTitleScreen();
    }

    void ToggleView() {
        cameraController.ToggleView();
    }

    void TogglePause() {
        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    public float GetArenaWidth() 
    {
        return GRID_SPACING * levels.GetComponent<Levels>().GetLevelWidthInCells();
    }

    public float GetGridSpacing() 
    {
        return GRID_SPACING;
    }

    public float GetPlayerHeight() 
    {
        return PLAYER_HEIGHT;
    }

    public float GetPlayerWidth() 
    {
        return PLAYER_WIDTH;
    }

    public float GetPlayerSpeed() 
    {
        return PLAYER_SPEED;
    }

    public float GetTailMinDistance() {
        return TAIL_MIN_DISTANCE;
    }

    public float GetWallHeight() {
        return WALL_HEIGHT;
    }

    public bool IsShowTurningPoints() {
        return IS_SHOW_TURNING_POINTS;
    }

    public bool IsShowCellTriggers() {
        return IS_SHOW_CELL_TRIGGERS;
    }

    public int GetCurrentLevelNum() {
        return currentLevelNum;
    }
    public string AddTurningPoint(Vector3 position, Vector3 incomingDirection, Vector3 outgoingDirection) {
        return turningPoints.GetComponent<TurningPoints>().AddTurningPoint(position, incomingDirection, outgoingDirection);
    }

    public TurningPoints GetTurningPoints() {
        return turningPoints.GetComponent<TurningPoints>();
    }

    public void HandleHeadCreated(GameObject head) 
    {
        cameraController.Init(head);
    }

    public void HandlePlayButtonPressed() {
        HideTitleScreen();

        StartLevel();
    }

    public void HandleHitFood() 
    {
        audioController.PlayPickupFX();

        // Reposition food
        food.GetComponent<Food>().Reposition();

        // Grow player
        player.GetComponent<Player>().Grow();
    }

    public void HandleHitWall() {
        GameOver();
    }

    public void HandleHitTail() {
        GameOver();
    }

    public void HandleCellActivated() {
        audioController.PlayActivateFX();
    }

    public void HandleAllCellsActivated() {
        GameCompleted();    
    }
    
    private void GameCompleted() {
        audioController.StopMusic();
        audioController.PlayCompleteFX();

        // Switch to Game Completed mode
        isGameCompleted = true;

        // Pause Game
        Time.timeScale = 0;

        // Display Game Completed message
        hud.GetComponent<HUD>().ShowGameCompletedMessage();
    }

    private void GameOver() {
        audioController.StopMusic();
        audioController.PlayGameOverFX();

        // Switch to Game Over mode
        isGameOver = true;

        // Pause Game
        Time.timeScale = 0;

        // Display Game Over message
        hud.GetComponent<HUD>().ShowGameOverMessage();
    }

    public List<Vector3> GetEmptyPositions() {
        return arena.GetComponent<Arena>().GetEmptyPositions();
    }

    public int GetNumberOfFillableCells() {
        return levels.GetComponent<Levels>().GetNumberOfFillableCells();
    }

    public int GetFilledPercentage() {
        return (int) Mathf.Floor((arena.GetComponent<Arena>().GetNumberOfActivatedCells() / (float) GetNumberOfFillableCells()) * 100);
    }

    public Arena.CellType[,] GetLevelCellTypes() {
        return levels.GetComponent<Levels>().GetLevelCellTypes();
    }
}
