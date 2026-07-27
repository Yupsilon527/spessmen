using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineQueue : MonoBehaviour
{
    public class CoroutineTask
    {
        public IEnumerator Coroutine { get; set; }
        public object Sender { get; set; } // Optional: sender reference if needed

        public CoroutineTask(IEnumerator coroutine, object sender = null)
        {
            Coroutine = coroutine;
            Sender = sender;
        }
    }
    private Queue<CoroutineTask> taskQueue = new Queue<CoroutineTask>();
    public UnityEvent onQueueFinished = new();
    private Coroutine currentCoroutine;

    public void EnqueueCoroutine(IEnumerator coroutine, object sender = null)
    {
        var task = new CoroutineTask(coroutine, sender);
        taskQueue.Enqueue(task);

        // Start the processing of the queue if not already running
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        while (taskQueue.Count > 0)
        {
            var task = taskQueue.Dequeue();
            yield return StartCoroutine(task.Coroutine); // Execute the coroutine task
        }
        onQueueFinished.Invoke();
        // Reset the current coroutine reference when done
        currentCoroutine = null;
    }
}
