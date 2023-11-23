using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomMacroPlugin0.Tools.TimeManager
{
    partial class CooldownTimer
    {
        /// <summary>
        /// 内部计时器类
        /// </summary>
        public sealed class InnerTimer
        {
            DateTime? starttime, starttime_lp, endtime;
            int? threshold = null;
            bool flag = false;
            bool is_long_press => ((DateTime)endtime!).Subtract((DateTime)starttime!).TotalMilliseconds > threshold;
            bool is_long_press_time_out => ((DateTime)starttime_lp!).Subtract((DateTime)endtime!).TotalMilliseconds > 60;

            /// <summary>
            /// <para>_threshold：<see cref="int"/>类型，超时阈值（比如填'100'，则当持续访问该方法时，于100毫秒内返回false，于100毫秒后返回true）</para>
            /// </summary>
            public bool Elapsed(int _threshold)
            {
                if (starttime is null)//长按期间该块仅执行一次
                {
                    if (threshold != _threshold) { threshold = _threshold; }
                    if (threshold == 0) { return true; }

                    starttime = starttime_lp = endtime = DateTime.Now;
                    ((Func<Task>)(async () =>
                    {
                        while (true)
                        {
                            await Task.Delay(50).ConfigureAwait(false); //每50毫秒检测一次

                            starttime_lp = DateTime.Now;
                            if (is_long_press_time_out is false)
                            {
                                //起点时间<终点时间，未超时，说明此刻按键尚处于按下状态
                                if (flag is false) { flag = is_long_press; }
                            }
                            else
                            {
                                //起点时间>终点时间，已超时，说明此刻按键处于弹起状态
                                starttime = starttime_lp = endtime = null;
                                flag = false;
                                break;
                            }
                        }
                    }))();
                }
                else//长按期间更新endtime
                {
                    endtime = DateTime.Now;
                }
                return flag;
            }
        }
    }

    partial class CooldownTimer
    {
        /// <summary>
        /// 内部字典
        /// </summary>
        private readonly Dictionary<string, InnerTimer> internalList = new();

        /// <summary>
        /// 通过key访问内部计时器
        /// </summary>
        public InnerTimer this[string key]
        {
            get
            {
                if (internalList.ContainsKey(key) is false)
                {
                    internalList.Add(key, new());
                }
                return internalList[key];
            }
        }
    }
}
