using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 5f;
    Vector3[] path;
    int targetIndex;

    Stopwatch stopwatch = new Stopwatch();
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            stopwatch.Reset();
            stopwatch.Start();
            PathManager.Instance.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        }
    }

    public void OnPathFound(Vector3[] newPath , bool pathSuccesfull)
    {
        if(pathSuccesfull)
        {
            path = newPath;
            stopwatch.Stop();
            float ms = stopwatch.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Calculated the path in ms = " + ms);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        targetIndex = 0;
        Vector3 currentTarget = path[0];

        while(true)
        {
            if (transform.position == currentTarget)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                    yield break;
                currentTarget = path[targetIndex];
            }
            //Debug.Log("transform position.x = " + transform.position.x + " transform position.z = " + transform.position.z + " target x = " + currentTarget.x + " z = " + currentTarget.z);
            transform.position = Vector3.MoveTowards(transform.position , currentTarget , speed * Time.deltaTime);
            yield return null;
        }
    }

}
