using System.Diagnostics;

namespace Canves {
    /// <summary>
    /// 类似 Unity 的 Time 类，提供 deltaTime 等时间信息。
    /// </summary>
    static class Time {
        private static Stopwatch _stopwatch = new Stopwatch();
        private static long _lastTicks = 0;

        /// <summary>
        /// 上一帧 Update 的时间间隔（秒）。第一帧为 0。
        /// </summary>
        public static float deltaTime { get; private set; } = 0f;

        /// <summary>
        /// 从 Start 开始经过的总时间（秒）。
        /// </summary>
        public static float time => (float)_stopwatch.Elapsed.TotalSeconds;

        /// <summary>
        /// 在游戏启动时调用一次，开始计时。
        /// </summary>
        public static void Start() {
            _stopwatch.Restart();
            _lastTicks = _stopwatch.ElapsedTicks;
            deltaTime = 0f;
        }

        /// <summary>
        /// 每帧 Update 开头调用，刷新 deltaTime。
        /// </summary>
        public static void Tick() {
            long currentTicks = _stopwatch.ElapsedTicks;
            deltaTime = (float)(currentTicks - _lastTicks) / Stopwatch.Frequency;
            _lastTicks = currentTicks;
        }
    }
}
