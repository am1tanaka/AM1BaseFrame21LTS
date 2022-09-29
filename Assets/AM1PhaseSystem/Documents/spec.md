# フェーズシステム仕様

## 基本情報
- PhaseManagerスクリプトをシーン内のオブジェクトにアタッチします
- フェーズは`PhaseBase`クラスを継承して実装します
- PhaseManager、および、PhaseBaseは各ゲームオブジェクトで独立して利用できるようにstaticは採用しません。利用するゲームオブジェクトのスクリプト上で必要なインスタンスをnewで生成して利用します
- フェーズを切り替える時は、PhaseManagerのインスタンスの???Request()メソッドに、フェーズ処理クラスのインスタンスを渡して要求します
- フェーズの切り替えは以下の4種類があります
  - 現在のフェーズを入れ替えるChangeRequest()
  - 現在のフェーズを待機させて新しいフェーズを開始するPushRequest()
  - 現在のフェーズを破棄して前のフェーズに戻すPopRequest()
  - ルートのフェーズまで戻すPopAllRequest()
- デフォルトのRequestは、実行フェーズが切り替えを許可していない(CanChangeがfalse)の時は、要求を失敗させて受け付けません
- 切り替え禁止の解除後に切り替えを予約する場合は、Request()メソッドのreserveフラグにtrueを設定して呼び出します


## PhaseManagerの処理の流れ

### phaseStackが空(CurrentPhaseInfo==null)のとき
- 何もしない
- 変更要求
  - 無条件で登録可能

### phaseStackが実行中の時(CurrentPhaseInfo != null)
- CurrentPhaseInfo.IsTerminatedがfalseなら、Update()とFixedUpdate()を実行
- 変更要求
  - ???Request()を各箇所から呼び出してrequestQueueに積む
- Update()の先頭でUpdateChangeRequest()を呼び出して切り替えチェックや処理
- UpdateChangeRequest()
  - CurrentStateで状態分け
    - Stadby フェーズがない状態
      - requestQueueの最初のフェーズのInitを呼び出して、状態をRunningにして切り替え完了
    - Running
      - 現在のフェーズのCanChangeがfalse
        - 要求を蓄積したまま次の更新に持ち越し
      - 現在のフェーズのCanChangeがtrue
        - toNextActionを呼び出してPauseかTerminateを実行
        - IsTerminatedがtrue
          - フェーズの切り替えを実行してInitを呼び出す
        - IsTerminatedがfalse
          - CurrentStateをWaitTerminateに設定
    - WaitTerminate
      - IsTerminatedがtrueになったら、フェーズの切り替えを実行。CurrentStateはRunningかPopAllへ
    - PopAll
      - IsTerminatedがtrueになったらPopAll()を呼ぶ
      - PopAll()内で必要に応じて現在の状態をRunningへ戻すか、PopAllに設定

### PopAllの流れ
- PopAllRequest()を呼ぶ
  - フェーズスタックが1つ以下の時はこれ以上戻せないので何もせずに成功で完了
  - フェーズスタックが2つ以上の時、PopAll()と現在処理の終了を登録
- UpdateChangeRequest
  - RunningでtoNextAction(Terminate)を実行
  - IsTerminatedがtrue
    - PopAll()を呼び出す
  - IsTerminatedがfalse
    - WaitTerminateで終了を待った後、PopAll()を呼び出す
    - CurrentStateはPopAllへ変更
- PopAll()
  - 現在のPhaseInfoをプールに戻してphaseStackをPop
  - phaseStackの残りが1つ
    - 切り替えたフェーズのResume()を呼び出して状態をRunningへ
  - phaseStackの残りが2つ以上
    - PopAll状態にして、現在のフェーズのTerminateを呼ぶ

## PhaseManager
- State Standby, Running, WaitTerminate
- ChangeRequest(IPhase ph, bool reserve=false)
  - 現在の階層のフェーズを切り替えるようPhaseManagerへ要求します
- PushRequest(IPhase ph, bool reserve=false)
  - 現在のフェーズに戻れるようにスタックに積んで切り替え要求します
- PopRequest(bool reserve=false)
  - 一つ前のフェーズに戻す要求をします
- PopAllRequest(bool reserve=false)
  - ルートのフェーズまで戻すよう要求します

## IPhase
- enum State.None, Init, Update, Terminate
- State CurrentState
- bool CanChange
  - 他のフェーズに切り替え可能な状態ならtrue
- bool IsTerminated
  - 終了処理が完了していたらtrue
- void Init()
  - フェーズ開始時の初期化処理
- void Update()
  - フェーズシステムのUpdate()から呼び出す処理
- void FixedUpdate()
  - フェーズシステムのFixedUpdateから呼び出す処理
- void Terminate()
  - フェーズの終了開始を要求する処理
