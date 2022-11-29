# AM1BaseFrame
Unity2021LTS向けにまとめた自家製フレームワークです。1週間ゲームジャムなどでの利用や設計の勉強を目的として開発しています。

そのまま使っていただいても構いませんし、改造や反面教師とするなどご自由にご利用ください。

## 対応バージョン
- 開発 Unity2021.3.14f1
- 対応 Unity2020LTS以降

## 機能
以下の機能を提供します。

- シーンの状態の定義と切り替え
- シーン切り替えと画面を覆う演出の連携
- 画面を覆う演出サンプルとしてフェードと扇形塗りつぶし
- BGMとシステム効果音の再生
- BGMと効果音のボリュームスライダー
- PlayerPrefsへのボリューム設定の保存と読み込み
- 新しい状態やシーンを作成するエディター拡張

## インポート手順
以下、フレームワークのインポート手順です。Unity2021.3.14f1での手順を示しています。別バージョンの場合は手順が異なるかも知れません。

### Package Managerを使う場合
1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. WindowメニューからPackage Managerを開きます
1. + をクリックして Add package from git URL... を選択します
1. `https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1Utils` と入力して、Addをクリックします
1. `https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1BaseFrame` と入力して、Addをクリックします

### manifest.jsonの依存関係を使う場合

1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. ProjectビューのAssetsを右クリックして Open C# Project を選択
1. ソリューションエクスプローラーのホームの隣の ソリューションと利用可能なビューとの切り替え をクリックして フォルダービュー にします
1. Packages/manifest.json を開きます
1. dependenciesに以下を追加します

```
    "jp.am1.baseframe": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1BaseFrame",
    "jp.am1.utils": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1Utils"
```

上書き保存してUnityに切り替えたら自動的にインストールが始まります。

以上でパッケージのインポート完了です。

最後の要素は最後の`,`は不要です。よく分からない場合はPackage Managerをご利用ください。

## 最初の設定
必要なアセットをインポートします。

1. Toolsメニューから AM1 > Import BaseFrame Assets を選択します
1. 起動スクリプトを保存するフォルダーを選択します。起動スクリプトから開発プロジェクトへアクセスできるように、フレームワークではなく開発プロジェクトのスクリプトフォルダーを選択してください
1. インポートダイアログが表示されたら Import ボタンをクリックします
  - 途中でエラーが表示されてもインポートが完了したら消えるので気にせず進めてください

インポートが完了したら、状態を管理するシステムシーンを作成します。

1. システム用のシーンを作成するか開きます
1. Toolsメニューから、AM1 > Set StateSystem to Active Scene を選択して、追加 をクリックします
1. Hierarchyウィンドウから Booter オブジェクトをクリックして選択します
1. Inspectorウィンドウの Add Component から Booter スクリプトをアタッチします

以上で初期設定が完了します。開始すると画面が真っ白になります。これは起動に備えて画面を覆う処理を実行したままになっているからです。最初の状態とシーンを作成して切り替えることで処理を開始できます。

## 本システムの考え方
本システムは、状態やBGM再生を管理するシステムシーンを最初に起動して常駐させて、タイトルやゲームなどの状態にあわせたシーンをマルチシーンで追加読み込みする仕組みです。スコアやプレイヤー情報などのアプリ全体で利用する情報やUIはシステムシーンで管理すると便利です。

先に作成した`Booter`や起動時の処理を実行する`BootSceneStateChanger`には、起動時に一度だけ実行したい処理を実装します。例えばBGMや効果音のボリュームの設定はBootSceneStateChangerで読み込んでいます。BootSceneStateChangerの最後で、起動したい状態へのリクエストを出すことでアプリを開始します。先に実行していた状態を保存しておいて、アプリを再起動する時に保存した状態を読み込んで該当状態から再開すれば、スマホの中断からの復帰が便利になります。

状態とシーンを管理するためのスクリプトや最小限のオブジェクトが必要になります。新しい状態やシーンを作成する場合は、Tools/AM1メニューを利用します。

## 最初の状態とシーンを作成する
最初の状態としてタイトルシーンを作成する例を示します。

1. Toolsメニューから AM1 > New SceneState を選択します
1. 状態名に`Title`と入力します。自動的にシーン名にも Title が入力されます。ここでシーンの作成や切り替え時の演出が選べますが、とりあえずそのまま Create で作成します
1. シーンとスクリプトの作成フォルダー選択ダイアログが開くので、開発しているプロジェクトのフォルダーを選択します。特にフォルダーを作成していない場合はAssetsフォルダーで構いません。選択したフォルダー内に`Scenes`フォルダーと`Scripts`フォルダーがあれば、その中にシーンとスクリプトを作成します。それらのフォルダーがなければ選択したフォルダー内に作成します
1. ScenesフォルダーなどにTitleシーンが作成されるので、FileメニューのBuild Settingsを開いて Scenes in Build 欄に追加します
1. 必要なアセットをインポートする時に選択したスクリプトフォルダーから BootSceneStateChanger スクリプトを開きます
1. 下の方に「ここに最初の状態への切り替え要求を追記」という行があるので、その下のコメントされている行のコメントを外して有効にして、上書き保存します

プログラムを開始すると、白い覆いが扇状に消えてタイトルシーンが表示されます。

最後の手順でコメントから復帰させた以下の行は、タイトル状態に切り替えるためのコードです。引数の`true`はすでに状態切り替え中だったら切り替えが終わってからさらに状態を切り替える予約をするための引数です。通常は省略します。

```cs
// 30:
        TitleSceneStateChanger.Instance.Request(true);
```

## ボリュームシステム
執筆予定。

## 新しいシーンを作ってスクリプトから切り替える
執筆予定。


## サンプル
超ミニゲームのサンプルです。まずは必要なパッケージをインストールします。

- WindowメニューからTextMeshPro > Import TMP Essential Resources と Import TMP Examples and Extras をインストールします
- WindowメニューからPackage Managerを開きます
- Packagesを Unity Registry にします
- 2D Tilemap Editorがインストールされていなかったら、インストールします
- Package Managerの左上のPackagesを In Project にします
- AM1 Base Framework を選択します
- Samples をクリックして Import をクリックします
- インポートが終わったらPakcage Managerを閉じます
- Toolsメニューから AM1 > Set Demo Tags and Layers を選択して、読み込む をクリックしてデモで利用しているタグとレイヤーを読み込みます
- Fileメニューから Build Settings を開きます
- Projectウィンドウで Assets > Samples > AM1 Base Framework > バージョン番号 > Demo > Scenes フォルダーを開きます
- 開いたフォルダーにある4つのシーンを全て Scenes In Build に加えます
- DemoSystem シーンを Scenes In Build の先頭に移動します
- DemoSystemシーンをダブルクリックして開きます

以上で実行できます。Gameビューのレイアウトは16:9、あるいは、960x540で実行すると想定したレイアウトになります。

マウスクリックかスペースキーで開始やジャンプをします。長押しするとジャンプの高さが変わります。落下しないように右にあるゴールを目指すミニゲームです。シーンの切り替えやボリューム制御などのサンプルです。

## ライセンス

### 本体パッケージ
[MIT License](./LICENSE.md)

Copyright (c) 2022 Yu Tanaka

### デモ使用アセット
以下、いずれもCC0。

- キャラクター、マップチップ、背景
  - [www.kenney.nl Platformer Assets Base](www.kenney.nl)
- BGM
  - [www.kenney.nl Music Loops/Retro](www.kenney.nl)
- 効果音
  - [www.kenney.nl Interface Sounds](www.kenney.nl)
