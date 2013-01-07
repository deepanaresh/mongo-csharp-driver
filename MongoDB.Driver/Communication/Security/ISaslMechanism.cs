
namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// A mechanism used to converse with the server.
    /// </summary>
    internal interface ISaslMechanism
    {
        /// <summary>
        /// Transitions to the next step in the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="input">The input.</param>
        /// <returns>An ISaslStep.</returns>
        ISaslStep Transition(SaslConversation conversation, byte[] input);
    }
}