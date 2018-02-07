using System;
using System.Runtime.InteropServices;
using System.Text;

namespace VideoPlayer40.LAVInterfaces
{

    public enum LAVSubtitleMode
    {
        LAVSubtitleMode_NoSubs,
        LAVSubtitleMode_ForcedOnly,
        LAVSubtitleMode_Default,
        LAVSubtitleMode_Advanced
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("774A919D-EA95-4A87-8A1E-F48ABE8499C7")]
    public interface ILAVSplitterSourceSettings
    {
        [PreserveSig]
        int SetRuntimeConfig([MarshalAs(UnmanagedType.Bool)] bool bRuntimeConfig);

        [PreserveSig]
        int GetPreferredLanguages([Out, MarshalAs(UnmanagedType.LPWStr)] out StringBuilder ppLanguages);

        [PreserveSig]
        int SetPreferredLanguages([MarshalAs(UnmanagedType.LPWStr)] string pLanguages);

        [PreserveSig]
        int GetPreferredSubtitleLanguages([Out, MarshalAs(UnmanagedType.LPWStr)] out StringBuilder ppLanguages);

        [PreserveSig]
        int SetPreferredSubtitleLanguages([MarshalAs(UnmanagedType.LPWStr)] string pLanguages);

        [PreserveSig]
        LAVSubtitleMode GetSubtitleMode();

        [PreserveSig]
        int SetSubtitleMode([In] LAVSubtitleMode mode);

        [Obsolete("Do not use anymore, deprecated and non-functional, replaced by advanced subtitle mode", true)]
        [PreserveSig]
        bool GetSubtitleMatchingLanguage();

        [Obsolete("Do not use anymore, deprecated and non-functional, replaced by advanced subtitle mode", true)]
        [PreserveSig]
        int SetSubtitleMatchingLanguage([MarshalAs(UnmanagedType.Bool)] bool dwMode);

        [PreserveSig]
        bool GetPGSForcedStream();

        [PreserveSig]
        int SetPGSForcedStream([MarshalAs(UnmanagedType.Bool)] bool bFlag);

        [PreserveSig]
        bool GetPGSOnlyForced();

        [PreserveSig]
        int SetPGSOnlyForced([MarshalAs(UnmanagedType.Bool)] bool bForced);

        [PreserveSig]
        int GetVC1TimestampMode();

        [PreserveSig]
        int SetVC1TimestampMode(int iMode);

        [PreserveSig]
        bool GetSubstreamsEnabled();

        [PreserveSig]
        int SetSubstreamsEnabled([MarshalAs(UnmanagedType.Bool)] bool bSubStreams);

        [Obsolete("No longer required", false)]
        [PreserveSig]
        int SetVideoParsingEnabled([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [Obsolete("No longer required", false)]
        [PreserveSig]
        bool GetVideoParsingEnabled();

        [Obsolete("No longer required", false)]
        [PreserveSig]
        int SetFixBrokenHDPVR([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [Obsolete("No longer required", false)]
        [PreserveSig]
        bool GetFixBrokenHDPVR();

        [PreserveSig]
        int SetFormatEnabled(
            [MarshalAs(UnmanagedType.LPStr)] string strFormat,
            [MarshalAs(UnmanagedType.Bool)] bool bEnabled
        );

        [PreserveSig]
        bool IsFormatEnabled([MarshalAs(UnmanagedType.LPStr)] string strFormat);

        [PreserveSig]
        int SetStreamSwitchRemoveAudio([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetStreamSwitchRemoveAudio();

        [PreserveSig]
        int GetAdvancedSubtitleConfig([Out, MarshalAs(UnmanagedType.LPStr)] out StringBuilder ppAdvancedConfig);

        int SetAdvancedSubtitleConfig([MarshalAs(UnmanagedType.LPStr)] string pAdvancedConfig);

        [PreserveSig]
        int SetUseAudioForHearingVisuallyImpaired([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetUseAudioForHearingVisuallyImpaired();

        [PreserveSig]
        int SetMaxQueueMemSize(uint dwMaxSize);

        [PreserveSig]
        uint GetMaxQueueMemSize();

        [PreserveSig]
        int SetTrayIcon([MarshalAs(UnmanagedType.Bool)] bool bEnabled);

        [PreserveSig]
        bool GetTrayIcon();

        [PreserveSig]
        int SetPreferHighQualityAudioStreams(bool bEnabled);

        [PreserveSig]
        bool GetPreferHighQualityAudioStreams();

        [PreserveSig]
        int SetLoadMatroskaExternalSegments(bool bEnabled);

        [PreserveSig]
        bool GetLoadMatroskaExternalSegments();

        [PreserveSig]
        int GetFormats([Out, MarshalAs(UnmanagedType.LPTStr)] string formats, uint nFormats);

        [PreserveSig]
        int SetNetworkStreamAnalysisDuration(uint dwDuration);

        [PreserveSig]
        uint GetNetworkStreamAnalysisDuration();

        [PreserveSig]
        int SetMaxQueueSize(uint dwMaxSize);

        [PreserveSig]
        uint GetMaxQueueSize();
    }
}
