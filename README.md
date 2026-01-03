# 🏢 SmartBuildingIoT Dashboard

500Home Technology RD/AIエンジニア向けポートフォリオプロジェクト

## 技術スタック

| 技術 | 実装 |
|------|------|
| **ASP.NET Core 8** | Web API バックエンド |
| **C#** | 全バックエンドロジック |
| **SQLite** | データ永続化 |
| **JavaScript** | リアルタイムダッシュボード |
| **AI** | 異常検知 (Z-score) |

## プロジェクト構造

```
smart_building_iot/
├── Controllers/
│   ├── SensorController.cs    # センサーAPI
│   └── AnalyticsController.cs # AI分析API
├── Services/
│   ├── AnomalyDetector.cs     # AI異常検知
│   └── SensorSimulator.cs     # IoTシミュレータ
├── Models/
│   └── SensorData.cs          # データモデル
├── Data/
│   └── ApplicationDbContext.cs # EF Core DB
├── wwwroot/
│   └── index.html             # ダッシュボードUI
└── Program.cs                 # エントリーポイント
```

## 起動方法

```bash
# 依存パッケージ復元
dotnet restore

# 実行
dotnet run

# ブラウザでアクセス
# http://localhost:5000
```

## 機能

### 1. リアルタイムセンサー監視
- 温度、湿度、電力、CO2センサー
- 5秒間隔で自動更新

### 2. AI異常検知
- Z-scoreベースの統計的異常検知
- 自動アラート生成

### 3. 予測分析
- 電力消費予測
- トレンド分析 (Rising/Falling/Stable)

### 4. データ可視化
- Chart.jsによるリアルタイムグラフ
- モダンダークテーマUI

## API エンドポイント

| Method | Endpoint | 説明 |
|--------|----------|------|
| GET | `/api/sensor/readings` | 最新センサーデータ |
| GET | `/api/sensor/history/{id}` | 履歴データ |
| GET | `/api/sensor/summary` | 統計サマリー |
| GET | `/api/analytics/{id}` | AI分析結果 |
| GET | `/api/analytics/alerts` | アラート一覧 |
| GET | `/api/analytics/forecast` | 電力予測 |

## スクリーンショット

ダッシュボードはリアルタイムでセンサーデータを表示し、異常を検知するとハイライト表示されます。

## ライセンス

MIT License
