using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateMachine
{
    public static GameStates GetGameState()
    {
        return Global.gm.GetGameState();
    }
    public static void SetGameState(GameStates state)
    {
        Global.gm.SetGameState(state);
    }
    public enum BuildingType
    {
        house,
        UBuilder,
        colector,
        turret
    }

    public enum GameStates
    {
        idel,
        Construction,
        Unit,
        paused,
        InBuilding
    }
    public enum Players
    {
        player1,
        player2,
        player3,
        player4,
        player5,
        player6
    }

    public enum UnitStates
    {
        guarding,
        MovingToPoint,
        idel,
        attacking,
        collecting,
        MovingToCollectionPoints
    }

    public enum UnitTypes
    {
        combat,
        worker
    }

    public enum ResourceType {
        gold,
        wood
    }

    public enum Training
    {
        idel,
        training,
        cancel,
        paused
    }
}
