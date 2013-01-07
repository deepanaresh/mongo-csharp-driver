
namespace MongoDB.Driver.Communication.Security
{
    /// <summary>
    /// A step in a Sasl Conversation.
    /// </summary>
    internal interface ISaslStep
    {
        /// <summary>
        /// The bytes that should be sent to ther server before calling Transition.
        /// </summary>
        byte[] BytesToSendToServer { get; }

        /// <summary>
        /// Transitions to the next step in the conversation.
        /// </summary>
        /// <param name="conversation">The conversation.</param>
        /// <param name="bytesReceivedFromServer">The bytes received from the server.</param>
        /// <returns>An ISaslStep.</returns>
        ISaslStep Transition(SaslConversation conversation, byte[] bytesReceivedFromServer);
    }
}