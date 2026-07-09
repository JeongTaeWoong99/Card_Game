using System.Collections;
using UnityEngine;

// [서비스 로케이터 교체 데모] 서버 연동 시의 '원격 사람 플레이어'를 흉내 내는 상대 구현.
// 실제로는 SetupPlace/Play에서 패킷을 주고받겠지만, 여기서는 디버그 로그로 시나리오만 재현한다.
// IEnemyAI 계약만 지키므로 TurnManager는 EnemyAI에서 이걸로 바뀐 걸 전혀 모른다 (DIP).
public class TestPlayer : MonoService<IEnemyAI>, IEnemyAI
{
    private readonly WaitForSeconds _actDelay = new WaitForSeconds(1.5f); // 서버 응답 대기 흉내

    // 원격 플레이어의 배치가 패킷으로 도착했다고 가정 — 게임 진행을 위해 앞줄 3 + 뒷줄 3을 채운다
    // (TurnManager.StartGameCo가 Services.Get<IEnemyAI>()로 호출)
    public void SetupPlace()
    {
        Debug.Log("[TestPlayer] 원격 배치 수신 — 앞줄 3 + 뒷줄 3 배치");

        for (int i = 0; i < 3; i++) Services.Get<CardManager>().TryPutCard(false, true);  // 앞줄
        for (int i = 0; i < 3; i++) Services.Get<CardManager>().TryPutCard(false, false); // 뒷줄

        Services.Get<CardManager>().DrawSkillCards(false, TurnManager.SetupSkillDraw);
    }

    // 상대 턴 시나리오를 패킷 흐름처럼 로그로 재현한 뒤, 턴 종료 패킷을 보낸 것처럼 EndTurn
    // (TurnManager.StartTurnCo가 상대 턴에 Services.Get<IEnemyAI>()로 호출)
    public void Play()
    {
        StartCoroutine(PlayCo());
    }

    // 서버 왕복을 흉내 내 텀을 두며 로그를 순차 출력하고 턴을 넘긴다
    private IEnumerator PlayCo()
    {
        Debug.Log("[TestPlayer] 상대 턴");

        yield return _actDelay;

        Debug.Log("[TestPlayer] 상대가 자기 턴 동안 작업을 수행합니다.");

        yield return _actDelay;

        Debug.Log("[TestPlayer] 상대 턴 종료");

        Services.Get<TurnManager>().EndTurn();
    }
}
