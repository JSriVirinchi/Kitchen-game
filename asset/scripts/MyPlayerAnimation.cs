using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerAnimation : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsWalkingAnimation", player.IsWalking());
    }
}
