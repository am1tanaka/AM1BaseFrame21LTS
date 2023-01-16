# AM1State Change Log

## Ver0.3.1()
- AM1StateQueue.ClearAll()を追加
- AM1StateQueue.RequestTerminateCurrentState()を追加

## Ver0.3.0(2022/12/28)
- AM1StateQueueBaseのコンストラクタで引数を省略すると優先度0にするように機能追加
- AM1StateBaseとAM1StateQueueBaseに、Updateが呼ばれるごとにTime.deltaTimeを加算するupdateTimeを追加

## Ver0.2.0(2022/11/29)
- AM1StateQueueを追加

## Ver0.1.2(2022/11/5)
- AM1StateStackの不要なusingを削除

## Ver0.1.0
- 最初のバージョン
