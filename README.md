# メモリーゲーム (Memory Game)

## 概要 (Project Overview)

画像の神経衰弱（メモリーゲーム）をブラウザ上で遊ぶためのアプリケーションです。

## 事前準備（画像の用意）

1. プロジェクトのルートディレクトリに **`Images`** フォルダを作成します。
2. **`Images`** フォルダの中に **12枚以上** の画像ファイル（`.jpg`, `.png`, `.gif`, `.webp` 等）を配置してください。

※ ゲームでは 12ペア（計24枚）のカードが生成されるため、12枚以上の画像が必要となります。

## 遊び方・起動手順

ご利用環境に合わせて、以下のいずれかの方法でサーバーを起動してください。

### A. Node.js がインストールされている場合

1. ターミナル（コマンドプロンプト等）でプロジェクトルートを開きます。
2. 以下のコマンドを実行して Node.js サーバーを起動します。

```bash
node server.js
```

3. Web ブラウザを開き、`http://localhost:8080/` にアクセスしてゲームを開始します。

---

### B. Node.js がインストールされていない場合（C# アプリを使用）

1. 以下のコマンドで C# アプリをビルドします。

```bash
# Debug ビルド
powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' WinForm.csproj -p:Configuration=Debug"

# Release ビルド
powershell.exe -Command "& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' WinForm.csproj -p:Configuration=Release"
```

2. 生成された WinForm アプリケーション（`WinForm.exe`）を起動します。
3. 画面内の **「start」** ボタンを押して内蔵サーバーを起動します。
4. **「ブラウザを開く」** ボタンを押すか、表示された URL（`http://localhost:8080/`）へブラウザでアクセスしてゲームを開始します。
