﻿using SoruxBot.SDK.Model.Message;
using SoruxBot.SDK.Plugins.Model;

namespace SoruxBot.Kernel.Services.PluginService.Model;

public class PluginsListenerDescriptor(
    MessageType messageType,
    string targetPlatformType,
    string targetAction,
    Func<MessageContext, bool> conditionCheck,
    bool isIntercept = true,
    Func<MessageContext, PluginFlag>? successfulFunc = null,
    Func<MessageContext, PluginFlag>? failureFunc = null)
{
    /// <summary>
    /// 监听事件的类型
    /// </summary>
    public MessageType MessageType { get; init; } = messageType;

    /// <summary>
    /// 监听事件的平台
    /// </summary>
    public string TargetPlatformType { get; init; } = targetPlatformType;

    /// <summary>
    /// 监听事件的方法
    /// </summary>
    public string TargetAction { get; init; } = targetAction;

    /// <summary>
    /// 监听事件的条件
    /// </summary>
    public Func<MessageContext, bool> ConditionCheck { get; init; } = conditionCheck;

    /// <summary>
    /// 是否禁止消息继续传递，表示禁止其他监听器继续监听消息
    /// </summary>
    public bool IsIntercept { get; init; } = isIntercept;

    /// <summary>
    /// 如果成功捕获到了，那么调用这个函数
    /// </summary>
    public Func<MessageContext, PluginFlag>? SuccessfulFunc { get; init; } = successfulFunc;

    /// <summary>
    /// 如果失败了，那么调用这个函数，此函数是可以被配置的
    /// </summary>
    public Func<MessageContext, PluginFlag>? FailureFunc { get; init; } = failureFunc;
}