using UnityEngine;

public class RandomScreamBehaviour : StateMachineBehaviour
{
    [SerializeField] private float minInterval = 8f;
    [SerializeField] private float maxInterval = 20f;

    private float timer;
    private float nextScreamTime;
    private static readonly int ScreamHash = Animator.StringToHash("ScreamTrigger");

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0f;
        nextScreamTime = Random.Range(minInterval, maxInterval);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer >= nextScreamTime)
        {
            animator.SetTrigger(ScreamHash);
            timer = 0f;
            nextScreamTime = Random.Range(minInterval, maxInterval);
        }
    }
}