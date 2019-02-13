using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public enum PlayMode { STANDARD, DEMO };

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
    GameObject arena;
    GameObject player;
    GameObject pickups;
    GameObject hud;

    bool isShowTitleScreen;
    bool isGameOver;
    bool isGameCompleted;
    bool isOkClicked;
    bool isDemoMode;

    int numberOfLevels;
    int currentLevelNum;
    bool isPaused;

    GameObject titleScreen;

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

        // -- Lights
        var lights = new GameObject();
        lights.name = "Lights";
        lights.AddComponent<Lights>();

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
        Time.timeScale = 1;
        isShowTitleScreen = true;
        titleScreen.GetComponent<TitleScreen>().Show(currentLevelNum);

        // Play Music
        audioController.PlayMusic();
    }

    void HideTitleScreen() {
        isShowTitleScreen = false;
        titleScreen.GetComponent<TitleScreen>().Hide();
    }

    void ShowHUD(PlayMode playMode)
    {
        hud.GetComponent<HUD>().Show(currentLevelNum, playMode);
    }

    void HideHUD()
    {
        hud.GetComponent<HUD>().Hide();
    }

    void StartLevel(int selectedLevelNumber, PlayMode playMode)
    {
        currentLevelNum = selectedLevelNumber;
        Time.timeScale = 1;
        isGameOver = false;
        isGameCompleted = false;
        isPaused = false;
        isOkClicked = false; 
        isShowDeathSequence = false;
        isDemoMode = playMode.Equals(PlayMode.DEMO);

        // Deactivate camera Fly Around mode (if active)
        cameraController.DeactivateFlyAroundMode();

        // Play Music
        audioController.PlayMusic();

        // -- Levels --
        levels.GetComponent<Levels>().Init(currentLevelNum);

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
        if (isDemoMode) {
            player.GetComponent<Player>().ToggleAutopilotMode();
        }

        // -- Pickups --
        pickups = new GameObject();
        pickups.name = "Pickups";
        pickups.AddComponent<Pickups>();
        pickups.GetComponent<Pickups>().Init(this);

        // -- Show HUD --
        ShowHUD(playMode);
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
        StartLevel(currentLevelNum, PlayMode.STANDARD);
    }

    void ToggleAutopilotMode() {
        player.GetComponent<Player>().ToggleAutopilotMode();
    }

    void DestroyAllCreatedObjects() 
    {
        Object.Destroy(pickups);
        Object.Destroy(player);
        Object.Destroy(arena);
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

    void StartDemoMode() {
        StartLevel(1, PlayMode.DEMO);
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
    
    public void HandleHeadCreated(GameObject head) 
    {
        cameraController.Init(head);
        if (isDemoMode) {
            cameraController.ActivateFlyAroundMode();
        }
    }

    public void HandleHeadEnteredCell(Vector3 position)
    {
        var arenaPosition = arena.GetComponent<Arena>().ToArenaPosition(position);
        player.GetComponent<Player>().HeadEnteredCell(arenaPosition.row, arenaPosition.col);
    }

    public void HandlePlayButtonPressed(int selectedLevelNumber) {
        HideTitleScreen();

        StartLevel(selectedLevelNumber, PlayMode.STANDARD);
    }

    public void HandleDemoButtonPressed() {
        HideTitleScreen();

        StartDemoMode();
    }

    public void HandleHudOkButtonClicked() {
        isOkClicked = true;
    }

    public void HandleHitPickup(Pickup pickup) 
    {

        // Eat pickup
        pickup.Eat();

        if (pickup.isFood()) 
        {
            // -- Handle Hit Food --
            // Play pickup food sound FX
            audioController.PlayPickUpFoodFX();
            // Grow player
            player.GetComponent<Player>().Grow();
        }
        else if (pickup.isPoison()) 
        {
            // -- Handle Hit Poison --
            if (player.GetComponent<Player>().IsNotShrinkable()) {
                // Play destroy poison sound FX
                audioController.PlayDestroyPoisonFX();
            } else {
                // Play pickup poison sound FX
                audioController.PlayPickUpPoisonFX();
                // Shrink player
                player.GetComponent<Player>().Shrink();
                // Shake camera
                cameraController.Shake(1);
            }
        }
        else if (pickup.isShield()) 
        {
            // -- Handle Hit Shield --
            // Play pickup shield sound FX
            audioController.PlayPickUpShieldFX();
            // Shield player
            player.GetComponent<Player>().Shield();
        }
    }

    public void HandlePickupAppear(Pickup pickup) 
    {
        audioController.PlayPickupAppearFX();
    }

    public void HandlePickupDisappear(Pickup pickup) 
    {
        audioController.PlayPickupDisappearFX();
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

        // Active camera Fly Around mode
        cameraController.ActivateFlyAroundMode();
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

        // Active camera Fly Around mode
        cameraController.ActivateFlyAroundMode();
        // Shake camera
        cameraController.Shake(1);
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

    public float GetFilledPercentage() {
        return (arena.GetComponent<Arena>().GetNumberOfActivatedCells() / (float) GetNumberOfFillableCells());
    }

    public Arena.CellType[,] GetLevelCellTypes() {
        return levels.GetComponent<Levels>().GetLevelCellTypes();
    }

    public void UpdateShieldStatusBar(float value) {
        hud.GetComponent<HUD>().UpdateShieldStatusValue(value);
    }
}
