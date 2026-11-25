namespace GuardianCommunication.Hardware.GuardianController.Definition
{
    /// <summary>
    /// نوع رویداد درب؛ همین عدد کد رویداد ثبت‌شده است
    /// </summary>
    public enum GuardianControllerDoorEventTypeEnumeration
    {
        Opened = 1,
        Closed = 2,
        HeldOpen = 3,
        HeldOpenCleared = 4,
        ForcedOpen = 5,
        ExitButtonPressed = 6,
        RelayFault = 7,
    }
}
