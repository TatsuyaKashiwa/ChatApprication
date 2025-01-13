# MagicOnionを用いたチャットアプリケーション

- 通信方式にMagicOnionを用いたチャットアプリケーションです

## 各プロジェクトについて

### ChatApplication.Clientプロジェクト
- チャットアプリケーションのクライアント機能のプロジェクトです
  -　Program.cs           : クライアント処理を行うためのクラスです
  - EntryDistinguisher.cs : Program.csでの入力に対して、Program.csにて行う処理に対応する列挙値を返却します
    - 拡張メソッドを利用するためプロジェクト内のDistinguishersフォルダ内に配置しました
  - CommandProcesser.cs   :  Program.csにてEntryDistinguisher.cs の結果に対応する処理を行うためのクラスです。
    - 拡張メソッドを利用するためプロジェクト内のCommandProseccingフォルダ内に配置しました

### ChatApplication.ServiceDefinitionプロジェクト
- チャットアプリケーションのサービス定義(CL-SV間でやり取りをするメソッドや独自型)のプロジェクトです
 - IChatService.cs : サービス定義に関する記述を行うインタフェースです  

### ChatApprication.Serviceプロジェクト
- チャットアプリケーションのサーバに関するプロジェクトです。サーバを立ち上げるためこのプロジェクトのみASP.NET Coreのプロジェクトとしています。
  - Program.cs     : サーバ起動のためのクラスです
  - ChatService.cs : サーバでの処理を行うためのクラスです。
