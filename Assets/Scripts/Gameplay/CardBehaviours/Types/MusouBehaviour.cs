using System.Collections;
using UnityEngine;
using DG.Tweening;

// 무쌍 — 광역 공격, 후방에서 충전. 데이터(대기 2턴 등)는 베이스 직렬화 필드, 튜닝 수치는 아래 인스펙터 필드에서 편집한다.
[CreateAssetMenu(fileName = "MusouBehaviour", menuName = "Scriptable Object/Card Behaviour/Musou")]
public class MusouBehaviour : CardBehaviour
{
    [CenterHeader("< 무쌍 튜닝 >")]
    [SerializeField] private float _chargeChance = 0.5f; // 후방 충전 발동 확률
    [SerializeField] private int   _chargeAmount = 1;    // 충전 시 HP 증가량(영구)
    [SerializeField] private float _splashRatio  = 0.5f; // 인접 추가 피해 비율(본체 피해의 절반 = 원래 HP의 25%)

    // 무쌍 공격 — 이동 후 대상에 현재 HP 50%, 인접 적 1체(랜덤)에 25% 피해 + 대상의 반격(현재 HP 절반)을 받음
    public override void Attack(Entity attacker, Entity defender)
    {
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        ICombatSystem cs = Services.Get<ICombatSystem>();
        Entity splashTarget = Services.Get<IBoardState>().GetRandomAdjacentFront(defender);

        DOTween.Sequence()
            .Append(attacker.transform.DOMove(defender.originPos, CombatSystem.MoveTime)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {
                int defenderHp = defender.health; // 반격용 — 공격 전 HP 캡처
                int mainDamage = defender.ApplyDefense(CombatSystem.CalcDamage(attacker.health, CombatSystem.DamageRatio));

                defender.Damaged(mainDamage);
                cs.ShowDamagePopup(mainDamage, defender.transform);

                if (splashTarget != null)
                {
                    int splashDamage = splashTarget.ApplyDefense(CombatSystem.CalcDamage(attacker.health, CombatSystem.DamageRatio * _splashRatio));
                    splashTarget.Damaged(splashDamage);
                    cs.ShowDamagePopup(splashDamage, splashTarget.transform);
                }

                int toAttacker = attacker.ApplyDefense(CombatSystem.CalcDamage(defenderHp, CombatSystem.CounterRatio));
                attacker.Damaged(toAttacker);
                cs.ShowDamagePopup(toAttacker, attacker.transform);
            })
            .Append(attacker.transform.DOMove(attacker.originPos, CombatSystem.MoveTime)).SetEase(Ease.OutSine)
            .OnComplete(() => cs.FinishAttack(attacker, defender, splashTarget));
    }

    // 후방에 있을 때만, 내 턴 시작 시 확률로 자신 HP 영구 버프(승격 시 강해짐)
    public override IEnumerator OnTurnStartPassive(TurnPassiveContext ctx)
    {
        if (ctx.IsFront || Random.value >= _chargeChance)
        {
            yield break;
        }

        ctx.Self.BuffHp(_chargeAmount, 0);

        yield return ctx.Delay;
    }
}
