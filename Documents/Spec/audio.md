# オーディオ管理
BGMとシステム効果音を簡易に鳴らすための簡易オーディオ管理システム。BGM、効果音でそれぞれ別のボリュームを設定でき、スライダーとの連携、インターフェースを実装したクラスを使って設定の読み書き、効果音を同時に鳴らそうとした時に遅延させる機能などを提供する。

効果音などに3Dサウンドを適用する場合は、再生にはここで用意するシステムは使わない。ボリュームの設定は、AudioSourceの出力先をAudio MixerのSEに設定することで共有することができる。

## 機能
- BGMのAudioClipをまとめて登録、インデックスによる再生開始、停止
- 登録したBGMのリストをenumで定義することと、enumの指定でBGMの開始を指定
- システム効果音のAudioClipをまとめて登録、インデックスによる再生開始、停止
- システム効果音が同時に鳴ることを抑制して、必要に応じて遅延再生する
- BGMと効果音のボリューム設定
- BGMのフェードイン、フェードアウト
- ボリューム設定の保存、読み込み

## BGM再生に関するクラスなど
- abstract AudioSourceAndClipsBase : MonoBehaviour
  - BGMSourceAndClipsやSESourceAndClipsのベースクラス
  - オーディオ設定やボリューム反映などを実装
- BGMSourceAndClips : AudioSourceAndClipsBase
  - BGM用のAudioClipの登録とBGMの再生開始、停止
  - フェードイン、フェードアウトの管理
- static Demo.BGMPlayer
  - enumでBGMClipsに設定したAudioClipの種類を定義して、再生開始をenumで呼び出すメソッドを提供するクラス

## 効果音再生に関するクラスなど
- SESourceAndClips : AudioSourceAndClipsBase
  - システム効果音のAudioClip登録とインデックスによる再生を担当する
  - 一定時間以内に同じ効果音を鳴らさないようにインデックスごとに最後に再生した時間を記録
  - 遅延再生
- static Demo.SEPlayer
  - BGMPlayerと同様
- DelaySEPlayer PureC#
  - 効果音の遅延再生を行う
  - 対応するAudioSourceAndClipsでインスタンスをnewして保有して、FixedUpdateからFixedUpdate()を呼び出す
  - 再生秒数、AudioClip、再生先のAudioSource
  - データはQueueで持つ

## ボリューム関連
ボリュームの設定はDemoではBGMと効果音のみだが、マスターボリュームなどを拡張できるように考える。BGMと効果音を専用のものとしてまとめずに、ボリューム1つ分を管理する汎用クラスを作り、起動時の初期化などでBGMや効果音のボリュームとして機能するように割り当てる。

### 汎用
- IVolumeSaver
  - VolumeSettingに渡して保存データにアクセスするためのインターフェース
  - Load(int 初期値)
  - Save(int 保存値)
- VolumeSetting
  - ボリューム値の保持、公開、変更、インターフェースを通した読み書きを汎用的に行うクラス
  - PureC#
  - このクラスのインスタンスのリストをstaticで保持する
  - BootStateChangerなどで初期化と割り当てを行う
  - SetSaver(IVolumeSaver)
    - 該当するボリュームの
  - static int volumeMax = 5
    - ボリュームの最大値
    - 初期化前に値を変更すると変更できる
- abstract VolumeSaverWithPlayerPrefs : IVolumeSaver
  - PlayerPrefsでボリュームを読み書きするクラスのベースクラス
  - このクラスを継承したクラスに、対応するボリュームのキーをKeyNameでオーバーライド
- Demo.VolumeSlider : MonoBehaviour
  - BGMや効果音のボリュームスライダーに割り当てるクラス
  - VolumeSetting.TypeでBGMかSEを設定
  - 初期化時にTypeのインデックスに該当するボリュームを読み込んでスライダーに設定
  - 変更時にTypeのインデックスを指定してボリュームを設定する
  - 設定したVolumeTypeに対応するボリュームにVolumeSetting.volumeSettingsを通してアクセス

