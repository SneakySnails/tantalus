using System.Collections;
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
    public Transform player3Spawn;
    public Transform player4Spawn;
    public Transform enemy1Spawn;
    public Transform enemy2Spawn;
    public Transform enemy3Spawn;
    public Transform enemy4Spawn;

    Unit player1Unit;
    Unit player2Unit;
    Unit enemy1Unit;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    //AAAHHHHHAHAHHAH
    public LinkedList<Unit> getUnitTurn(ArrayList<Unit> units)
    {
      LinkedList<Unit> order = new LinkedList<>();
      int[] speeds = new int[units.size()];
      for(int index = 0; index < speeds.length; index++)
      {
        int statusChange = 0;
        Unit check = units.get(index);
        if(check.status.equals("slow"))
          statusChange = SLOW_NUM;
        else if(check.status.equals("fast"))
          statusChange = FAST_NUM;
        speeds[index] = check.speed + statusChange;
      }
      //increment stuff, can do mulitple times
      for(int index = 0; index < speeds.length; index++)
      {
        int check = speeds[index];
        if(check == MAX_INCREMENT) //99
        {
          order.addLast(units.get(index));
          //OR unitTurn method for example - turn(units.get(index))
          speeds[index] = check - MAX_INCREMENT; 
        }
        else
          speeds[index] = check++; //possible multiplier here
      }
      return order;
    }
    
    IEnumerator SetupBattle()
    {
        /**
        Ideally this part of setting up the battle is done in BattleEncounter, where the Units will be sent here
        **/

        // Instantiating the prefab and locataion for player character 1
        Vector3 player1pos = player1Spawn.transform.position;
        Quaternion player1rot = player1Spawn.transform.rotation;
        GameObject player1GO = Instantiate(Resources.Load("cloud_prefab") as GameObject);
        player1GO.transform.position = player1pos;
        player1GO.transform.rotation = player1rot;
        player1Unit = player1GO.GetComponent<Unit>();

        // Instantiating the prefab and locataion for player character 2
        Vector3 player2pos = player2Spawn.transform.position;
        Quaternion player2rot = player2Spawn.transform.rotation;
        GameObject player2GO = Instantiate(Resources.Load("tifa_prefab") as GameObject);
        player2GO.transform.position = player2pos;
        player2GO.transform.rotation = player2rot;
        player2Unit = player2GO.GetComponent<Unit>();

        // Instantiating the prefab and locataion for enemy character 1
        Vector3 enemy1pos = enemy1Spawn.transform.position;
        Quaternion enemy1rot = enemy1Spawn.transform.rotation;
        GameObject enemy1GO = Instantiate(Resources.Load("sephiroth_prefab") as GameObject);
        enemy1GO.transform.position = enemy1pos;
        enemy1GO.transform.rotation = enemy1rot;
        enemy1Unit = enemy1GO.GetComponent<Unit>();



        /**
        Ideally this part will get Unit list from BattleEncounter, separated into enemy and player units.
        Then do a foreach loop for each Unit in the List to be sent to SetHUD method of BattleHUD
        **/
        var unitsFound = FindObjectsOfType<Unit>();
        foreach (var x in unitsFound)
        {
            playerHUD.SetHUD(x);
            enemyHUD.SetHUD(x);
        }

        //playerHUD.SetHUD(player1Unit);
        //enemyHUD.SetHUD(enemyUnit);

    


        //set up Turn system
        Debug.Log(unitsFound.Length +" units found in the battle:");

        IList<Unit> units = new List<Unit>();


        foreach (var x in unitsFound)
        {
            units.Add(x);
        }

        for (int i = 0; i < units.Count; i++)
        {
            Debug.Log(units[i].unitName + ", Speed: " + units[i].speed);
        }

        // create reference to TurnSystem and attach TurnSystem script to the BattleSystem game object
        TurnSystem turns = GameObject.Find("BattleSystem").AddComponent<TurnSystem>();
        // send our units to Turnystem
        turns.setupTurnSystem(units);




        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        //MakeSelection();
        
        bool isDead = enemy1Unit.TakeDamage(player1Unit.damage);

        enemyHUD.SetHP(enemy1Unit.currentHP);
        Debug.Log("Attack successful against " + enemy1Unit);

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
        player1Unit.HealDamage(player1Unit.damage);

        playerHUD.SetHP(player1Unit.currentHP);
        Debug.Log("Heal successful");

        // Hide the player Actions options after selection is made
        playerHUD.DisableActions(playerHUD);

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    IEnumerator EnemyAttack()
    {
        bool isDead = player1Unit.TakeDamage(enemy1Unit.damage);

        playerHUD.SetHP(player1Unit.currentHP);
        Debug.Log("Attack successful against " + player1Unit.unitName + "with " + player1Unit.currentHP + " HP");

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
        enemy1Unit.HealDamage(enemy1Unit.damage);

        enemyHUD.SetHP(enemy1Unit.currentHP);
        Debug.Log("Heal successful");

        // Hide the enemy's Actions options after selection is made
        enemyHUD.DisableActions(enemyHUD);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

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

    /**
    Unit MakeSelection()
    {
        // spawn indicator arrow thingy over the first enemy Unit.
        // set var selectedUnit to enemy1Unit
        // if input is W or Up Arrow, change selection.
        

        if (Input.GetButtonDown("Submit"))
        {
            // return Unit
        }
    }
    **/

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
