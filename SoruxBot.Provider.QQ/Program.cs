﻿using System.Security.Cryptography;
using System.Text.Json;
using Grpc.Core;
using Grpc.Net.Client;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Message.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using SoruxBot.Provider.QQ;
using SoruxBot.Provider.QQ.Service;
using SoruxBot.Provider.WebGrpc;
using SoruxBot.SDK.Model.Message;
using SoruxBot.SDK.Model.Message.Entity;

BotDeviceInfo deviceInfo = new()
{
    Guid = Guid.NewGuid(),
    MacAddress = GenRandomBytes(6),
    DeviceName = $"SoruxBot-QQ",
    SystemKernel = "Windows 10.0.19042",
    KernelVersion = "10.0.19042.0"
};

BotKeystore keystore = new BotKeystore();

BotConfig config = new BotConfig
{
    UseIPv6Network = false,
    GetOptimumServer = true,
    AutoReconnect = true,
    Protocol = Protocols.Linux,
    CustomSignProvider = new QqSigner(),
};

var bot = BotFactory.Create(config, deviceInfo, keystore);
var botClient = new BotClient(config, bot);

// 构建 Configuration
var configuration = new ConfigurationBuilder()
    .AddYamlFile("config.yaml", optional: false, reloadOnChange: true)
    .Build();


// 构建 gRpc 服务端
BuildGrpcServer(configuration, bot).Start();
Console.WriteLine("[SoruxBot.Provider.QQ] gRpc Server is listening at " +
                  $"{configuration.GetSection("server:host").Value}:{configuration.GetSection("server:port").Value}");

// 构建 gRpc 客户端
using var channel =
    GrpcChannel.ForAddress(
        $"http://{configuration.GetSection("client:host").Value}:{configuration.GetSection("client:port").Value}");
var client = new Message.MessageClient(channel);

// 注册回调事件

const string platformType = "QQ";

// 当好友消息发送的时候
bot.Invoker.OnFriendMessageReceived += (context, @event) =>
{
    Console.WriteLine($"[SoruxBot.Provider.QQ] Friend Message Received: {@event.Chain.ToPreviewString()}");
    var msgChain = new MessageChain(
        context.BotUin.ToString(),
        @event.Chain.FriendUin.ToString(),
        @event.Chain.FriendUin.ToString(), // 私聊时，这个值等于 TriggerId
        @event.Chain.FriendUin.ToString(), // 私聊时，这个值等于 TriggerId
        platformType
    );

    // 转换消息链
    ConvertMessageChain(msgChain, @event.Chain);

    var msg = new MessageContext(
        context.BotUin.ToString(),
        "FriendMessage",
        platformType,
        MessageType.PrivateMessage,
        @event.Chain.FriendUin.ToString(),
        @event.Chain.FriendUin.ToString(), // 私聊时，这个值等于 TriggerId
        @event.Chain.FriendUin.ToString(), // 私聊时，这个值等于 TriggerId
        msgChain,
        @event.EventTime
    );

    client.MessagePushStack(new MessageRequest()
    {
        Payload = JsonSerializer.Serialize(msg),
        Token = configuration.GetSection("client:token").Value
    });
};

// 登录
await botClient.LoginAsync();

// 保持程序运行
await Task.Delay(-1);


// 方法
static byte[] GenRandomBytes(int length)
{
    byte[] randomBytes = new byte[length];
    using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);

    return randomBytes;
}

static Server BuildGrpcServer(IConfiguration config, BotContext bot)
    => new Server
    {
        Services =
        {
            Message.BindService(
                new MessageService(
                    config.GetSection("server:token").Value,
                    bot
                ))
        },

        Ports =
        {
            new ServerPort(
                config.GetSection("server:host").Value,
                int.Parse(config.GetSection("server:port").Value!),
                ServerCredentials.Insecure)
        }
    };

static void ConvertMessageChain(MessageChain chain, Lagrange.Core.Message.MessageChain eventChain)
{
    foreach (var entity in eventChain)
    {
        // 转换文字消息
        if (entity is TextEntity textEntity)
        {
            chain.Messages.Add(new TextMessage(
                textEntity.Text
            ));
            continue;
        }
    }
}