# AM1BaseFrame21LTS
Unity2021LTS向けにまとめた自家製フレームワークです。1週間ゲームジャムなどでの利用や設計の勉強を目的として開発しています。そのまま使っていただいても構いませんし、改造や反面教師とするなどご自由にご利用ください。

## 対象バージョン
- Unity2021LTS

## 機能
以下の機能を提供します。

- 状態の定義と切り替え
- 状態の切り替えと画面を覆う演出の連携
- 画面を覆う演出サンプルとしてフェードと扇形塗りつぶし
- BGMとシステム効果音の再生
- BGMと効果音のボリュームスライダーと設定の保存と読み込み
- 新しい状態やシーンを作成するエディター拡張

## インポート手順
以下、フレームワークのインポート手順です。Unity2021.3.10f1での手順を示しています。別バージョンの場合は手順が異なるかも知れません。

1. フレームワークを組み込みたいUnityプロジェクトを開きます
1. WindowメニューからPackage Managerを開きます
1. + をクリックして Add package from git URL... を選択します

![Add package from git URL...](./Documents/Images/readme00.png)

1. `` を入力して、Addをクリックします

以上でインポートが始まります。

## サンプル
超ミニゲーム。

- Package ManagerのSampleをインストール
- WindowメニューからTextMeshPro > Import TMP Essential Resources と Import TMP Examples and Extras をインストール
- 2D Tilemap Editorをインストール

## ライセンス

### 本体パッケージ
MITライセンス



### デモ使用アセット
以下、いずれもCC0。

- キャラクター、マップチップ、背景
  - [www.kenney.nl Platformer Assets Base](www.kenney.nl)
- BGM
  - [www.kenney.nl Music Loops/Retro](www.kenney.nl)
- 効果音
  - [www.kenney.nl Interface Sounds](www.kenney.nl)
