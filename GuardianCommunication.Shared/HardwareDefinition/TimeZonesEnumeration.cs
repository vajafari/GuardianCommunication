using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    [DataContract]
    public enum TimeZonesEnumeration
    {
        [EnumMember]
        EniwetokKwajalein = -1200,
        [EnumMember]
        MidwayIslandSamoa = -1100,
        [EnumMember]
        Hawaii = -1000,
        [EnumMember]
        Taiohae = -930,
        [EnumMember]
        Alaska = -900,
        [EnumMember]
        PacificTimeUsCanada = -800,
        [EnumMember]
        MountainTimeUsCanada = -700,
        [EnumMember]
        CentralTimeUsCanadaMexicoCity = -600,
        [EnumMember]
        EasternTimeUsCanadaBogotaLima = -500,
        [EnumMember]
        Caracas = -430,
        [EnumMember]
        AtlanticTimeCanadaCaracasLaPaz = -400,
        [EnumMember]
        Newfoundland = -330,
        [EnumMember]
        BrazilBuenosAiresGeorgetown = -300,
        [EnumMember]
        MidAtlantic = -200,
        [EnumMember]
        AzoresCapeVerdeIslands = -100,
        [EnumMember]
        WesternEuropeTimeLondonLisbonCasablanca = 0,
        [EnumMember]
        BrusselsCopenhagenMadridParis = 100,
        [EnumMember]
        KaliningradSouthAfrica = 200,
        [EnumMember]
        BaghdadRiyadhMoscowStPetersburg = 300,
        [EnumMember]
        Tehran = 330,
        [EnumMember]
        AbuDhabiMuscatBakuTbilisi = 400,
        [EnumMember]
        Kabul = 430,
        [EnumMember]
        EkaterinburgIslamabadKarachiTashkent = 500,
        [EnumMember]
        BombayCalcuttaMadrasNewDelhi = 530,
        [EnumMember]
        KathmanduPokhara = 545,
        [EnumMember]
        AlmatyDhakaColombo = 600,
        [EnumMember]
        YangonMandalay = 630,
        [EnumMember]
        BangkokHanoiJakarta = 700,
        [EnumMember]
        BeijingPerthSingaporeHongKong = 800,
        [EnumMember]
        Eucla = 845,
        [EnumMember]
        TokyoSeoulOsakaSapporoYakutsk = 900,
        [EnumMember]
        AdelaideDarwin = 930,
        [EnumMember]
        EasternAustraliaGuamVladivostok = 1000,
        [EnumMember]
        LordHoweIsland = 1030,
        [EnumMember]
        MagadanSolomonIslandsNewCaledonia = 1100,
        [EnumMember]
        NorfolkIsland = 1130,
        [EnumMember]
        AucklandWellingtonFijiKamchatka = 1200,
        [EnumMember]
        ChathamIslands = 1245,
        [EnumMember]
        ApiaNukualofa = 1300,
        [EnumMember]
        LineIslandsTokelau = 1400
    }
}
