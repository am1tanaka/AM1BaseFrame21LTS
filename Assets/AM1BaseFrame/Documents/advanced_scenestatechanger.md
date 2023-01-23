# SceneStateChangerの応用

基本的なシーンの切り替え方法は[こちら](howto_changescene.md)で紹介しています。ここではシーン切り替えシーケンスの利用ケースを紹介します。

## シーン切り替えの流れ

シーン切り替えを要求してから切り替わりが完了するまでのUnityとのライフサイクルとの

1. ????SceneStateChanger.Instance.Request()を実行。SceneStateChangerのキューに登録
1. 次のフレームのSceneStateChangerのUpdate



## 汎用ゲームシーンとステージのマルチシーンの初期化


## リトライの実装


