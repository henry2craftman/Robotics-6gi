# Unity 학습 및 데모 프로젝트 Readme

## 📖 개요

이 프로젝트는 Unity 엔진의 다양한 핵심 기능과 고급 주제를 학습하고 시연하기 위해 제작된 데모 및 스터디 프로젝트 모음입니다. 물리 시뮬레이션, 애니메이션, UI, 비동기 프로그래밍, 그리고 Firebase 연동에 이르기까지 Unity 개발에 필요한 광범위한 주제들을 각 씬(Scene)별로 구현하고 있습니다.

각 씬은 특정 개념이나 기능을 집중적으로 다루며, 해당 씬의 로직은 `Scripts` 폴더 내의 관련 스크립트 파일에 구현되어 있습니다. 이 프로젝트는 Unity 개발자들이 특정 기능을 이해하고 자신의 프로젝트에 적용하는 데 참고 자료로 활용될 수 있습니다.

## ✨ 주요 학습 및 데모 주제

이 프로젝트는 다음을 포함한 다양한 Unity 개발 주제를 다룹니다:

*   **기본 조작 및 물리**:
    *   `1. FloorPlan_MovementStudy.unity`: 씬 내에서의 오브젝트 이동 및 조작 연구.
    *   `2. RotationStudy.unity`: 오브젝트 회전 및 방향 제어 연구.
    *   `4. PhysicsStudy.unity`: Unity 물리 엔진의 기본 동작 및 상호작용 연구.
    *   `5. Pinball.unity`: 핀볼 게임 구현을 통한 복합적인 물리 시뮬레이션 데모. (`PinballManager.cs`, `Ball.cs`, `GameoverLine.cs` 등)
    *   `7. Conveyor.unity`: 컨베이어 벨트와 같은 이동 메커니즘 구현 및 물리적 상호작용. (`7. Conveyor` 스크립트 폴더)
    *   `8. Raycast.unity`: 레이캐스트를 활용한 오브젝트 감지 및 상호작용. (`8. Raycast` 스크립트 폴더)
*   **애니메이션**:
    *   `9. Animation.unity`: Unity 애니메이션 시스템의 기본 사용법 및 컨트롤러. (`9. Animation` 스크립트 폴더)
    *   `11. AnimStudy.unity`: 캐릭터 애니메이션 및 IK 툴킷 활용 연구.
*   **UI 및 이펙트**:
    *   `6. UIStudy.unity`: Unity UI 시스템을 활용한 사용자 인터페이스 구현. (`UIManager.cs`)
    *   `10. Particle.unity`: 파티클 시스템을 활용한 시각 효과 구현.
*   **고급 프로그래밍**:
    *   `13. AsyncProgramming.unity`: 비동기 프로그래밍 패턴 (async/await) 연구. (`13. AsyncProgramming` 스크립트 폴더)
    *   `14. Serialization.unity`: JSON 직렬화를 포함한 데이터 저장 및 로드 기법. (`JsonSerializationManager.cs`)
    *   `17.ClientAsync.unity`: 클라이언트 측 비동기 통신 및 처리. (`17. ClientAsync` 스크립트 폴더)
*   **Firebase 연동**:
    *   `15. Firebase Realtime Database.unity`: Firebase Realtime Database를 활용한 데이터 저장 및 동기화. (`15. Firebase Realtime Database` 스크립트 폴더)
    *   `16. Firebase Authentication.unity`: Firebase Authentication을 활용한 사용자 인증 시스템. (`16. Firebase Authentication` 스크립트 폴더)
    *   `12-2. MPS with Firebase.unity`: Firebase와 연동된 다목적 시스템(MPS) 데모. (`12-2. MPS with Firebase` 스크립트 폴더)
*   **기타 시스템**:
    *   `12. MPS.unity`: 다목적 시스템(MPS)의 기본 구현. (`12. MPS` 스크립트 폴더)
    *   `12-1. MPS with Robot 1.unity`: 로봇과 연동된 MPS 데모.
    *   `18. RobotTeacher.unity`: 로봇을 활용한 교육 또는 시뮬레이션 데모.

## 🛠️ 사용된 기술 및 에셋

### 핵심 기술
*   **게임 엔진**: Unity 202x.x.x
*   **백엔드 서비스**: Google Firebase (Realtime Database, Authentication)
*   **입력 시스템**: Unity Input System
*   **애니메이션**: Unity Animation, IK Toolkit
*   **데이터 처리**: JSON 직렬화

### 주요 에셋
*   **캐릭터**: unity-chan!, AnimeGirls, NekoLegends, GhostCharacter_Free
*   **환경**: SimplePoly City - Low Poly Assets
*   **가구 및 소품**: FurnishedCabin, MinimalistBedroom, NextGen Furniture Pack, RawWoodenFurnitureFree
*   **무기/사운드**: IronSpear Content, Weapons of Choice FREE - Komposite Sound
*   **기타**: Phoenix3D, Floor materials pack

## ⚙️ 설치 및 실행 방법

1.  **프로젝트 클론**: 이 Git 저장소를 로컬 컴퓨터에 클론합니다.
2.  **Unity Hub에서 열기**: Unity Hub를 열고 'Add' 버튼을 클릭하여 클론한 프로젝트 폴더를 선택합니다.
    *   프로젝트에 맞는 Unity 에디터 버전이 설치되어 있어야 합니다. (권장 버전: `202x.x.x` - 프로젝트에 맞는 버전으로 수정해주세요)
3.  **Firebase 설정 (Firebase 관련 씬을 실행할 경우)**:
    *   Firebase 콘솔에서 새 프로젝트를 생성하거나 기존 프로젝트에 연결합니다.
    *   Android/iOS 앱을 추가하고 `google-services.json` 및 `GoogleService-Info.plist` 파일을 다운로드합니다.
    *   다운로드한 설정 파일을 `Assets` 폴더 내에配置합니다. (현재 `google-services.json` 파일이 존재합니다.)
    *   **중요**: Firebase 관련 SDK 및 External Dependency Manager가 패키지를 올바르게 가져왔는지 확인합니다.
4.  **프로젝트 실행**:
    *   Unity 에디터에서 `Assets/Scenes` 폴더에 있는 원하는 씬 파일을 엽니다.
    *   Play 버튼을 눌러 해당 씬의 데모를 실행합니다.

## 🕹️ 조작 방법

본 프로젝트는 Unity의 신규 입력 시스템을 사용합니다. 조작키는 `Assets/InputSystem_Actions.inputactions` 파일에서 확인 및 수정할 수 있습니다. 각 씬에 따라 필요한 조작이 다를 수 있습니다.

*   **일반적인 이동**: `W`, `A`, `S`, `D`
*   **점프**: `Space`
*   **카메라 회전**: `마우스` 이동
*   **상호작용**: `E` 또는 `F` (각 씬의 구현에 따라 다름)

## 📝 크레딧

본 프로젝트는 아래와 같은 다양한 에셋 스토어의 자료를 활용하여 제작되었습니다. 훌륭한 에셋을 제공해주신 모든 제작자분들께 감사드립니다.

*   unity-chan!: © Unity Technologies Japan/UCL
*   SimplePoly City - Low Poly Assets
*   FurnishedCabin
*   MinimalistBedroom
*   NextGen Furniture Pack
*   RawWoodenFurnitureFree
*   IronSpear Content
*   Weapons of Choice FREE - Komposite Sound
*   GhostCharacter_Free
*   ... (기타 `DownloadedAssets` 폴더 내 에셋)