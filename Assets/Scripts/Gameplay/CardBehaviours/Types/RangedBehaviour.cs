using System.Collections;
using UnityEngine;

// 원거리 — 반격 없음·도발 무시, 후방에서 견제 피해. 데이터(대기 1턴 등)는 베이스 직렬화 필드에서 편집한다.
[CreateAssetMenu(fileName = "RangedBehaviour", menuName = "Scriptable Object/Card Behaviour/Ranged")]
public class RangedBehaviour : CardBehaviour
{
    // 도발(방패)을 무시하고 모든 적을 공격할 수 있다
    public override bool IgnoresTaunt => true;

    // 반격을 받지 않는다 — 화살로 일방 피해만 입힌다
    public override bool ReceivesCounter => false;

    // 원거리 공격 — 제자리에서 화살을 발사하고, 화살이 도착했을 때만 대상에 현재 HP 절반 피해(반격 X)
    public override void Attack(Entity attacker, Entity defender)
    {
        CombatSystem cs = Services.Get<CombatSystem>();
        int rawDamage = CombatSystem.CalcDamage(attacker.health, CombatSystem.DamageRatio);

        cs.FireArrow(attacker.transform.position, defender.transform.position, () =>
        {
            int damage = defender.ApplyDefense(rawDamage);
            defender.Damaged(damage);
            cs.ShowDamagePopup(damage, defender.transform);
            cs.FinishAttack(attacker, defender);
        });
    }

    // 후방에 있을 때만, 내 턴 시작 시 적 전방 무작위 1체에 공격력의 1/3(버림, 최소 1) 화살 피해
    public override IEnumerator OnTurnStartPassive(TurnPassiveContext ctx)
    {
        if (ctx.IsFront)
        {
            yield break;
        }

        Entity target = Services.Get<EntityManager>().GetRandomEnemyFront(ctx.IsMine);
        if (target == null)
        {
            yield break;
        }

        int damage = Mathf.Max(1, ctx.Self.health / 3);
        Services.Get<CombatSystem>().FirePokeArrow(ctx.Self.transform.position, target, damage); // 후방 카드 자기 위치에서 발사

        yield return ctx.Delay;
    }
}
