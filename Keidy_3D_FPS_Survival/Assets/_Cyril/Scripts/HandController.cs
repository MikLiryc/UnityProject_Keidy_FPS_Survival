using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 핸드형 타입 무기
    [SerializeField]
    private Hand currentHand;

    //공격중인지
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                //코루틴 실행
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;

        currentHand.anim.SetTrigger("Attack");

        //공격하는 모션 딜레이동안 대기
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        //공격 활성화 시점


        //팔을 휘두르는 모션 시간 동안 대기
        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        //전체 공격모션 시간 - 지금까지 대기한 시간 동안 대기
        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);

        isAttack = false;
    }

    IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                //충돌됨
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
