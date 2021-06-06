using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KeepDisplayOn.WIN32APIs
{
    public static class ScreenSaverInteractions
    {
        /// <summary>
        /// Enables an application to inform the system that it is in use, thereby preventing the system from entering sleep or turning off the display while the application is running.
        /// </summary>
        /// <param name="esFlags">
        /// The thread's execution requirements. This parameter can be one or more of the following values.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the previous thread execution state.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
        /// <summary>
        /// Enables away mode. This value must be specified with ES_CONTINUOUS.
        /// Away mode should be used only by media-recording and media-distribution applications that must perform critical background processing on desktop computers while the computer appears to be sleeping.
        /// See Remarks.
        /// </summary>
        public const uint ES_AWAYMODE_REQUIRED = 0x00000040;
        /// <summary>
        /// Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
        /// </summary>
        public const uint ES_CONTINUOUS = 0x80000000;
        /// <summary>
        /// Forces the display to be on by resetting the display idle timer.
        /// </summary>
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
        /// <summary>
        /// Forces the system to be in the working state by resetting the system idle timer.
        /// </summary>
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        [Obsolete("This value is not supported. If ES_USER_PRESENT is combined with other esFlags values, the call will fail and none of the specified states will be set.")]
        public const uint ES_USER_PRESENT = 0x00000004;


        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters. This function can also update the user profile while setting a parameter.
        /// </summary>
        /// <param name="uAction"></param>
        /// <param name="lpvParam"></param>
        /// <param name="uParam"></param>
        /// <param name="fuWinIni"></param>
        /// <returns></returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern long SystemParametersInfo(uint uAction, uint lpvParam, ref uint uParam, uint fuWinIni);
        /// <summary>
        /// Determines whether a screen reviewer utility is running.
        /// A screen reviewer utility directs textual information to an output device, such as a speech synthesizer or Braille display.
        /// When this flag is set, an application should provide textual information in situations where it would otherwise present the information graphically.
        /// The pvParam parameter is a pointer to a BOOLvariable that receives TRUE if a screen reviewer utility is running, or FALSE otherwise.
        /// Note  Narrator, the screen reader that is included with Windows, does not set the SPI_SETSCREENREADER or SPI_GETSCREENREADER flags.
        /// </summary>
        public const uint SPI_GETSCREENREADER = 0x0046;
        /// <summary>
        /// Determines whether screen saving is enabled.
        /// The pvParam parameter must point to a BOOL variable that receives TRUE if screen saving is enabled, or FALSE otherwise.
        /// Windows 7, Windows Server 2008 R2 and Windows 2000: The function returns TRUE even when screen saving is not enabled.
        /// For more information and a workaround, see KB318781.
        /// </summary>
        public const uint SPI_GETSCREENSAVEACTIVE = 0x0010;
        /// <summary>
        /// Retrieves the screen saver time-out value, in seconds. The pvParam parameter must point to an integer variable that receives the value.
        /// </summary>
        public const uint SPI_GETSCREENSAVETIMEOUT = 0x000E;
        /// <summary>
        /// Sets the state of the screen saver. The uiParam parameter specifies TRUE to activate screen saving, or FALSE to deactivate it.
        /// If the machine has entered power saving mode or system lock state, an ERROR_OPERATION_IN_PROGRESS exception occurs.
        /// </summary>
        public const uint SPI_SETSCREENSAVEACTIVE = 0x0011;
        public static bool bScreenReader = true;

    }

}
