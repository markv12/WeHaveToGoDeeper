using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
    public Transform mainT;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<Player>() != null) {
            SessionData.lastCheckpoint = mainT.position;
        }
    }
}
