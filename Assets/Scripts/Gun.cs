using UnityEngine;
  
  public class Gun : MonoBehaviour   
  {
      [Header("Required Components")]
      public Transform bulletPos;    
      public GameObject fireEffect;  
      public GameObject hitEffect;
  
      [Header("Gun Settings")]
      public int maxBulletNum = 10;
      public float fireEffectDuration = 0.3f;
  
      [Header("Audio Clips")]
      public AudioClip fireClip;
      public AudioClip reloadClip;
      public AudioClip hitClip;
  
      // --- Private Fields ---
      private int bulletCnt = 0;
      private AudioSource audio;
  
      // --- Network Fields ---
      private PlayerController _playerController;
      private ClientAsync _clientAsync;
  
      // --- Hit Effect Fields ---
      public bool isHit = false;
      public Vector3 hitPos = Vector3.zero;
  
      void Start()
      {
          audio = GetComponent<AudioSource>();
  
          // 네트워크 동기화를 위해 상위 객체에서 컴포넌트 찾기
          _playerController = GetComponentInParent<PlayerController>();
          if (_playerController == null)
          {
              Debug.LogError("Gun.cs: PlayerController를 찾을 수 없습니다!");
          }
      }
  
      // ClientAsync 스크립트가 호출하여 자신의 참조를 전달하는 메서드
      public void SetClient(ClientAsync client)
      {
          _clientAsync = client;
      }
  
      void Update()
      {
          // 이 총이 '내' 플레이어의 것이 아니면 입력 처리 안함
          if (_playerController == null || !_playerController.isLocalPlayer)
          {
              return;
          }
  
          // 발사
          if (Input.GetMouseButtonDown(0))
          {
              Fire();
          }
          
          // 재장전
          if (Input.GetKeyDown(KeyCode.LeftShift))
          {
              print("재장전!");
              audio.clip = reloadClip;
              audio.Play();
              bulletCnt = 0;
          }
  
          HitCheck();
      }
  
      private void Fire()
      {
          if (bulletCnt >= maxBulletNum)
          {
              print("총알을 모두 소진했습니다. 재장전 하세요.");
              return;
          }

          // 1. 이펙트 및 사운드 재생
          FireEffectON();
          audio.clip = fireClip;
          audio.Play();
          Invoke(nameof(FireEffectOFF), fireEffectDuration);

          bulletCnt++;
  
          // 2. 네트워크로 발사 정보 전송
          if (_clientAsync != null)
          {
              // 총알의 위치와 회전값을 계산합니다.
              Vector3 bulletSpawnPos = bulletPos.position;
              Quaternion bulletSpawnRot = Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, transform.eulerAngles.z);
              
              _clientAsync.SendFireMessage(bulletSpawnPos, bulletSpawnRot.eulerAngles);
          }
          else
          {
              Debug.LogWarning("Gun.cs: ClientAsync 참조가 설정되지 않아 네트워크 전송을 할 수 없습니다.");
          }
      }
  
      private void HitCheck()
      {
          if (isHit)
          {
              hitEffect.transform.position = hitPos;
              hitEffect.gameObject.SetActive(true);
              Invoke(nameof(HitEffectOff), fireEffectDuration);
          }
      }
  
      private void HitEffectOff()
      {
          isHit = false;
          hitEffect.gameObject.SetActive(false);
      }
  
      private void FireEffectON()
      {
          fireEffect.SetActive(true);
          fireEffect.transform.position = bulletPos.position;
          fireEffect.transform.rotation = transform.rotation;
      }
  
      private void FireEffectOFF()
      {
          fireEffect.SetActive(false);
      }
  }