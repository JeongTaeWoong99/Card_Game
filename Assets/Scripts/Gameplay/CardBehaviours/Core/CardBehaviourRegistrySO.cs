using System;
using System.Collections.Generic;
using UnityEngine;

// 카드 타입별 behaviour SO를 한곳에 모아 ECardType → 행동 매핑을 만든다.
// 부트스트랩(CardManager.Awake)이 이 SO를 받아 CardBehaviours 파사드를 초기화한다.
[CreateAssetMenu(fileName = "CardBehaviourRegistry", menuName = "Scriptable Object/Card Behaviour Registry")]
public class CardBehaviourRegistrySO : ScriptableObject
{
    [CenterHeader("< behaviour 목록 (타입별 1개) >")]
    [SerializeField] private CardBehaviour[] _behaviours;

    // 등록된 behaviour들을 각자의 Type을 키로 하는 매핑으로 만든다 (CardBehaviours.Init이 호출)
    public Dictionary<ECardType, ICardBehaviour> BuildMap()
    {
        var map = new Dictionary<ECardType, ICardBehaviour>();

        foreach (CardBehaviour behaviour in _behaviours)
        {
            if (behaviour == null) // 인스펙터 할당 누락 방어
            {
                continue;
            }

            if (map.ContainsKey(behaviour.Type)) // 같은 타입을 두 번 넣은 세팅 실수를 알린다
            {
                Debug.LogError($"[CardBehaviourRegistry] {behaviour.Type} 타입 behaviour가 중복 등록됨: {behaviour.name}", this);
            }

            map[behaviour.Type] = behaviour;
        }

        // 모든 카드 타입이 빠짐없이 채워졌는지 검사한다 (누락 시 해당 타입 카드가 런타임에 예외로 죽는다)
        int typeCount = Enum.GetValues(typeof(ECardType)).Length;
        if (map.Count != typeCount)
        {
            Debug.LogError($"[CardBehaviourRegistry] behaviour 등록 수({map.Count})가 카드 타입 수({typeCount})와 다릅니다. 누락/중복을 확인하세요.", this);
        }

        return map;
    }
}
