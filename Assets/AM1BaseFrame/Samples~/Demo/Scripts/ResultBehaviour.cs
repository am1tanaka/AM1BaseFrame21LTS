using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AM1.BaseFrame.Assets;

namespace AM1.BaseFrame.Demo
{
    public class ResultBehaviour : MonoBehaviour, ISceneBehaviour
    {
        public static ResultBehaviour Instance { get; private set; }

        [Tooltip("0に成功、1にミスのアニメを設定"), SerializeField]
        Animator[] resultAnimators = new Animator[2];

        static float TransitionSeconds => 1f;

        /// <summary>
        /// シーンが開始したらtrueを設定
        /// </summary>
        public bool IsStarted { get; private set; }

        private void Awake()
        {
            Instance = this;
            StateChanger.AwakeDone(gameObject.scene.name);
        }

        void Update()
        {
            if (IsStarted && (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")))
            {
                IsStarted = false;
                SEPlayer.Play(SEPlayer.SE.Click);
                ToTitle();
            }
        }

        public void StartAnim(ResultStateChanger.ResultType type)
        {
            resultAnimators[(int)type].SetBool("Show", true);

        }

        /// <summary>
        /// アニメ完了を報告
        /// </summary>
        public void AnimDone()
        {
            IsStarted = true;
        }

        /// <summary>
        /// タイトルヘ切り替え
        /// </summary>
        public void ToTitle()
        {
            if (ResultStateChanger.CurrentResultType == ResultStateChanger.ResultType.Miss)
            {
                ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.Fade, TransitionSeconds);
            }
            else
            {
                ScreenTransitionRegistry.StartCover((int)ScreenTransitionType.FilledRadial, TransitionSeconds);
            }

            TitleStateChanger.Instance.Request();
        }
    }
}