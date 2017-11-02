using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class HeroController1 : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float attackRadius;
    public RectTransform attackRangeRect;
    public LayerMask enemyLayerMask;
    public Color dectColor;
    public Color normalcolor;
    public Image attackProbecircle;
    public GameObject target;
    public float turnSmooth = 15f;

    public Animator animator;
    public string attackBool = "attack";

    public AudioSource audioSource;
    public AudioClip firesound;

    public ParticleSystem gunShotEffect;

    private void Awake()
    {
        attackRadius = attackRangeRect.rect.width / 2;
    }
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        if (navMeshAgent.remainingDistance == 0)
        {
            Vector3 lookPos = target.transform.position - transform.position;
            lookPos.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSmooth * Time.deltaTime);

            animator.SetBool(attackBool, true);


        }
        else
        {
            animator.SetBool(attackBool, false);
        }
    }

    public void Move(Vector3 target)
    {
        navMeshAgent.SetDestination(target);
        navMeshAgent.isStopped = false;
    }

    public void FixedUpdate()

    {
        Collider[] allcollider = Physics.OverlapSphere(transform.position, attackRadius, enemyLayerMask);
        // Physics.OverlapSphere(transform.position, attackRadius, enemyLayerMask);
        if (allcollider.Length > 0)
        {
            attackProbecircle.color = dectColor;
            target = allcollider[0].gameObject;
        }
        else
        {
            attackProbecircle.color = normalcolor;
            target = null;
            animator.SetBool(attackBool, false);
        }
    }

    public void OnGunTrigger()
    {
        audioSource.PlayOneShot(firesound);
        gunShotEffect.Play();
    }
}
