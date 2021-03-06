﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    //public GameObject player1Prefab;
    //public GameObject player2Prefab;
    //public GameObject player3Prefab;
    //public GameObject enemy1Prefab;

    public Transform player1Spawn;
    public Transform player2Spawn;
    public Transform enemy1Spawn;

    Unit playerUnit;
    Unit enemyUnit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // Instantiating the prefab and locataion for player character 1


        Vector3 player1pos = player1Spawn.transform.position;
        Quaternion player1rot = player1Spawn.transform.rotation;
        GameObject player1GO = Instantiate(Resources.Load("cloud_prefab") as GameObject);
        player1GO.transform.position = player1pos;
        player1GO.transform.rotation = player1rot;
        playerUnit = player1GO.GetComponent<Unit>();

        // Instantiating the prefab and locataion for player character 2
        Vector3 player2pos = player2Spawn.transform.position;
        Quaternion player2rot = player2Spawn.transform.rotation;
        GameObject player2GO = Instantiate(Resources.Load("tifa_prefab") as GameObject);
        player2GO.transform.position = player2pos;
        player2GO.transform.rotation = player2rot;
        // CREATE UNIT REF HERE






        // Instantiating the prefab and locataion for enemy character 1
        Vector3 enemy1pos = enemy1Spawn.transform.position;
        Quaternion enemy1rot = enemy1Spawn.transform.rotation;
        GameObject enemy1GO = Instantiate(Resources.Load("sephiroth_prefab") as GameObject);
        enemy1GO.transform.position = enemy1pos;
        enemy1GO.transform.rotation = enemy1rot;
        enemyUnit = enemy1GO.GetComponent<Unit>();
        // CREATE UNIT REF HERE



        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        Debug.Log("Attack successful");

        // Hide the player Actions options after selection is made
        playerHUD.DisableActions(playerHUD);

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.HealDamage(playerUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);
        Debug.Log("Heal successful");

        // Hide the player Actions options after selection is made
        playerHUD.DisableActions(playerHUD);

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    IEnumerator EnemyAttack()
    {
        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);
        Debug.Log("Attack successful");

        // Hide the enemy's Actions options after selection is made
        enemyHUD.DisableActions(enemyHUD);

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EnemyHeal()
    {
        enemyUnit.HealDamage(enemyUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        Debug.Log("Heal successful");

        // Hide the enemy's Actions options after selection is made
        enemyHUD.DisableActions(enemyHUD);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    //IEnumerator EnemyTurn()
    void EnemyTurn()
    {
        // enemy logic
        Debug.Log("Enemy's turn");

        // enable the enemy Actions when their turn starts
        enemyHUD.EnableActions(enemyHUD);


        //yield return new WaitForSeconds(2f);
    }

    void PlayerTurn()
    {
        Debug.Log("Choose an action:");

        // enable the player's Actions when their turn starts
        playerHUD.EnableActions(playerHUD);
    }

    public void OnPlayerAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnPlayerHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    // This is used for debugging until the enemy logic is created
    public void OnEnemyAttackButton()
    {
        if (state != BattleState.ENEMYTURN)
            return;

        StartCoroutine(EnemyAttack());
    }

    public void OnEnemyHealButton()
    {
        if (state != BattleState.ENEMYTURN)
            return;

        StartCoroutine(EnemyHeal());
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You won the battle!");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You were defeated!");
        }
    }
}