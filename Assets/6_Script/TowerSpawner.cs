using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate; // 타워 정보
    [SerializeField] private InfoTower infoTower; // 타워정보 패널
    [SerializeField] private ToastMessage toastMsg; // 토스트 메시지
    private ContactFilter2D filter; // Raycast용 파라미터
    private List<RaycastHit2D> rcList; // Raycast 결과 저장용 리스트
    private bool isOnTowerButton = false; // 타워건설버튼 눌렸는지 체크
    private GameObject followTowerClone = null; // 임시타워 사용완료시 삭제를 위해 저장하는 변수

    private void Start()
    {
        filter = new ContactFilter2D(); // 파라미터 생성
        rcList = new List<RaycastHit2D>(); // 리스트 생성    
    }

    private void Update()
    {
        // 마우스가 UI 에 있을 때는 바로 리턴
        if (EventSystem.current.IsPointerOverGameObject() == true) return;
        
        // 마우스 왼쪽 버튼 클릭하면
        if (Input.GetMouseButtonDown(0))
        {
            // 리스트를 클리어 하고
            rcList.Clear();
            // 월드포지션값을 구해서
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ray를 쏴서 검출되는 오브젝트들 찾기
            Physics2D.Raycast(worldPosition, Vector2.zero, filter, rcList);
            // 리스트를 돌면서
            foreach (var item in rcList)
            {
                // "TOWER" 태그인 아이템이 있으면
                if (item.transform.CompareTag("TOWER"))
                {
                    // 타워 정보 패널에 표시할 정보를 넘기고 패널 켜기
                    infoTower.OnPanel(item.transform);
                    // 타워가 이미 있으므로 리턴
                    return;
                }
            }
            // 다시한번 리스트를 돌면서
            foreach (var item in rcList)
            { 
                // "TILE" 태그인 아이템이 있으면
                if (item.transform.CompareTag("TILE"))
                {
                    // 그 자리에 타워 생성
                    SpawnTower(item.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0)) // 다른곳 클릭했을 때 패널정보 없애기
        {
            foreach (var item in rcList)
            {
                // 타워가 있는 곳은 빼고
                if (item.transform.CompareTag("TOWER"))
                {
                    return;
                }
            }
            // 아닌 곳에서는 정보 패널 끄기
            infoTower.OffPanel();
        }
    }

    private void SpawnTower(Transform tileTransform)
    {
        // 타워건설버튼을 눌렀을 때만 건설 가능
        if (isOnTowerButton == false) return;
        // 다시 건설 버튼 눌러서 건설하도록 설정
        isOnTowerButton = false;
        // 소지골드에서 건설비용 차감
        PlayerManager.instance.CurrentGold -= towerTemplate.weapon[0].cost;
        // 타워템플릿에 있는 타워프리펩으로 타워 생성
        GameObject clone = Instantiate(towerTemplate.towerPrefab, tileTransform.position, 
            Quaternion.identity, transform);
        // 타워 무기 초기화
        clone.GetComponent<TowerWeapon>().Init();
        // 임시 타워 삭제
        Destroy(followTowerClone);
        // 타워 건설 취소하는 코루틴 중지
        StopCoroutine(OnTowerCancleSystem());
    }

    public void ReadyToSpawnTower()
    {
        // 버튼 중북해서 누르는 경우 방지
        if (isOnTowerButton) return;

        // 건설 소요 비용이 소지 골드보다 크면 리턴
        if (towerTemplate.weapon[0].cost > PlayerManager.instance.CurrentGold)
        {
            // 골드 부족 메시지 출력
            toastMsg.ShowToast(ToastType.Money);
            return;
        }
        // 타워건설 버튼 눌렸다고 설정
        isOnTowerButton = true;
        // 마우스를 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate.followPrefab);
        // 타워 건설 취소하는 코루틴 시작
        StartCoroutine(OnTowerCancleSystem());
    }

    private IEnumerator OnTowerCancleSystem()
    {
        while (true)
        {
            // esc 또는 마우스 우클릭하면 타워건설 취소
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }
    }
}
