using System;
using System.Threading.Tasks;

namespace WindNight.Core.Tools
{
    public static class RetryFunc
    {
        /// <summary>
        ///     ReTry to exec the Snippet code
        /// </summary>
        /// <typeparam name="T">the type of returns <see cref="T" /></typeparam>
        /// <param name="func">the main func to be execed</param>
        /// <param name="tryCount">the maximum of the main func can be re-execed </param>
        /// <param name="delayMs">the delayMs milliseconds wait re-exec the main func</param>
        /// <param name="errorResult">
        ///     the result which you sure must be a error result,then do <paramref name="warnAction" />,when
        ///     the <paramref name="warnAction" /> is  not null
        /// </param>
        /// <param name="rollBackAction">
        ///     when the main func catch's count is over <paramref name="tryCount" />,this action to do
        ///     rollback Job
        /// </param>
        /// <param name="warnAction">the action to do warn job</param>
        /// <param name="defaultValue"> </param>
        /// <returns></returns>
        public static T RetrySnippetFunc<T>(
            Func<T> func, int tryCount = 1, int delayMs = 1000, T errorResult = default,
            Action rollBackAction = null, Action<Exception> warnAction = null, T defaultValue = default(T)
        )
        {
            tryCount = tryCount < 1 ? 1 : tryCount;
            var execCount = 0;
            var rlt = defaultValue;
            while (tryCount > execCount - 1)
            {
                #region try

                try
                {
                    execCount++;
                    rlt = func.Invoke();
                    break;
                }
                catch (Exception ex)
                {
                    #region do catch

                    if (tryCount == execCount - 1)
                    {
                        //  告警策略实现
                        warnAction?.KeepSafeAction(ex);
                        //  回滚策略实现
                        rollBackAction?.KeepSafeAction();
                        break;
                    }

                    if (delayMs > 0) Task.Delay(delayMs).Wait();

                    #endregion //end do catch
                    rlt = defaultValue;
                }

                #endregion //end try
            }

            if (Equals(rlt, errorResult)) warnAction?.KeepSafeAction();

            return rlt;
        }


        /// <summary>
        ///     ReTry to exec the Snippet code which does not have return value.
        /// </summary>
        /// <param name="action">the main action to be execed</param>
        /// <param name="tryCount">the maximum of the main func can be re-execed </param>
        /// <param name="delayMs">the delayMs milliseconds wait re-exec the main func</param>
        /// <param name="rollBackAction">
        ///     when the main func catch's count is over <paramref name="tryCount" />,this action to do
        ///     rollback Job
        /// </param>
        /// <param name="warnAction">
        ///     the action to do warn job,the <see cref="Exception" /> where be catched when do the main
        ///     action and return to <paramref name="warnAction" />. this <see cref="Exception" /> can not be deel with throw
        ///     direct.
        /// </param>
        public static void RetrySnippetFunc(
            Action action, int tryCount = 1, int delayMs = 1000, Action rollBackAction = null,
            Action<Exception> warnAction = null)
        {
            tryCount = tryCount < 1 ? 1 : tryCount;
            var execCount = 0;
            while (tryCount > execCount - 1)
            {
                #region try

                try
                {
                    execCount++;
                    action.Invoke();
                    break;
                }
                catch (Exception ex)
                {
                    #region do catch

                    if (tryCount == execCount - 1)
                    {
                        //TODO 告警策略实现
                        warnAction?.KeepSafeAction(ex);
                        //TODO 回滚策略实现
                        rollBackAction?.KeepSafeAction();
                        break;
                    }

                    if (delayMs > 0) Task.Delay(delayMs).Wait();

                    #endregion //end do catch
                }

                #endregion //end try
            }
        }

        public static void KeepSafeAction(this Action action)
        {
            try
            {
                action.Invoke();
            }
            catch
            {
                // ignored
            }
        }

        public static void KeepSafeAction(this Action<Exception> action, Exception ex = null)
        {
            try
            {
                action.Invoke(ex);
            }
            catch
            {
                // ignored
            }
        }
    }
}