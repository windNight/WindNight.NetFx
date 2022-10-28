namespace WindNight.Core.Abstractions
{
    /// <summary>
    ///     Defines logging severity levels.
    ///     copy from <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> for internal use
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        ///     Logs that contain the most detailed messages. These messages may contain sensitive
        ///     application data. These messages are disabled by default and should never be
        ///     enabled in a production environment.
        /// </summary>
        Trace = 0,

        /// <summary>
        ///     Logs that are used for interactive investigation during development. These logs
        ///     should primarily contain information useful for debugging and have no long-term
        ///     value.
        /// </summary>
        Debug = 1,

        /// <summary>
        ///     Logs that track the general flow of the application. These logs should have long-term  value.
        /// </summary>
        Information = 2,

        /// <summary>
        ///     Logs that highlight an abnormal or unexpected event in the application flow,
        ///     but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        ///     Logs that highlight when the current flow of execution is stopped due to a failure.
        ///     These should indicate a failure in the current activity, not an application-wide
        ///     failure.
        /// </summary>
        Error = 4,

        /// <summary>
        ///     Logs that describe an unrecoverable application or system crash, or a catastrophic
        ///     failure that requires immediate attention.
        /// </summary>
        Critical = 5,

        /// <summary>
        ///     Not used for writing log messages. Specifies that a logging category should not
        ///     write any messages.
        /// </summary>
        None = 6,

        /// <summary>  </summary>
        ApiUrl = 10,

        /// <summary>  </summary>
        ApiUrlException = 20,

        /// <summary>  </summary>
        Report = 30,

        /// <summary>  </summary>
        SysRegister = 40,

        /// <summary>  </summary>
        SysOffline = 41
    }


}