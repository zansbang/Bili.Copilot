﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DanmakuViewModel"/> class.
    /// </summary>
    public DanmakuViewModel(
        IDanmakuService service,
        ILogger<DanmakuViewModel> logger)
    {
        _danmakuService = service;
        _logger = logger;
        Locations = [DanmakuLocation.Scroll, DanmakuLocation.Top, DanmakuLocation.Bottom];
        Colors = [Microsoft.UI.Colors.White, Microsoft.UI.Colors.Red, Microsoft.UI.Colors.Orange, Microsoft.UI.Colors.Khaki, Microsoft.UI.Colors.Yellow, Microsoft.UI.Colors.GreenYellow, Microsoft.UI.Colors.Green, Microsoft.UI.Colors.Blue, Microsoft.UI.Colors.Purple, Microsoft.UI.Colors.LightBlue];
    }

    /// <summary>
    /// 重置数据.
    /// </summary>
    public void ResetData(string aid, string cid)
    {
        ClearAll();
        _aid = aid;
        _cid = cid;
        ResetData();
    }

    /// <summary>
    /// 重置数据.
    /// </summary>
    public void ResetData()
    {
        ReloadFontsCommand.Execute(default);
        ResetOptions();
    }

    /// <summary>
    /// 更新进度.
    /// </summary>
    public void UpdatePosition(int position)
    {
        _position = position;
        ProgressChanged?.Invoke(this, position);
    }

    /// <summary>
    /// 暂停.
    /// </summary>
    public void Pause()
    {
        IsPaused = true;
        PauseDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 恢复.
    /// </summary>
    public void Resume()
    {
        IsPaused = false;
        ResumeDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 重新绘制.
    /// </summary>
    public void Redraw()
    {
        RequestRedrawDanmaku?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void ClearAll()
    {
        _duration = 0;
        _aid = string.Empty;
        _cid = string.Empty;
        ClearDanmaku();
    }

    /// <summary>
    /// 清理弹幕.
    /// </summary>
    public void ClearDanmaku()
        => RequestClearDanmaku?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 重置样式.
    /// </summary>
    public void ResetStyle()
        => RequestResetStyle?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// 是否为空.
    /// </summary>
    /// <returns>结果.</returns>
    public bool IsEmpty()
        => string.IsNullOrEmpty(_aid) || string.IsNullOrEmpty(_cid);

    [RelayCommand]
    private async Task LoadDanmakusAsync(int duration)
    {
        if (IsLoading || string.IsNullOrEmpty(_aid) || string.IsNullOrEmpty(_cid) || _duration == duration)
        {
            return;
        }

        IsLoading = true;
        _duration = duration;
        var count = Convert.ToInt32(Math.Ceiling(_duration / 360d));
        if (count == 0)
        {
            count = 1;
        }

        var totalDanmakus = new List<DanmakuInformation>();
        for (var i = 0; i < count; i++)
        {
            try
            {
                var danmakus = await _danmakuService.GetSegmentDanmakusAsync(_aid, _cid, i + 1);
                totalDanmakus.AddRange(danmakus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"加载 {_aid} | {_cid} 的弹幕失败，索引为 {i}");
                break;
            }
        }

        if (totalDanmakus.Count > 0)
        {
            CanShowDanmaku = true;
            ListAdded?.Invoke(this, totalDanmakus);
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task SendDanmakuAsync(string text)
    {
        try
        {
            var danmakuColor = (Color.R * 256 * 256) + (Color.G * 256) + Color.B;
            await _danmakuService.SendVideoDanmakuAsync(text, _aid, _cid, _position, danmakuColor.ToString(), IsStandardSize, Location);
            RequestAddSingleDanmaku?.Invoke(this, text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "弹幕发送失败.");
        }
    }

    [RelayCommand]
    private void AddDanmaku(string text)
    {
        CanShowDanmaku = true;
        RequestAddSingleDanmaku?.Invoke(this, text);
    }

    private void ResetOptions()
    {
        IsShowDanmaku = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, true);
        DanmakuOpacity = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, 0.8);
        DanmakuFontSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, 1.5d);
        DanmakuArea = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuArea, 1d);
        DanmakuSpeed = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, 1.2d);
        DanmakuFontFamily = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        IsDanmakuBold = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, true);
        IsDanmakuLimit = true;
        Location = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuLocation, DanmakuLocation.Scroll);
        Color = AppToolkit.HexToColor(SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuColor, Microsoft.UI.Colors.White.ToString()));
        IsStandardSize = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, true);
        ResetStyle();
    }

    [RelayCommand]
    private async Task ReloadFontsAsync()
    {
        var fonts = await this.Get<IFontToolkit>().GetFontsAsync();
        Fonts = fonts.ToList();
        var localFont = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, "Segoe UI");
        if (!Fonts.Contains(localFont))
        {
            DanmakuFontFamily = "Segoe UI";
        }
    }

    partial void OnIsShowDanmakuChanged(bool value)
    {
        if (value)
        {
            Redraw();
        }

        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsShowDanmaku, value);
    }

    partial void OnDanmakuOpacityChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuOpacity, value);

    partial void OnDanmakuFontSizeChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuFontSize, value);

    partial void OnIsDanmakuBoldChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsDanmakuBold, value);

    partial void OnDanmakuAreaChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuArea, value);

    partial void OnDanmakuSpeedChanged(double value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuSpeed, value);

    partial void OnDanmakuFontFamilyChanged(string value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuFontFamily, value);

    partial void OnIsStandardSizeChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.IsDanmakuStandardSize, value);

    partial void OnLocationChanged(DanmakuLocation value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuLocation, value);

    partial void OnColorChanged(Windows.UI.Color value)
        => SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.DanmakuColor, value.ToString());

    partial void OnExtraSpeedChanged(double value)
        => ExtraSpeedChanged?.Invoke(this, EventArgs.Empty);
}
