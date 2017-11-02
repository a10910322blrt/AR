using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RhinoController1 : MonoBehaviour
{//常數宣告
    private string roarTrigger = "roar";
    private string crystalTag = "crystal";
    private string deadTrigger = "dead";
    // 衝撞攻擊行為參數
    public float impactChargeTime = 1f;
    public Transform impactTarget;
    public float impactSpeed = 10f;
    public int impactDamage = 50;
    public ParticleSystem explosionEffect;
    public float walkSpeed = 3f;
    public Slider aimSlider;
    //音效宣告
    public AudioClip roarSound;
    public AudioClip deadSound;
    public AudioClip impactsound;
    //依賴組件宣告
    private NavMeshAgent navMeshAgent;
    private AudioSource audioSource;
    private Animator animator;

    private bool isImpacting;

    public Transform target;
    public int hitOffset = 1;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //  StartCoroutine("ProcessImpact");
        GameObject go = GameObject.FindGameObjectWithTag(crystalTag);
        if (go != null)
        {
            target = go.transform;
            StartCoroutine("ProcessState");
        }
    }
    private IEnumerator ProcessState()
    {
        while (target != null)
        {
            //隨機產生以水晶為圓心 半徑為衝撞距離的一點 讓犀牛往前
            navMeshAgent.speed = walkSpeed;
            float randomRad = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Vector3.Distance(impactTarget.position, transform.position);
            Vector3 randomPos = target.position + new Vector3(Mathf.Cos(randomRad), 0, Mathf.Sin(randomRad)) * distance;
            navMeshAgent.SetDestination(randomPos);
            yield return null;
            //等待犀牛走到目的地
            while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }
            if (target == null)
                yield break;
            //開始攻擊
            yield return StartCoroutine("ProcessImpact");
            //攻擊結束
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator ProcessImpact()
    {
        transform.LookAt(target);
        aimSlider.gameObject.SetActive(true);
        aimSlider.value = aimSlider.minValue;
        aimSlider.maxValue = impactChargeTime;
        animator.SetTrigger(roarTrigger);
        while (aimSlider.value < aimSlider.maxValue)
        {
            aimSlider.value += Time.deltaTime;
            yield return null;
        }
        aimSlider.gameObject.SetActive(false);
        navMeshAgent.speed = impactSpeed;
        RaycastHit hit;
        float distance = Vector3.Distance(impactTarget.position, transform.position);
        Vector3 targetPos = impactTarget.position;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            if (hit.collider.CompareTag(crystalTag))
            {
                targetPos = hit.point;
            }

        }
        navMeshAgent.SetDestination(targetPos - transform.forward * hitOffset);
        yield return null;
        //等待犀牛走到目的地
        while (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }
        isImpacting = false;

    }
    private void Update()
    {
        navMeshAgent.SetDestination(target.position);

    }
    public void TriggerRoarSound()
    {
        audioSource.PlayOneShot(roarSound);
    }
}
