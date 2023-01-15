# 状態遷移ライブラリ
- Ver0.3.0

## 利用例
以下のような用途で利用できるキューやスタックの状態切り替えシステムです。

- ゲームシーンに切り替わった後、画面の覆いが外れるまで待ったり、開始のカウントダウンをしたり、メニュー時は操作を止めたりといった状態の導入
- 状況に応じて操作が大きく変わるプレイヤーキャラの処理を状態で切り分ける
- シーンの切り替えや現在の状況などの管理

## できること
- ゲームオブジェクトごとに利用できるUnityのライフサイクルにあわせた状態遷移システムを提供します
- 以下の2種類を提供します
  - 現在の状態の切り替えと、現在の状態を一時停止して新しい状態に切り替えたり、一時停止した状態を復帰させて戻す状態スタックの`AM1StateStack`
  - 切り替えたい状態を優先度付きでキューに登録して、登録した次のフレームで切り替え可能な場合は優先度が一番高くて最初に登録された状態に切り替える状態キューの`AM1StateQueue`
- 各状態には以下のメソッドを実装できます
  - Init()
    - 開始時に呼ばれます
  - Update(), FixedUpdate(), LateUpdate()
    - 実行中にMonoBehaviourから呼ばれます
  - Pause()
    - 他の状態に一時的に遷移する前に呼ばれます
  - Resume()
    - 他の状態から復帰する時に呼ばれます
  - Terminate()
    - 状態を終了する時に呼ばれます


## 状態の切り替え

### AM1StackQueue
状態キューへの登録には以下のようなルールや使い方があります。

- `AM1StateQueue.Enqueue(IAM1State)`を呼び出します
- 実行中のキュー状態の`CanChangeToOtherState`がtrueにならないと他の状態への切り替えは実行されません
- 登録済みでまだキューに積まれている状態は`AM1StateQueue.Cancel(優先度);`を呼び出して指定の優先度以下のものを削除できます


### AM1StateStack
状態スタックへの登録には以下のような各種登録メソッドが用意されています。

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
- 状態ごとにUpdate()が実行されるたびにTime.deltaTimeを加算する`updateTime`を提供。いつでも0を代入してリセットできる
- 状態の変更は、リクエストした次のフレームで実行されます
- アニメや終了処理中などのすぐに状態が切り替わると不都合な場合は、切り替えを停止するフラグを設定することで切り替え開始を延期できます


## 組み込み方
Unity2021.3.14f1での手順を示しています。別バージョンの場合は手順が異なるかも知れません。

### Package Managerを使う場合

1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. WindowメニューからPackage Managerを開きます
1. + をクリックして Add package from git URL... を選択します
1. `https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1State` と入力して、Addをクリックします

### manifest.jsonの依存関係を使う場合

1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. ProjectビューのAssetsを右クリックして Open C# Project を選択
1. ソリューションエクスプローラーのホームの隣の ソリューションと利用可能なビューとの切り替え をクリックして フォルダービュー にします
1. Packages/manifest.json を開きます
1. dependenciesに以下を追加します

```
    "jp.am1.statesystem": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1State"
```

上書き保存してUnityに切り替えたら自動的にインストールが始まります。

以上でパッケージのインポート完了です。

## チュートリアル
- [チュートリアル](./Documents/tutorial.md)


## 仕様
- [状態システム仕様](./Documents/spec.md)

## ライセンス
[MIT License](./LISENCE.md)

Copyright (c) 2022 Yu Tanaka
