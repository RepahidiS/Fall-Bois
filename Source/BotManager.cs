using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int botCount = 10;
    public List<GameObject> bots;
    public CheckpointManager checkpointManager;
    public WaypointManager waypointManager;
    public PlayerMyself mainCharacter;

    void Start()
    {
        for(int i = 0; i < botCount; i++)
        {
            GameObject bot = Instantiate(botPrefab, transform);
            bot.name = "Bot " + (i + 1);

            Bot currentBot = bot.GetComponent<Bot>();
            currentBot.waypointManager = waypointManager;
            currentBot.checkpointManager = checkpointManager;
            currentBot.rb = bot.GetComponent<Rigidbody>();
            currentBot.animator = bot.GetComponent<Animator>();

            bots.Add(bot);
        }

        foreach(GameObject bot in bots)
        {
            foreach(GameObject b in bots)
            {
                if (bot != b)
                    Physics.IgnoreCollision(bot.GetComponent<CapsuleCollider>(), b.GetComponent<CapsuleCollider>());
            }

            Physics.IgnoreCollision(bot.GetComponent<CapsuleCollider>(), mainCharacter.GetComponent<CapsuleCollider>());
        }

        ResetBots();
    }

    public void UpdateCanMove(bool canMove)
    {
        foreach(GameObject bot in bots)
            bot.GetComponent<Bot>().UpdateCanMove(canMove);
    }

    public void ResetBots()
    {
        int x = -7;
        foreach(GameObject bot in bots)
        {
            bot.transform.position = new Vector3(x, 2.1f, 18);
            bot.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            Bot currentBot = bot.GetComponent<Bot>();
            currentBot.currentCheckpoint = checkpointManager.goSpawnPoint;
            currentBot.UpdateCanMove(false);
            currentBot.characterAnimState = AnimState.Idle;
            currentBot.animator.SetInteger("AnimState", (int)AnimState.Idle);
            currentBot.rb.velocity = Vector3.zero;
            currentBot.GetFirstWaypoint();
            currentBot.isFinished = false;
            x++;
        }
    }
}