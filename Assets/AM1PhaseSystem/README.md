# 汎用フェーズ管理システム

## できること
- 入力や表示を切り替えるフェーズを導入します
- システムに必要なインターフェースを提供します
- フェーズの切り替えや管理はインターフェースのインスタンスを介して行います
- 単純なフェーズ切り替えに加えて、スタックによる前のフェーズに戻す機能を提供します

## 環境の設定
- PhaseManagerスクリプトをシーン内のオブジェクトにアタッチします
- フェーズは`PhaseBase`クラスを継承して実装します
- 各フェーズはシングルトンであり、`Instance`プロパティで自身のインスタンスの生成とアクセスができます
- フェーズを切り替える時は、切り替えたいフェーズのインスタンスの`Request()`を呼び出します
- フェーズをスタックに積んで切り替える時は、インスタンスの`Push()`を呼び出します
- フェーズを戻す時は、インスタンスの`Pop()`を呼び出します
- ルートのフェーズまで戻す場合は、インスタンスの`PopAll()`を呼び出します
- デフォルトでは操作性の観点からRequest()、Push()、Pop()はフェーズの切り替え中は処理を受け付けません
- 何らかの理由で、切り替え後に次の切り替えを予約する場合は、Request(), Push(), Pop()の引数にtrueを設定します

## PhaseManagerの処理の流れ
### phaseStackが空のとき
- 何もしない
- 変更要求
  - Update()で現在のフェードを切り替えて、Init()を呼び出す
  - Update()やFixedUpdate()からフェーズの更新処理を呼ぶ。切り替え禁止などは更新処理で制御する
- 次のフェーズへの切り替え要求を呼ぶ
  - 現在の状態がRunning
    - 現在のフェーズのCanChangeがfalseなら要求を蓄積したまま次の更新に持ち越し
    - 現在のフェーズのCanChangeがtrueになったら、現在のフェーズのTerminateを呼ぶ
    - IsTerminatedがすぐにtrueになればフェーズの切り替えを実行してInitを呼び出す。falseならCurrentStateを終了待機に設定
  - 現在の状態がWaitTerminate
    - IsTerminatedがfalseなら次の処理へ持ち越し。UpdateやFixedUpdateを実行
    - IsTerminatedがtrueなら、フェーズの切り替えを実行してInitを呼び出す。CurrentStateはRunningに戻す

** ChangeやPushとPopで終了処理を呼ぶかどうかが異なるので再検討 **

## PhaseManager
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

## ライセンス
MIT License
