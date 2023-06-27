using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameEvents
{
    public delegate void PlayerFinsihedGame(bool x);
    public delegate void PlayerStartedGame();
    public delegate void GameFreezeState(bool state);
    public delegate void GameStartSetUp();
    public delegate void GameEndSetUp(bool x);

    public static PlayerFinsihedGame finsihEndGame;
    public static PlayerStartedGame finishStartGame;
    public static GameStartSetUp gameStartSetUp;
    public static GameEndSetUp gameEndSetUp;
    public static GameFreezeState gameFreezeState;

    public delegate void SoulCollect(SoulManager sm);
    public delegate void AbilityLock(string ability, bool lockState);

    public static SoulCollect onSoulCollect;
    public static AbilityLock onAbilityLock;
}
