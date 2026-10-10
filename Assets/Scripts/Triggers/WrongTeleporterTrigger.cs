using UnityEngine;

public class WrongTeleporterTrigger : MonoBehaviour
{
    Player player;
    public GameObject[] wrong_room_teleportation_points;
    public GameObject starting_point;
    void Start()
    {
        player = Level.current_level.player;
    }

    Vector2 GetNextRandomPosition()
    {
        float randomOdd = Random.Range(0f, 1f);
        int randomIndex = Random.Range(0, wrong_room_teleportation_points.Length);
        Vector2 randomPosition;
        if(randomOdd < 0.3f)
        {
            randomPosition = starting_point.transform.position;
        }
        else
        {
            randomPosition = wrong_room_teleportation_points[randomIndex].transform.position;
        }
        return randomPosition;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {   
        
        if (wrong_room_teleportation_points.Length > 0)
        {
            Vector2 randomPosition = GetNextRandomPosition();
            player.Teleport(randomPosition);
        }
        else
        {
            Debug.LogWarning("No teleportation points assigned in WrongTeleporterTrigger.");
        }

    }
}
