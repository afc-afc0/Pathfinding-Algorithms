using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathManager : MonoBehaviour
{
    #region SINGLETON
    private static PathManager instance;

    public static PathManager Instance
    {
        get => instance;
    }
    #endregion

    Queue<PathResult> results;
    PathRequest currentRequest;

    [SerializeField] private bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        results = new Queue<PathResult>();
    }

    int queueCount;
    private void Update()
    {
        if(results.Count > 0)
        {
            queueCount = results.Count;
            lock(results)
            {
                for(int i = 0;i < results.Count;i++)
                {
                    PathResult result = results.Dequeue();
                    result.callBack(result.path , result.isSuccesfull);
                }
            }
        }
    }

    public void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            Pathfinding.Instance.FindPath2(request, FinishedProcessingPath);
        };
        threadStart.Invoke();
    }


    public void FinishedProcessingPath(PathResult result)
    {
        PathResult res = new PathResult(result.path , result.isSuccesfull, result.callBack);
        lock(results)
            results.Enqueue(res);
    }

}

public struct PathResult
{
    public Vector3[] path;
    public bool isSuccesfull;
    public Action<Vector3[], bool> callBack;

    public PathResult(Vector3[] path, bool isSuccesfull, Action<Vector3[], bool> callBack)
    {
        this.path = path;
        this.isSuccesfull = isSuccesfull;
        this.callBack = callBack;
    }

}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        pathStart = start;
        pathEnd = end;
        this.callback = callback;
    }
}