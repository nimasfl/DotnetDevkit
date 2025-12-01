using FluentValidation;

namespace DotnetDevkit.Result.Extensions;

public static partial class ResultExtensions
{
    /// <param name="rule"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        /// <summary>
        /// Specifies the error code if the rule conditions are not met.
        /// </summary>
        /// <param name="enum"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public IRuleBuilderOptions<T, TProperty> WithErrorCode<TEnum>(TEnum @enum) where TEnum : Enum =>
            rule.WithErrorCode(@enum.GetHashCode().ToString());

        /// <summary>
        /// Specifies the error code if the rule conditions are not met.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public IRuleBuilderOptions<T, TProperty> WithErrorCode(int errorCode) =>
            rule.WithErrorCode(errorCode.ToString());

        /// <summary>
        /// Specifies the error arguments if the rule conditions are not met.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IRuleBuilderOptions<T, TProperty> WithArgs(params string[] args) => rule.WithState(_ => args);
    }
}
