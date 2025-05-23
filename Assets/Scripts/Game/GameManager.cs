using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<DayTime> dayTimeList;

    [Header("Internal")]
    public GameState currentState;
    public int currentLevel = 0;
    public int currentDayTimeInd = 0;
    public List<BaseZombieSpawnInfo> zombieSpawnInfos;
    public int zombiesAggressionPoints = 0;
    public float zombieTurnDelay = 1;
    public GameObject gameOver;

    public static GameManager I;

    void Awake() {
        if (I != null) Destroy(gameObject);
        else I = this;
        Messenger<BaseZombie>.AddListener(EventMessages.EVALUATE_ZOMBIE_SPEED, z => GetCurrentDayTime() switch {
            DayTime.Day => -0.5f,
            DayTime.Night => 0.25f,
            _ => 0,
        });
        Messenger<BaseZombie>.AddListener(EventMessages.EVALUATE_ZOMBIE_DAMAGE, z => GetCurrentDayTime() switch {
            DayTime.Day => -1,
            DayTime.Night => 1,
            _ => 0,
        });
        zombieSpawnInfos = Resources.LoadAll<BaseZombieSpawnInfo>("ZombieSpawnInfos").ToList();
    }

    void Start() {
        ChangeState(GameState.PrepareGame);
    }

    void ChangeStateInternal(GameState newState) {
        currentState = newState;
        switch (currentState) {
            case GameState.PrepareGame:
                AudioManager.I.StartMusic();
                gameOver.SetActive(false);
                Player.I.SetDeckToBase();
                Player.I.digitalCurrency = 20;
                currentLevel = 0;
                currentDayTimeInd = 0;
                ChangeState(GameState.PrepareLevel);
                break;
            case GameState.PrepareLevel:
                GridManager.I.GenerateGrid();
                Player.I.playerHp = Player.I.playerMaxHp;
                Player.I.ShuffleDeck();
                currentLevel += 1;
                SetZombieAggressionPoints();
                ChangeState(GameState.PlayerTurn);
                break;
            case GameState.PlayerTurn:
                MenuManager.I.ShowNewTurnText(GameState.PlayerTurn);
                Player.I.OnPlayerTurnStart();
                break;
            case GameState.ZombieTurn:
                MenuManager.I.ShowNewTurnText(GameState.ZombieTurn);
                StartCoroutine(PerformZombieTurn());
                break;
            case GameState.ChangeTime:
                currentDayTimeInd = (currentDayTimeInd + 1) % dayTimeList.Count;
                Messenger<DayTime>.Broadcast(EventMessages.ON_DAYTIME_CHANGE, dayTimeList[currentDayTimeInd]);
                ChangeState(GameState.PlayerTurn);
                break;
            case GameState.Shopping:
                Player.I.ShuffleDiscardToDeck();
                ShopManager.I.InitializeShop();
                break;
            case GameState.Defeated:
                AudioManager.I.StopMusic();
                gameOver.SetActive(true);
                break;
        }
    }

    public void ChangeState(GameState newState) {
        IEnumerator ChangeStateCoroutine() {
            ChangeStateInternal(newState);
            yield return null;
        }
        StartCoroutine(ChangeStateCoroutine());
    }

    IEnumerator PerformZombieTurn() {
        AllZombiesProgress();
        yield return new WaitForSeconds(zombieTurnDelay);
        yield return AllZombiesTakeTurn();
        if (currentState != GameState.ZombieTurn) yield break;
        GridManager.I.SetZombieVisibility();
        bool zombiesExists = GridManager.I.GetZombiesCount() > 0;
        SpawnWaveZombies(zombiesExists);
        if (currentState == GameState.ZombieTurn) ChangeState(GameState.ChangeTime);
    }

    public void AllZombiesProgress() {
        List<BaseZombie> zombies = GridManager.I.GetAllZombies();
        zombies.ForEach(z => z.Progress());
    }

    IEnumerator AllZombiesTakeTurn() {
        while (true) {
            if (currentState != GameState.ZombieTurn) yield break;
            List<BaseZombie> zombiesCanTakeTurn = GridManager.I.GetAllZombies().Where(z => z.CanTakeTurn()).ToList();
            if (zombiesCanTakeTurn.Count == 0) yield break;
            zombiesCanTakeTurn.ForEach(z => z.TakeTurn());
            yield return new WaitForSeconds(zombieTurnDelay / 2);
            if (currentState != GameState.ZombieTurn) yield break;
            Messenger.Broadcast(EventMessages.ON_ALL_ZOMBIE_TAKE_TURN);
            yield return new WaitForSeconds(zombieTurnDelay / 2);
        }
    }

    public BaseZombieSpawnInfo SelectZombieToSpawn(int aggressionPoints) {
        List<BaseZombieSpawnInfo> canSpawnZombies = zombieSpawnInfos.Where(z => z.prefab.aggressionCost <= aggressionPoints && z.CanSpawn()).OrderBy(_ => Random.value).ToList();
        if (canSpawnZombies.Count > 0) return canSpawnZombies.First();
        return null;
    }

    public DayTime GetCurrentDayTime() {
        return dayTimeList[currentDayTimeInd];
    }

    public void SpawnWaveZombies(bool zombiesExists) {
        int numZombiesToSpawn = 0;
        double baseChance = 0, chancePerLevel = 0, timesToCheckPerLevel = 0, chanceNotToWaistPoints = 0;
        switch (GetCurrentDayTime()) {
            case DayTime.Day:
                if (zombiesExists) { numZombiesToSpawn = Random.value <= .2 ? 1 : 0; baseChance = .04; chancePerLevel = .01; timesToCheckPerLevel = .2; }
                else { numZombiesToSpawn = 1; baseChance = .1; chancePerLevel = .02; timesToCheckPerLevel = .5; }
                break;
            case DayTime.Evening:
            case DayTime.Morning:
                if (zombiesExists) { numZombiesToSpawn = Random.value <= .4 ? 1 : 0; baseChance = .08; chancePerLevel = .02; timesToCheckPerLevel = .4; }
                else { numZombiesToSpawn = 1; baseChance = .15; chancePerLevel = .03; timesToCheckPerLevel = 1; }
                chanceNotToWaistPoints = .005;
                break;
            case DayTime.Night:
                if (zombiesExists) { numZombiesToSpawn = Random.value <= .6 ? 1 : 0; baseChance = .12; chancePerLevel = .03; timesToCheckPerLevel = .6; }
                else { numZombiesToSpawn = 1; baseChance = .2; chancePerLevel = .04; timesToCheckPerLevel = 1.5; }
                chanceNotToWaistPoints = .01;
                break;
        }
        for (int i = 0; i < Math.Ceiling(currentLevel * timesToCheckPerLevel); ++i) numZombiesToSpawn += (Random.value <= baseChance + chancePerLevel * currentLevel) ? 1 : 0;
        for (int i = 0; i < numZombiesToSpawn; ++i) {
            BaseZombieSpawnInfo zombieSpawnInfo = SelectZombieToSpawn(zombiesAggressionPoints);
            if (zombieSpawnInfo == null) break;
            if (Random.value > Math.Min(currentLevel, 80) * chanceNotToWaistPoints) zombiesAggressionPoints -= zombieSpawnInfo.prefab.aggressionCost;
            Tile tileToSpawn = GridManager.I.GetTileToSpawnZombie();
            BaseZombie.SpawnZombie(zombieSpawnInfo.prefab, tileToSpawn);
        }
    }

    void SetZombieAggressionPoints() {
        zombiesAggressionPoints = Player.I.deck.Sum(c => c.cardData.aggressionCost);
    }

    public void ChangeStateToPrepareGame() {
        ChangeState(GameState.PrepareGame);
    }
}

public enum GameState {
    PrepareGame,
    PrepareLevel,
    PlayerTurn,
    ZombieTurn,
    ChangeTime,
    Shopping,
    Defeated
}

public enum DayTime {
    Day,
    Evening,
    Night,
    Morning
}