### 専用
- Demo.BGMVolumeSaverWithPlayerPrefs : IVolumeSaver
  - BGM用の設定の読み書きをPlayerPrefsから行うための専用クラス
- Demo.SEVolumeSaverWithPlayerPrefs : IVolumeSaver
  - 効果音用の設定の読み書きをPlayerPrefsから行うための専用クラス
- enum Demo.VolumeType
  - BGMとSEを定義して、ボリュームの指定に使う


### ボリュームの初期化の流れ
BGMと効果音のボリュームの読み込みと、設定用のスライダーへの初期にの反映の流れを整理する。

保存してある設定の読み込みはBootStateChanger.Init()で行い、読み込んだ値をVolumeSettingクラスに設定する。

スライダーの設定はタイトルシーンのStart()で行う。この時点で設定は読み込み済みなので、プロパティで読み取ったボリュームをスライダーに反映させればよい。スライダーの値が変更された時は、BGMSliderとSESliderをそれぞれアタッチして、VolumeSettingクラスのセッターを呼ぶ。

ボリュームはVolumeSettingクラスで管理する。SetSEVolume(),SetBGMVolume()とSEVolue, BGMVolumeで値の設定、読み込みができる。


TitleBehaviourのBGMかSEのボリューム設定メソッドを呼ぶようようにして、TitleBehaviourからSystemシーンの該当メソッドを呼ぶようにする。


## 遅延再生
AudioSourceAndClipsBaseを継承したクラスとDelaySEPlayerを連携して処理する。

- 効果音再生クラスなどからDelaySEPlayを以下を指定してnew
  - 管理するチャンネル数
  - AudioSource
- FixedUpdateからFixedUpdate()を呼び出す
- 効果音の再生はDelaySEPlayer.Play(index, AudioClip)を呼び出す
  - DelaySEPlayer内で指定のインデックスを最後に鳴らした時間を管理
  - 一定秒数が経過していたらすぐに再生して再生時間を更新して終了
  - 一定時間以内なら、キューに再生時刻と鳴らすAudioClipを記録
  - FixedUpdateでキューをチェックして、時間がきたら記録していたAudioClipを再生する

### 機能
- PlayBGM(int index[, float sec])
  - 指定のインデックスのオーディオをBGMとして再生
  - 第２引数が設定されている時、指定秒数でフェードイン再生
- StopBGM([float sec])
  - BGMを停止
  - 引数が設定されている時、指定秒数でフェードアウト
- PlaySE(int index)
  - 指定のインデックスの効果音を再生
- StopSE()
  - 再生中の効果音を停止
- SetSettingSaver(IAudioSettingSaver)
  - 設定の読み書きに使うインスタンスを受け取る
- BGMVolume, SEVolume
  - 現在のBGMとSEのボリュームを返すプロパティ
- SetBGMVolume(int), SetSEVolume(int)
  - スライダーなどでボリュームを変更した時に設定する

このクラスはシステムシーンのAudioPlayerオブジェクトにアタッチして設定。

### BGMとSEのデータの対応
　BGMPlayerとSystemSEPlayerクラスに再生インデックスとAudioPlayerのメソッドを呼び出すstaticメソッドを定義する。

　BGMやSEを鳴らす時はこのクラスから呼び出す。SEには同時再生を防ぐバッファ機能を持たせる。

### テスト
- データの設定はインスペクターで行うのでシーンを起動してからタイトルシーンで実施する
- BGMPlayerで通常再生と停止
- BGMPlayerで次の曲を通常再生と停止
- BGMPlayerで1秒のフェードイン、1秒のフェードアウト
- BGMPlayerで1秒のフェードイン中に1秒のフェードアウト
- BGMPlayerで曲を鳴らして1秒のフェードアウト中に次の曲を鳴らす
- SystemSEPlayerで各種効果音再生
- SystemSEPlayerで同じ効果音を同時に3回鳴らす

#### ボリューム
- ボリュームの初期値を確認
- ボリュームを変更して反映を確認


## 設定の保存
　BGMと効果音のボリュームは、IAudioSaverインターフェースを介して行う。インスタンスの設定時に読み込みと初期化を行う。インスタンスがない時は保存や読み込みはしない。

