using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.Utils
{
    /// <summary>
    /// Instanceにアクセスするとシングルトンを返すシンプルなジェネリッククラス。
    /// Programmed by Yu Tanaka @am1tanaka
    /// CC0 License
    /// </summary>
    public class SimpleSingleton<T> where T : new()
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}