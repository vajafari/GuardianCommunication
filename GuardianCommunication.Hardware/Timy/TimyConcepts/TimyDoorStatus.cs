namespace GuardianCommunication.Hardware.Timy.TimyConcepts
{
    enum TimyDoorStatus
    {
        FORCEOPEN = 1,
        FORCECLOSE,
        SOFTWAREOPEN,
        RESTORETOAUTO,
        REBOOT_FPA_MACHINE, //Finger printer acquisition
        DEASSERT_ALARM

    };
}
