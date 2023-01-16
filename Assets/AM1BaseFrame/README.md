# AM1BaseFrame
Unity2021LTS向けにまとめた自家製フレームワークです。1週間ゲームジャムなどでの利用や設計の勉強を目的として開発しています。

そのまま使っていただいても構いませんし、改造や反面教師とするなどご自由にご利用ください。

## 対応バージョン
- 開発 Unity2021.3.16f1
- 対応 Unity2020LTS以降(C# 8.0以降)

## 機能
以下の機能を提供します。

- シーンの状態の定義と切り替え
- シーン切り替えと画面を覆う演出の連携
- 画面を覆う演出サンプルのフェードと扇形塗りつぶしを同梱
- BGMとシステム効果音の再生
- BGMと効果音のボリュームスライダー
- PlayerPrefsへのボリューム設定の保存と読み込み
- 新しい状態やシーンを作成するエディター拡張

## インポート手順
以下、フレームワークのインポート手順です。Unity2021.3.16f1での手順を示しています。別バージョンの場合は手順が異なるかも知れません。

### Package Managerを使う場合
1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. WindowメニューからPackage Managerを開きます
1. + をクリックして Add package from git URL... を選択します
1. `https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1Utils` と入力して、Addをクリックします
1. `https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1BaseFrame` と入力して、Addをクリックします

### manifest.jsonの依存関係を使う場合

1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. テキストエディターなどで`Packages/manifest.json`を開きます
   - Visual Studioなら以下の操作で開きます
     - ProjectビューのAssetsを右クリックして Open C# Project を選択
     - ソリューションエクスプローラーのホームの隣の ソリューションと利用可能なビューとの切り替え をクリックして フォルダービュー にします
     - `Packages/manifest.json`を開きます
1. dependenciesの最初に以下を追加します

```
    "jp.am1.baseframe": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1BaseFrame",
    "jp.am1.utils": "https://github.com/am1tanaka/AM1BaseFrame21LTS.git?path=/Assets/AM1Utils",
```

上書き保存してUnityに切り替えたら自動的にインストールが始まります。

以上でパッケージのインポート完了です。操作が分からない場合はPackage Managerをご利用ください。

## 最初の設定
パッケージのインストールが完了したら、プロジェクトで利用するアセットをインポートします。

1. Toolsメニューから AM1 > Import BaseFrame Assets を選択します
1. 起動スクリプトを保存するフォルダーを選択します。フレームワークではなく、**開発プロジェクトのスクリプトフォルダーを選択**してください
1. インポートダイアログが表示されたらImportボタンをクリックします
  - 途中でエラーが表示されてもインポートが完了したら消えるので気にせず進めてください

インポートが完了したら、状態を管理するシステムシーンを作成します。

1. システム用のシーンを作成するか、開きます
1. Toolsメニューから、AM1 > Set StateSystem to Active Scene を選択して、追加 をクリックします
1. Systemシーン用のオブジェクトの生成ダイアログが表示されたら追加をクリックします
1. TextMeshProのダイアログが表示されたらImport TMP Essentialsをクリックします
1. インポートが完了したらダイアログを閉じます
1. HierarchyウィンドウからBooterオブジェクトをクリックして選択します
1. InspectorウィンドウのAdd ComponentからBooterスクリプトをアタッチします

以上で初期設定が完了します。開始すると画面が真っ白になります。これは起動に備えて画面を覆う処理を実行したままになっているからです。最初の状態とシーンを作成して切り替えることで処理を開始できます。



## シーン構成と切り替え本システムの考え方
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

## Assembly Definition
テストを行うなどでAssembly Definitionをプロジェクトのスクリプトフォルダーに作成する時は、以下への参照を加えてください。

- AM1.BaseFrame
- AM1.BaseFrame.Assets
- AM1.Utils


## ボリュームシステム
執筆予定。

## 新しいシーンを作ってスクリプトから切り替える
執筆予定。

## 追加シーンの作成
例えばタイトルシーンの時に、別のシーンに作成したステージ選択のUIを重ねて表示したいような場合があります。その時には追加シーンを利用します。

本システムでは、シーン内のいずれかのゲームオブジェクトのAwake()で`SceneStateChanger.AwakeDone(gameObject.scene.name);`を呼ぶことでシーンの読み込みが完了したことをシステムに知らせる必要があります。手動で作成したシーンをUnityのSceneManager.LoadScene()やLoadSceneAsync()で呼び出しただけだと、シーンの初期化が完了したかをシステムが判定できないため、シーン切り替え処理が停止してしまいます。

追加シーンを作成するには、Toolsメニューから New BaseFrame Scene を選択してするとシステムに必要な処理を実行するゲームオブジェクトとスクリプトがアタッチされたシーンが作成されます。

なお、手動で作成したシーンの場合は、シーン内のいずれかのゲームオブジェクトにアタッチしたスクリプトのAwake()で以下を実行してください。

```cs
        private void Awake()
        {
            if (SceneStateChanger.IsReady)
            {
                SceneStateChanger.AwakeDone(gameObject.scene.name);
            }
        }
```

追加シーンは、任意のタイミングで以下で読み込みを開始できます。

```cs
    SceneStateChanger.LoadSceneAsync("シーン名", false);
```

読み込みは非同期で実行されます。処理の完了はコルーチン内で以下で待てます。

```cs
yield return SceneStateChanger.WaitAsyncAndAwake();
```

追加シーンは解放も手動でやる必要があります。

```cs
SceneStateChanger.UnloadSceneAsync("シーン名");
```

解放待ちは上記の`WaitAsyncAndAwake();`でも待ちますし、解放だけなら以下で待ちます。

```cs
yield return SceneStateChanger.WaitUnloadscenes();
```

### シーン状態に連動して追加シーンを読み込んだり解放する

シーン状態に連動して追加シーンを読み込みや解放する場合は、該当するシーン状態の????SceneStateChangerに処理を書きます。本システムでは、画面を覆う演出の開始と同時にシーンの読み込みを非同期に開始することで読み込み時間を削減する作りになっています。

読み込みは、該当するシーン状態の????SceneStateChangerの`Init()`で行うのが通常です。自動的に作成されているシーン読み込みの書き方に習って追加シーンの読み込みを追加してください。画面を覆う処理の途中で読み込みが完了しても大丈夫なように、単独で呼び出す時とは２番目の引数が変わるのでご注意ください。

```cs
    SceneStateChanger.LoadSceneAsync("シーン名", true);
```

読み込みの完了待ちはシステム側で自動的に行うのでWaitAsyncAndAwake()で待つ必要はありません。

シーンの解放は、シーン状態の????SceneStateChangerの`Terminate()`で実行します。これもメインのシーンの解放処理が予め書かれているので、それと同じように不要になった追加シーン名を指定してSceneStateChanger.UnloadSceneAsync()で解放を指示します。



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
