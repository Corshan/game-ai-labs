using UnityEngine;

public class Move : MonoBehaviour {

    public Vector3 dir;
    public float speed = 10;
    public GameObject goal;

    void Start() {

        dir = goal.transform.position  - transform.position;
        // this.transform.Translate(goal.transform.position.normalized);
    }

    private void Update() {
        Vector3 vel = dir.normalized * speed * Time.deltaTime;
        transform.position = transform.position + vel;
    }
}
