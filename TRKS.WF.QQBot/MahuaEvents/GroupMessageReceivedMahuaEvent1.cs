﻿using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Humanizer;
using Settings;
using System.Text.RegularExpressions;

namespace TRKS.WF.QQBot.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceivedMahuaEvent1
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;
        internal static readonly WFNotificationHandler _WfNotificationHandler = new WFNotificationHandler();
        private static readonly WFStatus _WFStatus = new WFStatus();
        private static readonly WMSearcher _wmSearcher = new WMSearcher();
        private static readonly RMSearcher _rmSearcher = new RMSearcher();

        public GroupMessageReceivedMahuaEvent1(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            if (Messenger.GroupCallDic.ContainsKey(context.FromGroup))
            {
                if (Messenger.GroupCallDic[context.FromGroup] > Config.Instance.CallperMinute && Config.Instance.CallperMinute != 0) return;
            }
            else
            {
                Messenger.GroupCallDic[context.FromGroup] = 0;
            }                             

            if (HotUpdateInfo.PreviousVersion) return;

            try
            {
                var message = context.Message;
                if (message.StartsWith("/") || !Config.Instance.IsSlashRequired)
                {
                    var command = "";
                    if (message.Contains("/"))
                    {
                        command = message.Substring(1).ToLower();
                    }
                    else
                    {
                        command = message;
                    }

                    var fortuna = new[] {"金星赏金", "金星平原赏金", "福尔图娜赏金", "奥布山谷赏金"};
                    var ostron = new[] {"地球赏金", "地球平原赏金", "希图斯赏金"};
                    var fissures = new [] {"裂隙", "裂缝", "虚空裂隙", "查询裂缝", "查询裂隙"};
                    if (ostron.Any(word => command.StartsWith(word)))
                    {
                        var index = 0;
                        if (command.Length > ostron.First(syn => command.StartsWith(syn)).Length + 1)
                        {
                            var indexString = command.Substring(ostron.First(syn => command.StartsWith(syn)).Length);
                            if (indexString.IsNumber())
                            {
                                index = int.Parse(indexString);
                            }
                        }
                        _WFStatus.SendCetusMissions(context.FromGroup, index);
                    }
                    if (fortuna.Any(word => command.StartsWith(word)))
                    {
                        var index = 0;
                        if (command.Length > fortuna.First(syn => command.StartsWith(syn)).Length + 1)
                        {
                            var indexString = command.Substring(fortuna.First(syn => command.StartsWith(syn)).Length);
                            if (indexString.IsNumber())
                            {
                                index = int.Parse(indexString);
                            }
                        }
                        _WFStatus.SendFortunaMissions(context.FromGroup, index);
                    }

                    if (fissures.Any(fissure => command.StartsWith(fissure)))
                    {
                        Messenger.SendGroup(context.FromGroup, "裂隙查询已经改版，请直接使用 /裂隙.");
                        /*var words = command.Split(' ').ToList();
                        if (words.Count >= 2)
                        {
                            words.RemoveAt(0);
                            _wFStatus.SendFissures(context.FromGroup, words);
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, $"需要参数, e.g. /裂隙 古纪");
                        }*/

                    }
                    if (command.StartsWith("查询"))
                    {
                        if (!command.Contains("裂隙") || !command.Contains("裂缝"))
                        {
                            if (command.Length > 3)
                            {
                                if (command.Substring(2).StartsWith(" "))
                                {
                                    var item = command.Substring(3).Format();
                                    _wmSearcher.SendWMInfo(item, context.FromGroup);
                                }
                            }
                            else
                            {
                                Messenger.SendGroup(context.FromGroup, "你没输入要查询的物品.");
                            }

                        }

                    }

                    if (command.StartsWith("紫卡"))
                    {
                        if (command.Length >= 3)
                        {
                            if (command.Substring(2).StartsWith(" "))
                            {
                                var weapon = command.Substring(3).Replace("&amp;", "&").Format();
                                _rmSearcher.SendRiveninfos(context.FromGroup, weapon);
                            }

                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "你没输入要查询的武器.");
                        }

                        
                    }
                    if (command.StartsWith("翻译"))
                    {
                        if (command.Length >= 3)
                        {
                            if (command.Substring(2).StartsWith(" "))
                            {
                                var word = command.Substring(3).Format();
                                _WFStatus.SendTranslateResult(context.FromGroup, word);
                            }
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "缺少源名.");
                        }
                    }
                    if (command.StartsWith("遗物"))
                    {
                        if (command.Length >= 3)
                        {
                            if (command.Substring(2).StartsWith(" "))
                            {
                                var word = command.Substring(3).Format();
                                _WFStatus.SendRelicInfo(context.FromGroup, word);
                            }
                        }
                        else
                        {
                            Messenger.SendGroup(context.FromGroup, "请在后面输入关键词.");
                        }
                    }
                    switch (command)
                    {
                        case "警报":
                            _WfNotificationHandler.SendAllAlerts(context.FromGroup);
                            break;
                        case "平野":
                        case "夜灵平野":
                        case "平原":
                        case "夜灵平原":
                        case "金星平原":
                        case "奥布山谷":
                        case "金星平原温度":
                        case "平原温度":
                        case "平原时间":
                            _WFStatus.SendCycles(context.FromGroup);
                            break;
                        case "入侵":
                            _WfNotificationHandler.SendAllInvasions(context.FromGroup);
                            break;
                        case "突击":
                            _WFStatus.SendSortie(context.FromGroup);
                            break;
                        case "奸商":
                        case "虚空商人":
                        case "商人":
                            _WFStatus.SendVoidTrader(context.FromGroup);
                            break;
                        case "活动":
                        case "事件":
                            _WFStatus.SendEvent(context.FromGroup);
                            break;
                        case "裂隙":
                        case "裂缝":
                            _WFStatus.SendFissures(context.FromGroup);
                            break;
                        case "小小黑":
                        case "追随者":
                        case "焦虑":
                        case "怨恨":
                        case "躁狂":
                        case "苦难":
                        case "折磨":
                        case "暴力":
                            _WfNotificationHandler.SendAllPersistentEnemies(context.FromGroup);
                            break;
                        case "help":
                        case "帮助":
                        case "功能":
                        case "救命":
                            Messenger.SendHelpdoc(context.FromGroup);
                            break;
                    }
                }


            }
            catch (Exception e)
            {
                Messenger.SendDebugInfo(e.ToString());
            }

        }
    }
}
