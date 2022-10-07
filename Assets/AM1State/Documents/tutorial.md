# 状態システム　チュートリアル

## 実装内容
TestState1, TestState2, TestState3という3つの状態を行き来するサンプルです。

操作は以下の通りです。

- 右キー
  - TestState1 > TestState2 > TestState3の順にスタックに積みながら状態を切り替えます
- 左キー
  - 状態を１つ戻します
- 1,2,3キー
  - 現在の状態から押したキーの状態に切り替え
- Q,W,Eキー
  - 現在の状態を中断して、対応する状態(Q=TestState1, W=TestState2, E=TestState3)に切り替える
- A,S,Dキー
  - 押したキーがスタックにあったら、その状態(A=TestState1, S=TestState2, D=TestState3)まで戻す
- Rキー
  - 最初の状態まで戻します
- 0キー
  - 全ての状態を戻して状態なしにします

切り替え中は、新しい切り替え要求は失敗させます。

左右キー操作は各状態から呼び出します。それ以外の状態変更は状態の管理スクリプトに実装します。

### 実装手順

各状態の雛形となるクラスを作成します。

1. 新しいC#スクリプトを作成して、名前を`TestState`にします
1. AM1StateBaseを継承した抽象クラスとして、以下のコードを実装します

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.State;

public abstract class TestState : AM1StateBase
{
    AM1StateBase next;

    protected int Counter { get; private set; }
    protected bool IsPausing { get; private set; }

    AM1StateStack stateStack;

    public TestState(AM1StateStack ins)
    {
        stateStack = ins;
    }

    /// <summary>
    /// 次の状態を設定。
    /// </summary>
    /// <param name="nx">次の状態</param>
    public void SetNext(AM1StateBase nx)
    {
        next = nx;
    }

    public override void Init()
    {
        base.Init();
        Counter = 0;
    }

    public override void Update()
    {
        base.Update();
        if (IsRunning)
        {
            Counter++;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (next != null)
                {
                    stateStack?.PushRequest(next);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                stateStack?.PopRequest();
            }
        }
    }

    public override void Pause()
    {
        base.Pause();
        IsPausing = true;
    }

    public override void Resume()
    {
        base.Resume();
        IsPausing = false;
    }

    public abstract void OnGUI();
}
```

状態が有効な時は IsRunning が true になり、その間はUpdate()ごとにカウントアップします。Update()では左右キーを監視して、右が押されたら設定された次の状態をプッシュで切り替えて、左が押されたら一つ前の状態に戻します。

他の状態にプッシュで移行している間が分かるように IsPausing フラグを実装しています。

状態が実行中の時は、カウンターを増やしながらGUIに表示します。ポーズ中はカウントを停止した状態で文字表示します。Popして状態を破棄したらGUIの表示を止めてカウンターをリセットします。

作成したTestStateを継承して、テスト用の状態を3つを作成します。

※今回は使い方を紹介するためのサンプルなので似たような状態なので親クラスを作りましたが、通常はそのままAM1StateBaseを継承して状態を作成します。

1. 新しいC#スクリプトを作成して、名前を`TestState1`にします
1. 先に作成した TestState 状態を継承した以下のスクリプトにします

```cs
using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState1 : TestState
{
    public TestState1(AM1StateStack ins) : base(ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.red;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 20, 400, 20), $"TestState1 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 20, 400, 20), $"TestState1 Pausing {Counter}");
        }
    }
}
```

- 同様に以下の`TestState2`, `TestState3`も作成します

```cs
using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState2 : TestState
{
    public TestState2(AM1StateStack ins) : base (ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.green;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 50, 400, 20), $"TestState2 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 50, 400, 20), $"TestState2 Pausing {Counter}");
        }
    }
}
```

```cs
using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestState3 : TestState
{
    public TestState3(AM1StateStack ins) : base(ins) { }

    public override void OnGUI()
    {
        GUI.color = Color.yellow;
        if (IsRunning)
        {
            GUI.Label(new Rect(20, 80, 400, 20), $"TestState3 Running {Counter}");
        }
        else if (IsPausing)
        {
            GUI.Label(new Rect(20, 80, 400, 20), $"TestState3 Pausing {Counter}");
        }
    }
}
```

以上で３つの状態ができました。これらの状態を管理するAM1StateStackを継承したクラスを作ります。

- 新しいC#スクリプトを作成して、名前を`TestStateStack`にします
- AM1StateStackクラスを継承した以下のようなクラスにします

```cs
using AM1.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStateStack : AM1StateStack
{
    TestState[] states = new TestState[3];

    private void Awake()
    {
        states[0] = new TestState1(this);
        states[1] = new TestState2(this);
        states[2] = new TestState3(this);
        states[0].SetNext(states[1]);
        states[1].SetNext(states[2]);
        states[2].SetNext(null);
    }



    protected override void Update()
    {
        base.Update();

        if ((CurrentState == null) && Input.GetKeyDown(KeyCode.RightArrow)) {
            this.PushRequest(states[0]);
        }

        // 1,2,3キー - 現在の状態から押したキーの状態に切り替え
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PopAndPushRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PopAndPushRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PopAndPushRequest(states[2]);
        }

        // Q,W,Eキー - 現在の状態を中断して、対応する状態(Q = TestState1, W = TestState2, E = TestState3)に切り替える
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PushRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            PushRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PushRequest(states[2]);
        }

        // A,S,Dキー - 押したキーがスタックにあったら、その状態(A = TestState1, S = TestState2, D = TestState3)まで戻す
        if (Input.GetKeyDown(KeyCode.A))
        {
            PopRequest(states[0]);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PopRequest(states[1]);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PopRequest(states[2]);
        }

        // Rキー - 最初の状態まで戻します
        if (Input.GetKeyDown(KeyCode.R))
        {
            PopToRootRequest();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 0キー - 全ての状態を戻して状態なしにします
            PopAllRequest();
        }
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(20, 200, 400, 20), "Left and Right Arrow Key to change state.");

        foreach(var state in states)
        {
            state.OnGUI();
        }
    }
}
```

Awake()で３つの状態のインスタンスを作成して、次の状態やAM1StateStackのインスタンスを渡して初期化しています。

Update()では、キー入力に応じて各種の状態切り替え要求を呼び出しています。このサンプルでは通常のRequest()メソッドを使っているので、キーを同時に押した場合、一方の状態切り替えは失敗して動作しません。切り替え要求を???QueueRequest()にすると、後からの要求はキューに積んで、前の切り替えが完了し次第、次の切り替えが続けて実行されるようにもできます。

[EOF]
