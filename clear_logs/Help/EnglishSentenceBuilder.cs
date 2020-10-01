namespace ClearLogs.Help
{
    /// <summary>
    ///     Models an english sentence builder, currently the default one.
    /// </summary>
    public class EnglishSentenceBuilder : BaseSentenceBuilder
    {
        /// <summary>
        ///     Gets a string containing word 'option' in english.
        /// </summary>
        /// <value>
        ///     The word 'option' in english.
        /// </value>
        public override string OptionWord => "option";

        /// <summary>
        ///     Gets a string containing the word 'and' in english.
        /// </summary>
        /// <value>
        ///     The word 'and' in english.
        /// </value>
        public override string AndWord => "and";

        /// <summary>
        ///     Gets a string containing the sentence 'required option missing' in english.
        /// </summary>
        /// <value>
        ///     The sentence 'required option missing' in english.
        /// </value>
        public override string RequiredOptionMissingText => "required option is missing";

        /// <summary>
        ///     Gets a string containing the sentence 'violates format' in english.
        /// </summary>
        /// <value>
        ///     The sentence 'violates format' in english.
        /// </value>
        public override string ViolatesFormatText => "violates format";

        /// <summary>
        ///     Gets a string containing the sentence 'violates mutual exclusiveness' in english.
        /// </summary>
        /// <value>
        ///     The sentence 'violates mutual exclusiveness' in english.
        /// </value>
        public override string ViolatesMutualExclusivenessText => "violates mutual exclusiveness";

        /// <summary>
        ///     Gets a string containing the error heading text in english.
        /// </summary>
        /// <value>
        ///     The error heading text in english.
        /// </value>
        public override string ErrorsHeadingText => "ERROR(S):";
    }
}