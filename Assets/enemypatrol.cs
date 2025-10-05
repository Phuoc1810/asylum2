using UnityEngine;

public class enemypatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public int targetPoint;
    public float speed;
    public float scale = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == patrolPoints[targetPoint].position)
        {
            increaseTargetint();
        }
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint].position,speed *Time.deltaTime);
    }
    void increaseTargetint()
    {
        scale *= -1;
        transform.localScale =new Vector3(0.2f,0.2f,scale);
        targetPoint++;
        if (targetPoint >= patrolPoints.Length) { targetPoint = 0; }
    }
}
