using UnityEngine;

// 방패 — 피해 경감 + 도발(적 근접이 이 카드만 노림). 경감량은 아래 인스펙터 필드에서 편집한다.
[CreateAssetMenu(fileName = "ShieldBehaviour", menuName = "Scriptable Object/Card Behaviour/Shield")]
public class ShieldBehaviour : CardBehaviour
{
    [CenterHeader("< 방패 튜닝 >")]
    [SerializeField] private int _damageReduction = 2; // 받는 피해에서 차감(최소 1 보장)

    // 받는 모든 피해 -_damageReduction (최소 1 보장)
    public override int ModifyIncomingDamage(int rawDamage) => Mathf.Max(1, rawDamage - _damageReduction);

    // 적 근접 공격이 이 카드만 노리게 한다 (도발)
    public override bool IsTaunter => true;
}
