# パッケージ化

## パッケージ構成
本プロジェクトはPackage Managerからインストールするフレームワークパッケージを開発するためのものです。`Assets`フォルダー下にパッケージ用のフォルダーを作成します。

`package.json`や`Runtime`、`Samples~`フォルダーは`Assets`フォルダー下に作成した各パッケージ用のフォルダーの直下に置きます。

利用先プロジェクトのAssetsフォルダーに展開したいアセットは`Assets/AM1`フォルダー内にフォルダーを作成して管理します。フォルダーをエクスポートしたunitypackageを`Assets/AM1BaseFrame/Package Resources`フォルダー内にコピーします。

サンプル用のアセットは`Samples~`フォルダー以下に置きます。Unityは最後に`~`が付いているフォルダーを無視するので、`Samples~`へのシンボリックリンクを作成します。

## サンプルフォルダーへのシンボリックリンクの作成
サンプルフォルダーはUnityの管理外にするため直に参照できません。以下でシンボリックリンクを作成してUnityに認識させます。

### Windows
参考 https://learn.microsoft.com/ja-jp/powershell/scripting/windows-powershell/wmf/whats-new/new-updated-cmdlets?view=powershell-7.2

- PowerShellを起動
- プロジェクトのAssetsフォルダーへ移動
- 以下、BaseFrameのサンプルへのシンボリックリンクの作成例

```
New-Item -ItemType SymbolicLink -Path ./AM1BaseFrameSamples.win -Value ./AM1BaseFrame/Samples~
```

- .gitignoreに作成したシンボリックリンクを無視するよう設定

### Mac

- ターミナルを起動
- プロジェクトのAssetsフォルダーへ移動
- 以下、BaseFrameのサンプルへのシンボリックリンクの作成例

```
ln -s ./AM1BaseFrame/Samples~ .
```

作成後に`.mac`を付けて名前を変更して、.gitignoreに作成したシンボリックリンクを無視するよう設定。

シンボリックリンクの削除は以下。

```
unlink シンボリックリンクのパス
```

