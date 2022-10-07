# AM1BaseFrame21LTS
フレームワークや共通コード、フェーズシステムなどの自家製汎用パッケージの開発リポジトリです。本リポジトリのサブフォルダーで複数のパッケージの開発と公開をしています。

まだ実装方針から研究しながら実装しているので、安定したライブラリではりません。ドキュメントも未整理状態です。

## 対象バージョン
- 開発 Unity2021.3.10f1
- 対応 Unity2020LTS以降

## パッケージの利用
このプロジェクトは各種パッケージの開発用です。各パッケージの利用方法は以下を参照ください。

- [自家製フレームワーク](./Assets/AM1BaseFrame/README.md)
- [フェーズシステム](./Assets/AM1PhaseSystem/README.md)
- [共用ライブラリ](./Assets/AM1Utils/README.md)

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


---

以下、開発用のドキュメントです。

## サンプルフォルダーへのシンボリックリンクの作成
サンプルフォルダーはUnityの管理外にするため直に参照できない。以下でシンボリックリンクを作成してUnityに認識させる。

### Windows
参考 https://learn.microsoft.com/ja-jp/powershell/scripting/windows-powershell/wmf/whats-new/new-updated-cmdlets?view=powershell-7.2

- Assetsフォルダーへ移動
- 以下はPhaseSystemのサンプルへのシンボリックリンクの作成例

```
New-Item -ItemType SymbolicLink -Path ./AM1PhaseSystemSample.win -Value ./AM1PhaseSystem/Samples~
```

- .gitignoreに作成したシンボリックリンクを無視するよう設定

### Mac

```
ln -s ./AM1PhaseSystem/Samples~ .
```

作成後に`.mac`を付けて名前を変更して、.gitignoreに作成したシンボリックリンクを無視するよう設定。

シンボリックリンクの削除は以下。

```
unlink シンボリックリンクのパス
```


## 使用アセット
- キャラクター、マップチップ、背景
  - [www.kenney.nl Platformer Assets Base](www.kenney.nl)
- BGM
  - [www.kenney.nl Music Loops/Retro](www.kenney.nl)
効果音
  - [www.kenney.nl Interface Sounds](www.kenney.nl)
  - jump drop_004.ogg
  - Start confirmation_001.ogg
  - Cursor bong_001.ogg
  - Miss error_007.ogg
  - Success confirmation_002.ogg
