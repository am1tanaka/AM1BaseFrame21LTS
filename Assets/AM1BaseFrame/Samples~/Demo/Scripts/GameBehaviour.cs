using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame.Assets;

namespace AM1.BaseFrame.Demo
{
    public class GameBehaviour : MonoBehaviour, ISceneBehaviour
    {
        public static GameBehaviour Instance { get; private set; }

        [Tooltip("開始テキストアニメ"), SerializeField]
        Animator startTextAnimator = default;

        /// <summary>
        /// ゲーム内状態
        /// </summary>
        public enum State
        {
            WaitStateStart,
            WaitStart,
            Game,
            Result,
        }

        /// <summary>
        /// 現在のゲーム内状態
        /// </summary>
        public static State CurrentState { get; private set; }

        /// <summary>
        /// シーンが開始したらtrueを設定
        /// </summary>
        public bool IsStarted { get; private set; }


        private void Awake()
        {
            if (!SceneStateChanger.IsReady) return;

            Instance = this;
            SceneStateChanger.AwakeDone(gameObject.scene.name);
        }

        void Start()
        {
            if (!SceneStateChanger.IsReady) return;

            IsStarted = true;
        }

        void Update()
        {
            if (CurrentState == State.WaitStart)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    OnClickStartButton();
                }
            }
        }

        /// <summary>
        /// 状態が開始した時に呼び出して、開始文字のアニメ開始
        /// </summary>
        public void StartState()
        {
            startTextAnimator.SetBool("Show", true);
            CurrentState = State.WaitStart;
            BGMPlayer.Play(BGMPlayer.BGM.Game);
        }

        /// <summary>
        /// スタート操作をした時に呼び出します。
        /// </summary>
        public void OnClickStartButton()
        {
            if (CurrentState == State.WaitStart)
            {
                CurrentState = State.Game;
                startTextAnimator.SetBool("Show", false);
                SEPlayer.Play(SEPlayer.SE.Start);

                // プレイヤー行動開始
                DemoPlayer.Instance.GameStart();
            }
        }

        public void ToGameover()
        {
            ResultStateChanger.Instance.Request(ResultStateChanger.ResultType.Miss);
        }

        public void ToClear()
        {
            ResultStateChanger.Instance.Request(ResultStateChanger.ResultType.Success);
        }
    }
}