using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AM1.BaseFrame
{
    /// <summary>
    /// 画面切り替え処理インスタンスを登録するクラス
    /// </summary>
    public static class ScreenTransitionRegistry
    {
        readonly static Dictionary<int, IScreenTransition> screenTransitionDictionary = new Dictionary<int, IScreenTransition>();

        /// <summary>
        /// 変更中を表します
        /// </summary>
        public static bool IsTransitioning()
        {
            foreach (var tr in ScreenTransitionRegistry.screenTransitionDictionary)
            {
                if ((tr.Value != null) && tr.Value.IsTransitioning)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定の種類の画面切り替えを開始する。
        /// </summary>
        /// <param name="type">切り替える種類</param>
        /// <param name="time">切り替え時間。省略か0で即時</param>
        /// <returns>開始成功時true。すでになにか切り替え中ならfalseで遷移キャンセル</returns>
        public static bool StartCover(int type, float time = 0)
        {
            if (IsTransitioning())
            {
                return false;
            }

            GetInstance(type).StartCover(true, time);
            return true;
        }

        /// <summary>
        /// 色つきフェード開始
        /// </summary>
        /// <param name="type">切り替える種類</param>
        /// <param name="color">覆う色</param>
        /// <param name="time">切り替え時間。省略か0で即時</param>
        /// <returns>開始成功時true。すでになにか切り替え中ならfalseで遷移キャンセル</returns>
        public static bool StartCover(int type, Color color, float time = 0)
        {
            if (IsTransitioning())
            {
                return false;
            }

            GetInstance(type).StartCover(true, color, time);
            return true;
        }

        /// <summary>
        /// 画面を覆っているものを解除する。
        /// </summary>
        /// <param name="time">切り替え時間。省略か0で即時</param>
        public static void StartUncover(float time = 0)
        {
            foreach (var tr in ScreenTransitionRegistry.screenTransitionDictionary)
            {
                // 切り替え中か覆われているなら解除
                if ((tr.Value != null) && (tr.Value.IsTransitioning || tr.Value.IsCover))
                {
                    tr.Value.StartCover(false, time);
                }
            }
        }

        /// <summary>
        /// 指定のIDのインスタンスを返す。
        /// 無効なIDを指定するとnullを返す。
        /// </summary>
        /// <param name="type">取得したい画面切り替えのタイプ</param>
        /// <returns>インスタンス。無効時はnull</returns>
        public static IScreenTransition GetInstance(int type)
        {
            if (!screenTransitionDictionary.ContainsKey(type))
            {
                return null;
            }

            return screenTransitionDictionary[type];
        }

        /// <summary>
        /// 指定の種類の画面切り替え処理のインスタンスを登録。
        /// 登録済みのIDを指定すると新しいインスタンスで上書きする。
        /// </summary>
        /// <param name="type">画面切り替えの種類</param>
        /// <param name="ins">インスタンス</param>
        public static void Register(int type, IScreenTransition ins)
        {
            if (screenTransitionDictionary.ContainsKey(type))
            {
                screenTransitionDictionary[type] = ins;
            }
            else
            {
                screenTransitionDictionary.Add(type, ins);
            }
        }

        public static void Unregister(int type)
        {
            if (screenTransitionDictionary.ContainsKey(type))
            {
                screenTransitionDictionary.Remove(type);
            }
        }

        /// <summary>
        /// 登録されている全ての画面切り替えが完了するのを待つ。
        /// </summary>
        public static IEnumerator WaitAll()
        {
            IScreenTransition[] sctr = new IScreenTransition[screenTransitionDictionary.Count];
            int i = 0;
            foreach (var ins in screenTransitionDictionary)
            {
                sctr[i] = ins.Value;
                i++;
            }

            for (i = 0; i < sctr.Length; i++)
            {
                if (sctr[i] != null)
                {
                    yield return sctr[i].Wait();
                }
            }
        }
    }
}