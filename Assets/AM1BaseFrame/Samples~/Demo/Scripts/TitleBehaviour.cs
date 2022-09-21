using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame.Assets;

namespace AM1.BaseFrame.Demo
{
    public class TitleBehaviour : MonoBehaviour, ISceneBehaviour
    {
        public static TitleBehaviour Instance { get; private set; }

        static float TransitionSeconds => 0.5f;

        /// <summary>
        /// シーンが開始したらtrueを設定
        /// </summary>
        public bool IsStarted { get; private set; }

        public enum State
        {
            WaitStart,
            GameStart,
        }

        public State CurrentState { get; private set; }

        private void Awake()
        {
            if (!StateChanger.IsReady) return;

            Instance = this;
            StateChanger.AwakeDone(gameObject.scene.name);
        }

        void Start()
        {
            if (!StateChanger.IsReady) return;

            IsStarted = true;
        }

        void Update()
        {
            if (!Input.GetMouseButtonDown(0)
                && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")))
            {
                OnStartButtonClicked();
            }
        }

        /// <summary>
        /// スタートボタンが押された時の処理
        /// </summary>
        public void OnStartButtonClicked()
        {
            if (StateChanger.IsRequestOrChanging) return;

            switch (CurrentState)
            {
                case State.WaitStart:
                    if (GameStateChanger.Instance.Request())
                    {
                        SEPlayer.Play(SEPlayer.SE.Start);
                        ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial, TransitionSeconds);
                        CurrentState++;
                    }
                    break;
            }
        }

    }
}