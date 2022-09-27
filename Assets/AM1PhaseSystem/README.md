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

## IPhaseRequest
- Request(bool reserve=false)
  - 現在の階層のフェーズを切り替えるようPhaseManagerへ要求します
- Push(bool reserve=false)
  - 現在のフェーズに戻れるようにスタックに積んで切り替え要求します
- Pop(bool reserve=false)
  - 一つ前のフェーズに戻す要求をします
- PopAll(bool reserve=false)
  - ルートのフェーズまで戻すよう要求します

## IPhase
- enum State.None, Init, Update, Terminate
- State CurrentState
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
