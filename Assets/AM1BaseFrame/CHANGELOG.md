# AM1BaseFrame Change Log

## Ver0.9.3(2023/6/23)
- SceneStateChangerのAwakeでstaticをリセットするように修正

## Ver0.9.2(2023/2/22)
- .editorconfigの文字エンコードを全ファイル共通に変更

## Ver0.7.1(2022/11/30)
- gitignoreにローカル設定フォルダーを追加

## Ver0.7.0(2022/11/29)
- StateChangerだとStateStackやStateQueueと混乱するので`SceneStateChanger`にリネーム

## Ver0.6.5(2022/10/27)
- ScreenTransitionに色指定付きメソッドを追加

## Ver0.6.4(2022/10/27)
- StateChangerの完了待ちメソッドを外部から利用できるように公開

## Ver0.6.3(2022/10/23)
- 効果音の起動時にMixerの反映遅れ対策で微小ボリュームで空再生を追加

## Ver0.6.2(2022/10/21)
- ボリューム(VolumeBase)とオーディオソース、オーディオクリップのクラス(AudioSourceAndClips)を分離
- ボリュームスライダーの初期化をシステムにある時でも個別シーンでも可能なように修正

## Ver0.6.1
- ボリュームスライダーの初期化をinitEventsに登録して、起動時にボリュームを設定した後に呼び出すように修正

## Ver0.6.0
- シングルトンを共通パッケージ`AM1.Utils`に分離

## Ver0.5.2
- 最初の公開
