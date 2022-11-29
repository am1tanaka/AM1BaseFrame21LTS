# 状態システム仕様

　状態の管理をスタックで行うAM1StateStackと優先度付きキューで行うAM1StateQueueを提供します。

※シーン切り替えを伴う状態は、状態ごとの処理をシーン内のBehaviourで実行するのでこれらとは別の`SceneStateChanger`で管理します。

## 基本情報

### AM1StackQueue
- プレイヤーの行動やシナリオ進行などの状態によって動作を切り替えたい処理の優先度付きの状態管理を提供します
- AM1StateQueueクラスそのものか、継承したクラスをシーン内のオブジェクトにアタッチします
- 各状態は`AM1StateQueueBase`クラスを継承して実装します
- AM1StateQueue、および、AM1StateQueueBaseは各ゲームオブジェクトで独立して利用できるようにシングルトンにはしません。利用するゲームオブジェクト上で必要なインスタンスを生成して利用します
- 状態の切り替えはAM1StateQueueのインスタンスメソッド`Enqueue`を利用します。切り替え要求は優先度に従って並び替えながらListedListに登録して、Update()のタイミングで更新を確認します
  - Enqueue(AM1StateQueueBase)
    - 状態の切り替え要求を登録します
- 状態を切り替える時には、各状態の`CanChangeToOtherState`を`true`にします
- ステージクリアの状態などの優先度の低い処理をキャンセルしたい場合は`Cancel(int priority)`を実行します
  - void Cancel(int priority)

### AM1StateStack
- メニューやウィンドウなどの状態を戻す必要がある時に利用します
- AM1StateStackクラスそのものか、継承したクラスをシーン内のオブジェクトにアタッチします
- 各状態は`AM1StateBase`クラスを継承して実装します
- AM1StateStack、および、AM1StateBaseは各ゲームオブジェクトで独立して利用できるようにstaticによるインスタンス管理はしません。利用するゲームオブジェクト上で必要なインスタンスを生成して利用します
- 状態への切り替えには以下のAM1StateStackのインスタンスメソッドを利用します。通常は、状態の切り替え中の更なる状態切り替えは失敗扱いにしますが、イベント処理などで連続した遷移が必要な場合は、Queueがついたメソッドを利用することでキューへ予約して、連続切り替えができます
  - PopAndPushRequest() / PopAndPushQueueRequest()
    - 現在の状態を切り替えます
  - PushRequest() / PushQueueRequest()
    - 現在の状態を一時中止して、新しい状態に切り替えます
  - PopRequest() / PopQueueRequest()
    - 前の状態に戻します。状態を指定するとその状態まで、未指定だと１つ前に戻します
  - PopToRootRequest() / PopToRootQueueRequest()
    - 一番最初の状態まで戻します
  - PopAllRequest() / PopAllQueueRequest()
    - 全ての状態を戻して、実行中の状態をなくします

### AM1StateBase
QueueとStackの双方で利用する状態のAM1StateBaseには以下の基本メソッドがあります。

- bool CanChangeToOtherState
  - 他の状態へ切り替えてよいことをシステムに知らせるフラグ
- StateChangeAction ChangeAction
  - PushやPopなどの切り替えコルーチンを設定するデリゲート。キューでは未使用
- Init()
  - 状態を開始する時にUpdateに先立って呼ばれます
- Update(), FixedUpdate(), LateUpdate()
  - 実行中にMonoBehaviourから呼ばれます
- Pause()
  - 他の状態に一時的に遷移する前に呼ばれます
  - アプリの一時中断の時にも呼ばれます
- Resume()
  - 他の状態から復帰する時に呼ばれます
  - アプリの一時中断からの回復の時にも呼ばれます
- Terminate()
  - 状態を終了する時に呼ばれます

## StateQueueの切り替え
キューは、登録は優先度とインスタンスを受け取って無条件で受け付けます。登録時に優先度に応じて並び替えながらキューに積みます。切り替えはUpdate()時に以下のようなチェックをします。

- 実行中の状態がない時
  - キューから状態を取り出して切り替え
- 実行中の状態がある時
  - CanChangeToOtherStateがfalseなら切り替えなし
  - CanChangeToOtherStateがtrueなら現在の処理を終了させて、キューの状態を取り出して切り替え

次の状態への切り替えの前に時間がかかる終了処理が必要な場合は、Terminate()ではなく、Update()などの更新処理内で終了処理を完了させてからCanChangeToOtherStateをtrueにします。

CanChangeToOtherStateを最初からtrueにしておくと、同時に他の登録があったら何もせずに次の状態へ切り替えます。初期化や更新を1回は実行したい場合はCanChangeToOtherStateをfalseにしておく。


## StateStackの切り替え条件
状態切り替えの登録、切り替え実効の可否の判断基準です。

### 登録
- 通常のRequest()は、すでに他の状態が登録されていたり、状態の切り替え中の時は、登録に失敗します
- QueueRequest()は、すでに同じ状態が要求されていたら失敗します。それ以外の時は登録キューに登録して、前の処理が完了したら続けて切り替えを開始します

### 切り替え処理
- AM1StateStackのUpdate()で確認します
- 現在の状態のCanChangeToOtherStateフラグがfalseか、あるいは、前の切り替え処理が実行中(IsChangingがtrue)の時は、切り替え処理を次フレームに延期します


## AM1StateStackの処理

### stateStackが空(CurrentState==null)のとき
- 何もしない
- 変更要求は無条件で登録可能

### stateStackで状態を処理中(CurrentState != null)
- 現在の状態のUpdate()とFixedUpdate()を実行
- 変更要求
  - ???Request()を各箇所から呼び出してrequestQueueに積む
- stateStack.Update()の先頭でUpdateChangeRequest()を呼び出して切り替えチェックや処理
- UpdateChangeRequest()
  - 要求があり、切り替え中ではなく、現在の状態のCanChangeToOtherStateフラグがfalseの時、状態切り替えキューから要求を一つ取り出して、ChangeActionをコルーチンで開始
  - ChangeActionは、IEnumeratorを返して、IAM1Stateを受け取るデリゲート。受け取ったIAM1Stateに切り替えるための処理を実行
  - 登録する処理は、以下の通りPushState,
    - PushState
      - 現在の状態があれば一時停止してから、スタックに要求の状態を積んでInit呼び出し
    - PopPrevState
      - 一手戻す
    - PopState
      - 指定の状態まで戻す
    - PopToRootState
      - スタックの先頭の状態まで戻す
    - PopAllState
      - スタックを全て戻して状態をなしにする
