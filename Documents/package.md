# パッケージ化
本プロジェクトはPackage Managerからインストールするフレームワークパッケージを開発するためのものです。Package Managerに登録するためのパッケージのルートは以下の場所にします。

`Assets/AM1Framework`

この直下に`package.json`を置き、`Runtime`や`Samples~`を置きます。

サンプル用のアセットは`Samples~`以下に置きますが、`~`が付いているとUnityが無視をするので開発ができません。そこで`Samples~`へのシンボリックリンクを、Windows用は`jp.am1.framework.samples.win`、mac用は`jp.am1.framework.samples.mac`という名前で作成して、`Assets`フォルダー内に入れてUnityからアクセスさせます。
