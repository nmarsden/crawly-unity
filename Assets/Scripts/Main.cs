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
    GameObject pickups;
    GameObject hud;

    bool isShowTitleScreen;
    bool isShowHelpScreen;
    bool isGameOver;
    bool isGameCompleted;
    bool isOkClicked;

    int numberOfLevels;
    int currentLevelNum;
    bool isPaused;

    GameObject titleScreen;
    GameObject helpScreen;

    bool isShowDeathSequence;
    float deathSequenceStartTime;
    float deathSequenceDuration = 2f;

    void Start() {
        currentLevelNum = 1;
        isPaused = false;

        // -- Console --
        var console = new GameObject();
        console.name = "Console";
        console.AddComponent<Consolation.Console>();
        
        // -- Audio Controller --
        var audio = new GameObject();
        audio.name = "Audio Controller";
        audio.AddComponent<AudioController>();
        audioController = audio.GetComponent<AudioController>();

        // -- Camera Controller
        Camera camera = Camera.main;
        cameraController = camera.gameObject.AddComponent<CameraController>();

        // Uncomment this to Zoom in
        //camera.orthographicSize = 15;

        // Uncomment this to Zoom out
        // camera.orthographicSize = 40;

        // -- Levels --
        levels = new GameObject();
        levels.name = "Levels";
        numberOfLevels = levels.AddComponent<Levels>().GetNumberOfLevels();

        // -- Setup Title Screen --
        titleScreen = new GameObject();
        titleScreen.name = "Title Screen";
        titleScreen.AddComponent<TitleScreen>();
        titleScreen.GetComponent<TitleScreen>().Init(this, numberOfLevels);

        // -- Setup Help Screen --
        helpScreen = new GameObject();
        helpScreen.name = "Help Screen";
        helpScreen.AddComponent<HelpScreen>();
        helpScreen.GetComponent<HelpScreen>().Init(this);

        // -- HUD --
        hud = new GameObject();
        hud.name = "HUD";
        hud.AddComponent<HUD>();
        hud.GetComponent<HUD>().Init(this);

        // -- Show Title Screen --
        ShowTitleScreen();
    }

    void ShowTitleScreen()
    {
        isShowTitleScreen = true;
        titleScreen.GetComponent<TitleScreen>().Show(currentLevelNum);

        // Play Music
        audioController.PlayMusic();
    }

    void HideTitleScreen() {
        isShowTitleScreen = false;
        titleScreen.GetComponent<TitleScreen>().Hide();
    }

    void ShowHelpScreen()
    {
        isShowHelpScreen = true;
        helpScreen.GetComponent<HelpScreen>().Show();

        // Play Music
        audioController.PlayMusic();
    }

    void HideHelpScreen() {
        isShowHelpScreen = false;
        helpScreen.GetComponent<HelpScreen>().Hide();
    }

    void ShowHUD()
    {
        hud.GetComponent<HUD>().Show(currentLevelNum);
    }

    void HideHUD()
    {
        hud.GetComponent<HUD>().Hide();
    }

    void StartLevel(int selectedLevelNumber)
    {
        currentLevelNum = selectedLevelNumber;
        Time.timeScale = 1;
        isGameOver = false;
        isGameCompleted = false;
        isPaused = false;
        isOkClicked = false; 
        isShowDeathSequence = false;

        // Play Music
        audioController.PlayMusic();

        // -- Levels --
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

        // -- Pickups --
        pickups = new GameObject();
        pickups.name = "Pickups";
        pickups.AddComponent<Pickups>();
        pickups.GetComponent<Pickups>().Init(this);

        // -- Show HUD --
        ShowHUD();
    }

    void Update()
    {
        if (!isShowTitleScreen && !isShowHelpScreen) {
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
                if (isShowDeathSequence) {
                    // -- Game Over - Showing Death Sequence --
                    if (Time.time - deathSequenceStartTime > deathSequenceDuration) {
                        isShowDeathSequence = false;

                        // Pause Game
                        Time.timeScale = 0;

                        // Display Game Over message
                        hud.GetComponent<HUD>().ShowGameOverMessage();
                    }
                } else {
                    // -- Game Over - Showing Game Over Message --
                    // Reset when 'Space' or 'Return' pressed
                    if (isOkClicked || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) 
                    {
                        Reset();
                    }
                }
            } else if (isGameCompleted) {
                // -- Game Completed --
                // Start Next Level when 'Space' or 'Return' pressed
                if (isOkClicked || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) 
                {
                    StartNextLevel();
                }
            } else if (isPaused) {
                // Unpause when when 'Space' or 'Return' pressed
                if (isOkClicked || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) 
                {
                    Unpause();
                }
            } else {
                // -- Game In Progress --
                // Pause when 'P' pressed
                if (Input.GetKeyDown(KeyCode.P)) {
                    Pause();
                }
                // Reset when 'R' pressed
                if (Input.GetKeyDown(KeyCode.R)) 
                {
                    Reset();
                } 
            }
        }

    }

    void StartNextLevel() {
        currentLevelNum++;
        if (currentLevelNum > numberOfLevels) {
            currentLevelNum = 1;
        }
        Reset();
    }
    
    void Reset() {
        DestroyAllCreatedObjects();
        StartLevel(currentLevelNum);
    }

    void DestroyAllCreatedObjects() 
    {
        Object.Destroy(pickups);
        Object.Destroy(player);
        Object.Destroy(arena);
        Object.Destroy(turningPoints);
    }

    void QuitGame() {
        HideHUD();
        DestroyAllCreatedObjects();
        ShowTitleScreen();
    }

    void ToggleView() {
        cameraController.ToggleCameraMode();
    }

    void Pause() {
        isPaused = true;
        Time.timeScale = 0;

        // Display Paused message
        hud.GetComponent<HUD>().ShowPausedMessage();
    }

    void Unpause() {
        isOkClicked = false;
        isPaused = false;
        Time.timeScale = 1;

        // Hide Paused message
        hud.GetComponent<HUD>().HidePausedMessage();
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

    public void HandlePlayButtonPressed(int selectedLevelNumber) {
        HideTitleScreen();

        StartLevel(selectedLevelNumber);
    }

    public void HandleHelpButtonPressed() {
        HideTitleScreen();
        ShowHelpScreen();
    }

    public void HandleHelpCloseButtonPressed() {
        HideHelpScreen();
        ShowTitleScreen();
    }
 
    public void HandleHudOkButtonClicked() {
        isOkClicked = true;
    }

    public void HandleHitPickup(Pickup pickup) 
    {
        // Play pickup FX
        audioController.PlayPickupFX();

        // Eat pickup
        pickup.Eat();

        if (pickup.isFood()) 
        {
            // -- Handle Hit Food --
            // Grow player
            player.GetComponent<Player>().Grow();
        }
        else if (pickup.isPoison()) 
        {
            // -- Handle Hit Poison --
            // Shrink player
            if (player.GetComponent<Player>().IsShrinkable()) 
            {
                player.GetComponent<Player>().Shrink();
            } 
            else 
            {
                GameOver();
            }
        }
    }

    public void HandlePickupAppear(Pickup pickup) 
    {
        if (pickup.isFood()) {
            audioController.PlayFoodAppearFX();

        } else if (pickup.isPoison()) {
            audioController.PlayPoisonAppearFX();
        }
    }

    public void HandlePickupDisappear(Pickup pickup) 
    {
        if (pickup.isFood()) {
            audioController.PlayFoodDisappearFX();

        } else if (pickup.isPoison()) {
            audioController.PlayPoisonDisappearFX();
        }
    }

    public void HandleHitWall() {
        GameOver();
    }

    public void HandleHitTail() {
        GameOver();
    }

    public void HandleButtonPressedFX() {
        audioController.PlayActivateFX();
    }

    public void HandleCellActivated() {
        audioController.PlayActivateFX();
    }

    public void HandleAllCellsActivated() {
        GameCompleted();    
    }
    
    private void GameCompleted() {
        if (isGameOver) {
            return;
        }

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
        if (isGameOver) {
            return;
        }
        // Switch to Game Over mode
        isGameOver = true; 

        // De-active Pickups
        pickups.SetActive(false);

        // Show Death Sequence
        isShowDeathSequence = true;
        deathSequenceStartTime = Time.time;
        player.GetComponent<Player>().Kill();

        // Stop music & play game over FX
        audioController.StopMusic();
        audioController.PlayGameOverFX();
    }

    public List<Vector3> GetEmptyPositions() {
        var arenaScript = arena.GetComponent<Arena>();

        var emptyPositions = arenaScript.GetEmptyArenaPositions();
        var pickupPositions = arenaScript.ToArenaPositions(pickups.GetComponent<Pickups>().GetPositions());
        emptyPositions.RemoveAll(v => pickupPositions.Contains(v));

        return arenaScript.ToCellPositions(emptyPositions);
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