## IAudioSettingSaverインターフェース
- LoadBGMVolume(初期値)
  - BGMのボリュームを読み込む。読めなかった場合は初期値をそのまま返す
- LoadSEVolume(初期値)
  - SEのボリュームを読み込む。読めなかった場合は初期値をそのまま返す
- SaveBGMVolue(保存値)
  - BGMのボリュームを書き込む
- SaveSEVolue(保存値)
  - SEのボリュームを書き込む
- ClearSaveData()
  - 保存データを削除

## AudioSettingSaverWithPlayerPrefs
　PlayerPrefsで保存するクラス。Demoに同梱。デバッグ用にファイル名のprefixを設定するメソッドを提供。

## テスト
1. テスト用のプレフィックスを設定
1. 設定削除
1. 読み込み実行。戻り値が初期値と同じことを確認
1. 別の値を保存
1. 読み込みチェック
1. 別の値を保存
1. 読み込みチェック

## 効果音の時間差再生
効果音を同時に鳴らすとボリュームが大きくなるため、同じフレームでの再生は1回にとどめ、既定の時間後に鳴らす仕組みを実装する。メモリ効率を考え、ジェネリックなリングバッファAM1RingBufferを作成して、再生までの秒数と再生する効果音を既定の数積み立てられるようにする。

AM1RingBufferは内部はListでデータを保持して、あとから要素数を変更できるように実装する。



  ## クラスやインターフェースの設計と作成
  これらの作成はテキストで箇条書きをしたり、UMLのクラス図で検討することができるが、Visual Studioのクラスデザイナーを使えばクラス図を作ったり、定義したクラスをクラス図として表示ができるので、設計の検討段階から導入することも便利そうである。

  Visual Studioでクラスデザイナーを利用するには、以下の手順に従ってインストーラーから必要なモジュールを追加する。

  https://docs.microsoft.com/ja-jp/visualstudio/ide/class-designer/how-to-add-class-diagrams-to-projects?view=vs-2022

  Visual Studioにクラスデザイナーをインストールしたら、UnityでC#スクリプトを作成して、それをクラスデザイナーで表示して編集する、という流れになる。

  ### 作業の流れの例
  以下、Visual Studioのクラスデザイナーを使ってクラスやインターフェースを検討しつつ、定義する作業の流れである。

  1. Unityで必要になりそうなクラスやインターフェースをC#スクリプトを作成
  1. 作成したスクリプトをVisual Studioで開く
  1. ネームスペースを利用していたらコードに宣言を追加する

  クラスダイアグラムファイルを作成していない場合は以下の手順で作成する。

  1. ソリューションエクスプローラーでクラス図を作りたいプロジェクトを右クリックして、追加 > 新しい項目 を選択
  1. 項目から 全般 を選んで、クラスダイアグラム を選択して、任意の名前を入力して追加。例えば`SoundSystemClassDiagram.cd`など

  作成したクラスダイアグラムファイル(.cd)をダブルクリックすると、クラスデザイナーが開く。ツールボックスから必要なものをドラッグ＆ドロップすることで、UMLのクラス図を作成する要領でクラスなどを作成できる。

  また、ソリューションエクスプローラーから既存のC#ファイルをクラスデザイナーにドラッグ&ドロップすれば、自動的にクラス図が追加される。Unityのファイルの場合はMonoBehaviourを継承しているので、UnityでC#スクリプトを作成してから、クラスデザイナーにドラッグ&ドロップすると楽だと思われるが、クラスデザイナー側で作成して、MonoBehaviourを継承しても構わない。

  クラスデザイナー上で編集したいクラスなどをクリックして選択すると、クラスの詳細欄でメソッドやプロパティの選択や編集、追加、削除ができる。
  ## Todo
  - [x] BGMと効果音のボリュームテストを通す
  - [x] 効果音の再生のテスト
  - [x] 効果音の再生の実装
  - [x] BGMと効果音のスライダー
  - [x] DelaySEPlayerクラスを作成
