# Overview
----------------
囲碁の配石や手順などを記録する「SGF」形式ファイルの座標データを一次元配列に変換するプロジェクト。
手順の分岐（枝分かれ）にも対応済みです（処理速度は遅い）。

サンプルSGFファイルを3つ入れておいたので、よければテスト用にご利用ください。

### テスト環境

* Unity2018.1.2f1 (64-bit)
* 上記以外のバージョンでは未検証

### インストール

Clone or downloadと書かれた緑色のボタンからダウンロード、またはコマンドラインからリポジトリをgit clone！

### 使い方

1. SGFファイルをtxt形式に変換して、プロジェクト内のAssets/SGF以下に入れる
2. UnityエディタでMainシーンを開き、Hierarchyビューにある「SGFParser」オブジェクトを選択する
3. Inspectorビューの右上にある南京錠のようなボタンを押してロックする
4. SGFParserオブジェクトの「SGF Files」の横の▼を押して項目を表示する
5. ProjectビューのAssets/SGF以下に入れてあるtxtファイルを「SGF Files」の文字のところまでドラッグ＆ドロップする
6. 最後にUnityEditorを実行すればConsoleビューに変換結果がログ出力されます！

### ライセンス

[MIT Licence](https://github.com/intenseG/SGFParser/blob/master/LICENSE)

### Author

[intenseG](https://github.com/intenseG)