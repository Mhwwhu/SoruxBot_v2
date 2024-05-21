using SoruxBot.SDK.Model.Message.Entity;

namespace SoruxBot.SDK.Model.Message;

public class MessageBuilder(MessageChain messageChain)
{
    
    public static MessageBuilder GroupMessage(string platformId, string platformType) => new(new MessageChain(String.Empty, String.Empty, platformId, String.Empty, platformType));
    
    public static MessageBuilder PrivateMessage(string targetId, string platformType) => new(new MessageChain(String.Empty, targetId, String.Empty, String.Empty, platformType));
    
    public MessageBuilder AddTextMessage(string content)
    {
        messageChain.Messages.Add(new TextMessage("text", content));
        return this;
    }
    
    public MessageBuilder AddCommonMessage(string type, string content)
    {
        messageChain.Messages.Add(new CommonMessage(type, content));
        return this;
    }
    
    public virtual MessageChain Build() { return messageChain; }
}