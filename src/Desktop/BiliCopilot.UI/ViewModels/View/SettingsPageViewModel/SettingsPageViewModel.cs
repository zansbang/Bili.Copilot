﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.Storage;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : AISettingsViewModelBase
{
    [RelayCommand]
    private static async Task OpenMpvConfigAsync()
    {
        var mpvConfig = await ApplicationData.Current.LocalFolder.CreateFileAsync("mpv.conf", CreationCollisionOption.OpenIfExists).AsTask();
        await Launcher.LaunchFileAsync(mpvConfig);
    }

    [RelayCommand]
    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        CheckTheme();
        IsTopNavShown = SettingsToolkit.ReadLocalSetting(SettingNames.IsTopNavBarShown, true);
        IsAutoPlayWhenLoaded = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        IsAutoPlayNextRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        AutoPlayNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, false);
        PlayNextWithoutTip = SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
        EndWithPlaylist = SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true);
        SingleFastForwardAndRewindSpan = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        IsCopyScreenshot = SettingsToolkit.ReadLocalSetting(SettingNames.CopyAfterScreenshot, true);
        IsOpenScreenshotFile = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAfterScreenshot, true);
        PlayerSpeedEnhancement = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedEnhancement, false);
        GlobalPlayerSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true);
        HideWhenCloseWindow = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        AutoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
        IsNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotificationEnabled, true);
        IsVideoMomentNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsVideoMomentNotificationEnabled, true);
        NoP2P = SettingsToolkit.ReadLocalSetting(SettingNames.PlayWithoutP2P, false);
        PlayerDisplayModeCollection = Enum.GetValues<PlayerDisplayMode>().ToList();
        PreferCodecCollection = Enum.GetValues<PreferCodecType>().ToList();
        PreferQualityCollection = Enum.GetValues<PreferQualityType>().ToList();
        PreferDecodeCollection = Enum.GetValues<PreferDecodeType>().ToList();
        PlayerTypeCollection = Enum.GetValues<PlayerType>().Where(p => p != PlayerType.Mpv).ToList();
        MTCBehaviorCollection = Enum.GetValues<MTCBehavior>().ToList();
        ExternalPlayerTypeCollection = Enum.GetValues<ExternalPlayerType>().ToList();
        DefaultPlayerDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        PreferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodecType.H264);
        PreferQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        IsPopularNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(PopularPage).Name}Visible", true);
        IsMomentNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(MomentPage).Name}Visible", true);
        IsVideoNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(VideoPartitionPage).Name}Visible", true);
        IsLiveNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(LivePartitionPage).Name}Visible", true);
        IsAnimeNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(AnimePage).Name}Visible", true);
        IsCinemaNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(CinemaPage).Name}Visible", true);
        IsArticleNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(ArticlePartitionPage).Name}Visible", true);
        try
        {
            PreferDecode = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        }
        catch (Exception)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
            PreferDecode = PreferDecodeType.Auto;
        }

        // 移除 MPV 播放器.
        if (SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island) == PlayerType.Mpv)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerType, PlayerType.Island);
        }

        PlayerType = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island);
        MTCBehavior = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic);
        ExternalPlayerType = SettingsToolkit.ReadLocalSetting(SettingNames.ExternalPlayer, ExternalPlayerType.Mpv);
        BottomProgressVisible = SettingsToolkit.ReadLocalSetting(SettingNames.IsBottomProgressVisible, true);
        DefaultDownloadPath = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadFolder, string.Empty);
        UseExternalBBDown = SettingsToolkit.ReadLocalSetting(SettingNames.UseExternalBBDown, false);
        OnlyCopyCommandWhenDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OnlyCopyCommandWhenDownload, false);
        WithoutCredentialWhenGenDownloadCommand = SettingsToolkit.ReadLocalSetting(SettingNames.WithoutCredentialWhenGenDownloadCommand, false);
        IsExternalPlayerType = PlayerType == PlayerType.External;
        IsIslandPlayerType = PlayerType == PlayerType.Island;
        FilterAISubtitle = SettingsToolkit.ReadLocalSetting(SettingNames.FilterAISubtitle, true);
        IsAIStreamingResponse = SettingsToolkit.ReadLocalSetting(SettingNames.IsAIStreamingResponse, true);
        IsMpvCustomOptionVisible = PreferDecode == PreferDecodeType.Custom && PlayerType == PlayerType.Island;
        if (string.IsNullOrEmpty(DefaultDownloadPath))
        {
            DefaultDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Bili Downloads");
        }

        OpenFolderAfterDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OpenFolderAfterDownload, true);
        DownloadWithDanmaku = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadWithDanmaku, false);
        UseWebPlayerWhenLive = SettingsToolkit.ReadLocalSetting(SettingNames.UseWebPlayerWhenLive, false);
        ShowSearchRecommend = SettingsToolkit.ReadLocalSetting(SettingNames.ShowSearchRecommend, false);

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2024);
        PackageVersion = this.Get<IAppToolkit>().GetPackageVersion();

        InitializeWebDavConfigCommand.Execute(default);

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ChooseDownloadFolderAsync()
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
        if (folder != null)
        {
            DefaultDownloadPath = folder.Path;
        }
    }

    [RelayCommand]
    private async Task OpenDownloadFolderAsync()
    {
        if (!Directory.Exists(DefaultDownloadPath))
        {
            Directory.CreateDirectory(DefaultDownloadPath);
        }

        await Launcher.LaunchFolderPathAsync(DefaultDownloadPath);
    }

    [RelayCommand]
    private async Task EditVideoSummarizeAsync()
    {
        if (!FileToolkit.IsLocalDataExist("video_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_summarize.txt", PromptConstants.VideoSummaryPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("video_summarize", ResourceToolkit.GetLocalizedString(StringNames.VideoSummarize))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditVideoEvaluationAsync()
    {
        if (!FileToolkit.IsLocalDataExist("video_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_evaluation.txt", PromptConstants.VideoEvaluationPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("video_evaluation", ResourceToolkit.GetLocalizedString(StringNames.VideoEvaluation))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditArticleSummarizeAsync()
    {
        if (!FileToolkit.IsLocalDataExist("article_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_summarize.txt", PromptConstants.ArticleSummaryPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("article_summarize", ResourceToolkit.GetLocalizedString(StringNames.ArticleSummarize))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditArticleEvaluationAsync()
    {
        if (!FileToolkit.IsLocalDataExist("article_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_evaluation.txt", PromptConstants.ArticleEvaluationPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("article_evaluation", ResourceToolkit.GetLocalizedString(StringNames.ArticleEvaluation))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    private void CheckTheme()
    {
        AppThemeText = AppTheme switch
        {
            ElementTheme.Light => ResourceToolkit.GetLocalizedString(StringNames.LightTheme),
            ElementTheme.Dark => ResourceToolkit.GetLocalizedString(StringNames.DarkTheme),
            _ => ResourceToolkit.GetLocalizedString(StringNames.SystemDefault),
        };
    }

    private void WriteNavVisibleSetting(Type pageType, bool isVisible)
    {
        this.Get<ISettingsToolkit>().WriteLocalSetting($"Is{pageType.Name}Visible", isVisible);
        this.Get<NavigationViewModel>().SetNavItemVisibility(pageType, isVisible);
    }

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(value);
    }

    partial void OnAutoPlayNextChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoPlayNext, value);

    partial void OnAutoLoadHistoryChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoLoadHistory, value);

    partial void OnIsAutoPlayWhenLoadedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ShouldAutoPlay, value);

    partial void OnDefaultPlayerDisplayModeChanged(PlayerDisplayMode value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DefaultPlayerDisplayMode, value);

    partial void OnPreferCodecChanged(PreferCodecType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferCodec, value);

    partial void OnPreferDecodeChanged(PreferDecodeType value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, value);
        IsMpvCustomOptionVisible = PreferDecode == PreferDecodeType.Custom && PlayerType == PlayerType.Island;
    }

    partial void OnPlayerTypeChanged(PlayerType value)
    {
        IsExternalPlayerType = value == PlayerType.External;
        IsIslandPlayerType = value == PlayerType.Island;
        SettingsToolkit.WriteLocalSetting(SettingNames.PlayerType, value);
    }

    partial void OnExternalPlayerTypeChanged(ExternalPlayerType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ExternalPlayer, value);

    partial void OnSingleFastForwardAndRewindSpanChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, value);

    partial void OnIsCopyScreenshotChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CopyAfterScreenshot, value);

    partial void OnIsOpenScreenshotFileChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OpenAfterScreenshot, value);

    partial void OnPlayerSpeedEnhancementChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedEnhancement, value);

    partial void OnGlobalPlayerSpeedChanged(bool value)
    {
        if (!value)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerSpeed, 1.0d);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedShare, value);
    }

    partial void OnIsAutoPlayNextRecommendVideoChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoPlayNextRecommendVideo, value);

    partial void OnPreferQualityChanged(PreferQualityType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferQuality, value);

    partial void OnHideWhenCloseWindowChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, value);

    partial void OnIsNotificationEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsNotificationEnabled, value);

    partial void OnIsVideoMomentNotificationEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsVideoMomentNotificationEnabled, value);

    partial void OnBottomProgressVisibleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsBottomProgressVisible, value);

    partial void OnNoP2PChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PlayWithoutP2P, value);

    partial void OnDefaultDownloadPathChanged(string value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DownloadFolder, value);

    partial void OnDownloadWithDanmakuChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DownloadWithDanmaku, value);

    partial void OnOpenFolderAfterDownloadChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OpenFolderAfterDownload, value);

    partial void OnUseExternalBBDownChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseExternalBBDown, value);

    partial void OnOnlyCopyCommandWhenDownloadChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OnlyCopyCommandWhenDownload, value);

    partial void OnWithoutCredentialWhenGenDownloadCommandChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.WithoutCredentialWhenGenDownloadCommand, value);

    partial void OnIsWebDavEnabledChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsWebDavEnabled, value);
        this.Get<NavigationViewModel>().CheckWebDavItemCommand.Execute(default);
    }

    partial void OnSelectedWebDavChanged(WebDavConfig value)
    {
        if (value is null)
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.SelectedWebDavConfigId);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.SelectedWebDavConfigId, value.Id);
        }
    }

    partial void OnFilterAISubtitleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.FilterAISubtitle, value);

    partial void OnPlayNextWithoutTipChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PlayNextWithoutTip, value);

    partial void OnIsTopNavShownChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsTopNavBarShown, value);
        this.Get<NavigationViewModel>().IsTopNavBarShown = value;
    }

    partial void OnMTCBehaviorChanged(MTCBehavior value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MTCBehavior, value);

    partial void OnIsAIStreamingResponseChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsAIStreamingResponse, value);

    partial void OnUseWebPlayerWhenLiveChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseWebPlayerWhenLive, value);

    partial void OnShowSearchRecommendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ShowSearchRecommend, value);

    partial void OnIsPopularNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(PopularPage), value);

    partial void OnIsMomentNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(MomentPage), value);

    partial void OnIsVideoNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(VideoPartitionPage), value);

    partial void OnIsLiveNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(LivePartitionPage), value);

    partial void OnIsAnimeNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(AnimePage), value);

    partial void OnIsCinemaNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(CinemaPage), value);

    partial void OnIsArticleNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(ArticlePartitionPage), value);
}
